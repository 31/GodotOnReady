using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis.CSharp;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyGetResourceAddition : OnReadyGetAddition
	{
		public OnReadyGetResourceAddition(MemberAttributeSite site) : base(site) { }

		private bool IsGeneratingAssignment => Default is { Length: >0 };

		public override void WriteDeclaration(SourceStringBuilder g)
		{
			string export = Private ? "" : "[Export] ";

			g.Line();
			g.Line(export, "public ", Member.Type.ToFullDisplayString(), " ", ExportPropertyName);
			g.BlockBrace(() =>
			{
				g.Line("get => ", Member.Name, ";");
				g.Line("set { _hasBeenSet", Member.Name, " = true; ", Member.Name, " = value; }");
			});

			g.Line("private bool _hasBeenSet", Member.Name, ";");
		}

		public override bool WritesConstructorStatements => IsGeneratingAssignment;

		public override void WriteConstructorStatement(SourceStringBuilder g)
		{
			if (!IsGeneratingAssignment) return;

			g.Line("if (Engine.EditorHint)");
			g.BlockBrace(() =>
			{
				WriteAssignment(g);
			});
		}

		public override bool WritesOnReadyStatements => true;

		public override void WriteOnReadyStatement(SourceStringBuilder g)
		{
			if (IsGeneratingAssignment || !OrNull)
			{
				g.Line();
			}

			if (IsGeneratingAssignment)
			{
				g.Line("if (!_hasBeenSet", Member.Name, ")");
				g.BlockBrace(() =>
				{
					WriteAssignment(g);
				});
			}

			if (!OrNull)
			{
				g.Line("if (", Member.Name, " == null)");
				g.BlockBrace(() =>
				{
					g.Line(
						"throw new NullReferenceException($\"",
						Member.Name, " is null, but OnReadyLoad not OrNull=true. (Default = '",
						Default ?? "null", "') ",
						"(Node = '{Name}' '{this}')\");");
				});
			}
		}

		private void WriteAssignment(SourceStringBuilder g)
		{
			g.Line(Member.Name, " = GD.Load",
				"<", Member.Type.ToFullDisplayString(), ">",
				"(", SyntaxFactory.Literal(Default ?? "").ToString(), ");");
		}

		protected virtual string ExportPropertyName => SuffixlessExportPropertyName + "Resource";
	}
}
