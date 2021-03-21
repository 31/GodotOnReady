using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyGetResourceAddition : OnReadyGetAddition
	{
		public OnReadyGetResourceAddition(MemberAttributeSite site) : base(site) { }

		private bool IsGeneratingAssignment => Path is { Length: >0 };

		public override Action<SourceStringBuilder>? DeclarationWriter => g =>
		{
			string export = Path is { Length: >0 } || Export
				? "[Export] "
				: "";

			g.Line();
			g.Line(export, "public ", Member.Type.ToFullDisplayString(), " ", ExportPropertyName);
			g.BlockBrace(() =>
			{
				g.Line("get => ", Member.Name, ";");
				g.Line("set { _hasBeenSet", Member.Name, " = true; ", Member.Name, " = value; }");
			});

			g.Line("private bool _hasBeenSet", Member.Name, ";");
		};

		public override Action<SourceStringBuilder>? ConstructorStatementWriter =>
			IsGeneratingAssignment
				? g =>
				{
					g.Line("if (Engine.EditorHint)");
					g.BlockBrace(() =>
					{
						WriteAssignment(g);
					});
				}
				: null;

		public override Action<SourceStringBuilder>? OnReadyStatementWriter =>
			IsGeneratingAssignment || !OrNull
				? g =>
				{
					g.Line();

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
						WriteMemberNullCheck(g, ExportPropertyName);
					}
				}
				: null;

		private void WriteAssignment(SourceStringBuilder g)
		{
			g.Line(Member.Name, " = GD.Load",
				"<", Member.Type.ToFullDisplayString(), ">",
				"(", SyntaxFactory.Literal(Path ?? "").ToString(), ");");
		}

		protected virtual string ExportPropertyName => SuffixlessExportPropertyName + "Resource";
	}
}
