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

			var onReadyPathSymbol = GetSymbolByName("GodotOnReady.Attributes.OnReadyPathAttribute");
			var onReadySymbol = GetSymbolByName("GodotOnReady.Attributes.OnReadyAttribute");
			var onReadyLoadSymbol = GetSymbolByName("GodotOnReady.Attributes.OnReadyLoadAttribute");

			List<PartialClassAddition> additions = new();

			foreach (var classDecl in receiver.AllClasses)
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
					continue;
				}

				foreach (var propertySymbol in classSymbol.GetMembers().OfType<IPropertySymbol>())
				{
					foreach (var attribute in propertySymbol.GetAttributes())
					{
						if (Equal(attribute.AttributeClass, onReadyPathSymbol))
						{
							additions.Add(new OnReadyPathAddition(attribute)
							{
								Class = classSymbol,
								MemberType = propertySymbol.Type.ToFullDisplayString(),
								MemberName = propertySymbol.Name,
							});
						}
					}
				}

				foreach (var fieldSymbol in classSymbol.GetMembers().OfType<IFieldSymbol>())
				{
					foreach (var attribute in fieldSymbol.GetAttributes())
					{
						if (Equal(attribute.AttributeClass, onReadyPathSymbol))
						{
							additions.Add(new OnReadyPathAddition(attribute)
							{
								Class = classSymbol,
								MemberType = fieldSymbol.Type.ToFullDisplayString(),
								MemberName = fieldSymbol.Name,
								MemberPathName = GetPropertyNameForFieldName(fieldSymbol.Name) + "Path",
							});
						}
						else if (Equal(attribute.AttributeClass, onReadyLoadSymbol))
						{
							additions.Add(new OnReadyLoadAddition(attribute)
							{
								Class = classSymbol,
								MemberType = fieldSymbol.Type.ToFullDisplayString(),
								MemberName = fieldSymbol.Name,
								MemberPathName = GetPropertyNameForFieldName(fieldSymbol.Name) + "Resource",
							});
						}
					}
				}

				foreach (var methodSymbol in classSymbol.GetMembers().OfType<IMethodSymbol>())
				{
					foreach (var attribute in methodSymbol
						.GetAttributes()
						.Where(a => Equal(a.AttributeClass, onReadySymbol)))
					{
						additions.Add(new OnReadyAddition(attribute)
						{
							Class = classSymbol,
							Method = methodSymbol,
						});
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
					source.BlockBrace(() =>
					{
						foreach (var addition in classAdditionGroup)
						{
							addition.WriteDeclaration(source);
						}

						if (classAdditionGroup.Any(a => a.WritesConstructorStatements))
						{
							source.Line();
							source.Line("public ", classAdditionGroup.Key.Name, "()");
							source.BlockBrace(() =>
							{
								foreach (var addition in classAdditionGroup.OrderBy(a => a.Order))
								{
									addition.WriteConstructorStatement(source);
								}

								source.Line("Constructor();");
							});

							source.Line("partial void Constructor();");
						}

						if (classAdditionGroup.Any(a => a.WritesOnReadyStatements))
						{
							source.Line();
							source.Line("public override void _Ready()");
							source.BlockBrace(() =>
							{
								// OrderBy is a stable sort.
								// Sort by Order, then by discovery order (implicitly).
								foreach (var addition in classAdditionGroup.OrderBy(a => a.Order))
								{
									addition.WriteOnReadyStatement(source);
								}
							});
						}
					});
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

		private static string GetPropertyNameForFieldName(string fieldName)
		{
			// Handle field name convention: _ prefix with lowercase name.
			string pathName = fieldName.TrimStart('_');
			pathName =
				pathName[0].ToString().ToUpperInvariant() +
				pathName.Substring(1);

			return pathName;
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
