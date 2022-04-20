using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyFindNodeAddition : OnReadyGetAddition
	{
		public OnReadyFindNodeAddition(MemberAttributeSite site) : base(site) { }

		public override Action<SourceStringBuilder>? DeclarationWriter => g =>
		{
			string export = Path is not { Length: > 0 } || Export
				? "[Export] "
				: "";

			g.Line();
			g.Line(export, "public string ", ExportPropertyName, " { get; set; }");

			g.BlockTab(() =>
			{
				if (Path is { Length: > 0 })
				{
					g.Line("= ", SyntaxFactory.Literal(Path).ToString(), ";");
				}
				else
				{
					g.Line("= \"\";");
				}
			});
		};

		public override Action<SourceStringBuilder>? OnReadyStatementWriter => g =>
		{
			g.Line();

			g.Line("if (", ExportPropertyName, " != null)");
			g.BlockBrace(() =>
			{
				g.Line(Member.Name, " = ",
					"(", Member.Type.ToFullDisplayString(), ")",
					"FindNode", "(", ExportPropertyName);

				g.BlockTab(() =>
				{
					if (NonRecursive)
					{
						g.Line(", recursive: false");
					}
					if (Unowned)
					{
						g.Line(", owned: false");
					}

					g.Line(")");

					if (Property is { Length: > 0 } property)
					{
						g.Line("?.Get(", SyntaxFactory.Literal(property).ToString(), ")");
					}

					g.Line(";");
				});
			});

			if (!OrNull)
			{
				WriteMemberNullCheck(g, ExportPropertyName);
			}
		};

		protected virtual string ExportPropertyName => SuffixlessExportPropertyName + "Mask";
	}
}
