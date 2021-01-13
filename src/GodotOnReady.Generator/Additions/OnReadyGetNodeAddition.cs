using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis.CSharp;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyGetNodeAddition : OnReadyGetAddition
	{
		public OnReadyGetNodeAddition(MemberAttributeSite site) : base(site) { }

		public override void WriteDeclaration(SourceStringBuilder g)
		{
			string export = Private ? "" : "[Export] ";

			g.Line();
			g.Line(export, "public NodePath ", ExportPropertyName, " { get; set; }");

			if (Default is { Length: >0 })
			{
				g.BlockTab(() =>
				{
					g.Line("= ", SyntaxFactory.Literal(Default).ToString(), ";");
				});
			}
		}

		public override bool WritesOnReadyStatements => true;

		public override void WriteOnReadyStatement(SourceStringBuilder g)
		{
			if (OrNull)
			{
				g.Line("if (", ExportPropertyName, " != null)");
				g.BlockBrace(() => WriteGetNodeLine(g));
				g.Line();
			}
			else
			{
				WriteGetNodeLine(g);
			}
		}

		private void WriteGetNodeLine(SourceStringBuilder g)
		{
			g.Line(Member.Name, " = GetNode" +
				(OrNull ? "OrNull" : "") +
				"<", Member.Type.ToFullDisplayString(), ">" +
				"(", ExportPropertyName, ");");
		}

		protected virtual string ExportPropertyName => SuffixlessExportPropertyName + "Path";
	}
}
