using GodotOnReady.Generator.Additions;
using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GodotOnReady.Generator
{
	public record AttributeSite(
		INamedTypeSymbol Class,
		AttributeData Attribute);

	public record MemberAttributeSite(
		MemberSymbol Member,
		AttributeSite AttributeSite);

	[Generator]
	public class GodotOnReadySourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new OnReadyReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			// If this isn't working, run 'dotnet build-server shutdown' first.
			if (Environment
				.GetEnvironmentVariable($"Debug{nameof(GodotOnReadySourceGenerator)}") == "true")
			{
				Debugger.Launch();
			}

			var receiver = context.SyntaxReceiver as OnReadyReceiver ?? throw new Exception();

			INamedTypeSymbol GetSymbolByName(string fullName) =>
				context.Compilation.GetTypeByMetadataName(fullName)
				?? throw new Exception($"Can't find {fullName}");

			var onReadyGetSymbol = GetSymbolByName("GodotOnReady.Attributes.OnReadyGetAttribute");
			var onReadySymbol = GetSymbolByName("GodotOnReady.Attributes.OnReadyAttribute");
			var generateDataSelectorEnumSymbol =
				GetSymbolByName("GodotOnReady.Attributes.GenerateDataSelectorEnumAttribute");

			var resourceSymbol = GetSymbolByName("Godot.Resource");
			var nodeSymbol = GetSymbolByName("Godot.Node");

			List<PartialClassAddition> additions = new();

			var classSymbols = receiver.AllClasses
				.Select(classDecl =>
				{
					INamedTypeSymbol? classSymbol = context.Compilation
						.GetSemanticModel(classDecl.SyntaxTree)
						.GetDeclaredSymbol(classDecl);

					if (classSymbol is null)
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								new DiagnosticDescriptor(
									"GORSG0001",
									"Inspection",
									$"Unable to find declared symbol for {classDecl}. Skipping.",
									"GORSG.Parsing",
									DiagnosticSeverity.Warning,
									true
								),
								classDecl.GetLocation()
							)
						);
					}

					return classSymbol;
				})
				.Distinct(SymbolEqualityComparer.Default)
				.OfType<INamedTypeSymbol>();

			foreach (var classSymbol in classSymbols)
			{
				foreach (var attribute in classSymbol.GetAttributes()
					.Where(a => Equal(a.AttributeClass, generateDataSelectorEnumSymbol)))
				{
					var fields = classSymbol.GetMembers()
						.OfType<IFieldSymbol>()
						.Where(f => f.IsReadOnly && f.IsStatic)
						.ToArray();

					additions.Add(new DataSelectorEnumAddition(
						fields,
						new AttributeSite(classSymbol, attribute)));
				}

				var members = Enumerable
					.Concat(
						classSymbol.GetMembers().OfType<IPropertySymbol>().Select(MemberSymbol.Create),
						classSymbol.GetMembers().OfType<IFieldSymbol>().Select(MemberSymbol.Create))
					.ToArray();

				foreach (var member in members)
				{
					foreach (var attribute in member.Symbol
						.GetAttributes()
						.Where(a => Equal(a.AttributeClass, onReadyGetSymbol)))
					{
						var site = new MemberAttributeSite(
							member,
							new AttributeSite(classSymbol, attribute));

						if (site.AttributeSite.Attribute.NamedArguments.Any(
							a => a.Key == "Property" && a.Value.Value is string { Length: > 0 }))
						{
							additions.Add(new OnReadyGetNodePropertyAddition(site));
						}
						else if (member.Type.IsOfBaseType(nodeSymbol))
						{
							additions.Add(new OnReadyGetNodeAddition(site));
						}
						else if (member.Type.IsOfBaseType(resourceSymbol))
						{
							additions.Add(new OnReadyGetResourceAddition(site));
						}
						else
						{
							string issue =
								$"The type '{member.Type}' of '{member.Symbol}' is not supported." +
								" Expected a Resource or Node subclass.";

							context.ReportDiagnostic(
								Diagnostic.Create(
									new DiagnosticDescriptor(
										"GORSG0002",
										"Inspection",
										issue,
										"GORSG.Parsing",
										DiagnosticSeverity.Error,
										true
									),
									member.Symbol.Locations.FirstOrDefault()
								)
							);
						}
					}
				}

				foreach (var methodSymbol in classSymbol.GetMembers().OfType<IMethodSymbol>())
				{
					foreach (var attribute in methodSymbol
						.GetAttributes()
						.Where(a => Equal(a.AttributeClass, onReadySymbol)))
					{
						additions.Add(new OnReadyAddition(methodSymbol, attribute, classSymbol));
					}
				}
			}

			foreach (var classAdditionGroup in additions.GroupBy(a => a.Class))
			{
				SourceStringBuilder source = CreateInitializedSourceBuilder();

				if (classAdditionGroup.Key is not { } classSymbol) continue;

				source.NamespaceBlockBraceIfExists(classSymbol.GetSymbolNamespaceName(), () =>
				{
					source.Line("public partial class ", classAdditionGroup.Key.Name);
					if (classAdditionGroup.Key.IsGenericType)
					{
						source.BlockTab(() =>
						{
							source.Line(
								"<",
								string.Join(
									", ",
									classAdditionGroup.Key.TypeParameters
										.Select(p => p.ToFullDisplayString())),
								">");
						});
					}

					source.BlockBrace(() =>
					{
						foreach (var addition in classAdditionGroup)
						{
							addition.DeclarationWriter?.Invoke(source);
						}

						if (classAdditionGroup.Any(a => a.ConstructorStatementWriter is not null))
						{
							source.Line();
							source.Line("public ", classAdditionGroup.Key.Name, "()");
							source.BlockBrace(() =>
							{
								foreach (var addition in classAdditionGroup.OrderBy(a => a.Order))
								{
									addition.ConstructorStatementWriter?.Invoke(source);
								}

								source.Line("Constructor();");
							});

							source.Line("partial void Constructor();");
						}

						if (classAdditionGroup.Any(a => a.OnReadyStatementWriter is not null))
						{
							source.Line();
							source.Line("public override void _Ready()");
							source.BlockBrace(() =>
							{
								source.Line("base._Ready();");

								// OrderBy is a stable sort.
								// Sort by Order, then by discovery order (implicitly).
								foreach (var addition in classAdditionGroup.OrderBy(a => a.Order))
								{
									addition.OnReadyStatementWriter?.Invoke(source);
								}
							});
						}
					});

					foreach (var addition in classAdditionGroup)
					{
						addition.OutsideClassStatementWriter?.Invoke(source);
					}
				});

				string escapedNamespace =
					classAdditionGroup.Key.GetSymbolNamespaceName()?.Replace(".", "_") ?? "";

				context.AddSource(
					$"Partial_{escapedNamespace}_{classAdditionGroup.Key.Name}",
					source.ToString());
			}
		}

		private static SourceStringBuilder CreateInitializedSourceBuilder()
		{
			var builder = new SourceStringBuilder();
			builder.Line("using Godot;");
			builder.Line("using System;");
			builder.Line();
			return builder;
		}

		private static bool Equal(ISymbol? a, ISymbol? b)
		{
			return SymbolEqualityComparer.Default.Equals(a, b);
		}

		private class OnReadyReceiver : ISyntaxReceiver
		{
			public List<ClassDeclarationSyntax> AllClasses { get; } = new();

			public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
			{
				if (syntaxNode is ClassDeclarationSyntax cds)
				{
					AllClasses.Add(cds);
				}
			}
		}
	}
}
