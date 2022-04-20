using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyGetNodeAddition : OnReadyGetAddition
	{
		public OnReadyGetNodeAddition(MemberAttributeSite site) : base(site) { }

		public override Action<SourceStringBuilder>? DeclarationWriter => g =>
		{
			string export = Path is not { Length: > 0 } || Export
				? "[Export] "
				: "";

			g.Line();
			g.Line(export, "public NodePath ", ExportPropertyName, " { get; set; }");

			if (Path is { Length: > 0 })
			{
				g.BlockTab(() =>
				{
					g.Line("= ", SyntaxFactory.Literal(Path).ToString(), ";");
				});
			}
		};

		public override Action<SourceStringBuilder>? OnReadyStatementWriter => g =>
		{
			g.Line();

			g.Line("if (", ExportPropertyName, " != null)");
			g.BlockBrace(() =>
			{
				if (Property is { Length: > 0 } property)
				{
					g.Line(Member.Name, " = " +
						"(", Member.Type.ToFullDisplayString(), ") " +
						"GetNode", (OrNull ? "OrNull" : "") +
						"(", ExportPropertyName, ")?.Get(" +
						SyntaxFactory.Literal(property), ");");
				}
				else
				{
					g.Line(Member.Name, " = GetNode" +
						(OrNull ? "OrNull" : "") +
						"<", Member.Type.ToFullDisplayString(), ">" +
						"(", ExportPropertyName, ");");
				}
			});

			if (!OrNull)
			{
				WriteMemberNullCheck(g, ExportPropertyName);
			}
		};

		protected virtual string ExportPropertyName => SuffixlessExportPropertyName + "Path";
	}
}
