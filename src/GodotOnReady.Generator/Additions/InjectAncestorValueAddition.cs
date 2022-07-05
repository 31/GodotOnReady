using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;

namespace GodotOnReady.Generator.Additions
{
	public class InjectAncestorValueAddition : PartialClassAddition
	{
		public MemberSymbol Member { get; }

		private readonly string _nodeName;
		private readonly INamedTypeSymbol _nodeType;
		private readonly MemberSymbol? _sourceMember;

		public InjectAncestorValueAddition(MemberAttributeSite memberSite)
			: base(memberSite.AttributeSite.Class)
		{
			Member = memberSite.Member;

			_nodeType = (INamedTypeSymbol)Member.Type;
			_nodeName = _nodeType.Name;

			var args = memberSite.AttributeSite.Attribute.ConstructorArguments;
			if (args.Length > 0)
			{
				if (args[0].Value is INamedTypeSymbol s)
				{
					_nodeType = s;
					_nodeName = _nodeType.Name;
				}
			}
			if (args.Length > 1)
			{
				if (args[1].Value is string s)
				{
					_nodeName = s;
				}
			}

			// We don't want the node, we want a property or field of the node with a specific type.
			if (!Member.Type.Equals(_nodeType, SymbolEqualityComparer.Default))
			{
				var matches = MemberSymbol.CreateAll(_nodeType)
					.Where(s =>
						// Ignore backing fields.
						!s.Symbol.IsImplicitlyDeclared &&
						s.Type.IsOfBaseType(Member.Type))
					.ToArray();

				if (matches.Length == 0)
				{
					string issue =
						$"No field or property of type \"{Member.Type.ToFullDisplayString()}\" " +
						$"found in injection source \"{_nodeType.ToFullDisplayString()}\". " +
						"Expected exactly one";

					Diagnostics.Add(
						Diagnostic.Create(
							new DiagnosticDescriptor(
								"GORSG0004",
								"Inspection",
								issue,
								"GORSG.Parsing",
								DiagnosticSeverity.Error,
								true
							),
							Member.Symbol.Locations.FirstOrDefault()
						)
					);
				}
				else if (matches.Length > 1)
				{
					string issue =
						$"Multiple members of type \"{Member.Type.ToFullDisplayString()}\" " +
						$"found in injection source {_nodeType.ToFullDisplayString()}, " +
						"but expected exactly one. Found: " +
						string.Join(", ", matches.Select(m => m.Name));

					Diagnostics.Add(
						Diagnostic.Create(
							new DiagnosticDescriptor(
								"GORSG0005",
								"Inspection",
								issue,
								"GORSG.Parsing",
								DiagnosticSeverity.Error,
								true
							),
							Member.Symbol.Locations.FirstOrDefault()
						)
					);
				}
				else
				{
					_sourceMember = matches[0];
				}
			}
		}

		public override Action<SourceStringBuilder>? OnReadyStatementWriter => g =>
		{
			if (Diagnostics.Any())
			{
				return;
			}

			string noParent = $"FindParent(\"{_nodeName}\") found no parent.";
			string wrongType = $"FindParent(\"{_nodeName}\") is not of type \"{_nodeType}\".";

			g.Line();
			g.BlockBrace(() =>
			{
				g.Line(
					"var ancestor = (FindParent(",
					SyntaxFactory.Literal(_nodeName).ToString(),
					")",
					" ?? throw new Exception(",
					SyntaxFactory.Literal(noParent).ToString(),
					"))");
				g.BlockTab(() =>
				{
					g.Line(
						"as ",
						_nodeType.ToFullDisplayString(),
						" ?? throw new Exception(",
						SyntaxFactory.Literal(wrongType).ToString(),
						")",
						";");
				});
				if (_sourceMember is null)
				{
					g.Line(Member.Name, " = ancestor;");
				}
				else
				{
					g.Line(Member.Name, " = ancestor.", _sourceMember.Name, ";");
				}
			});
		};
	}
}
