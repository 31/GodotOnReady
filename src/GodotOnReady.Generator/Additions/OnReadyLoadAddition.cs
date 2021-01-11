using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using System;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyLoadAddition : PartialClassAddition
	{
		public string? MemberType { get; set; }
		public string? MemberName { get; set; }
		public string? MemberPathName { get; set; }

		public string? Default { get; set; }

		public bool OrNull { get; set; }

		public OnReadyLoadAddition(AttributeData attribute)
		{
			foreach (var constructorArg in attribute.ConstructorArguments)
			{
				if (constructorArg.Value is string v)
				{
					Default = v;
				}
			}

			foreach (var namedArg in attribute.NamedArguments)
			{
				if (namedArg.Key == "Default" && namedArg.Value.Value is string v)
				{
					Default = v;
				}
				else if (namedArg.Key == "OrNull" && namedArg.Value.Value is bool b)
				{
					OrNull = b;
				}
			}
		}

		public override void WriteDeclaration(SourceStringBuilder g)
		{
			if (MemberName is null) return;
			if (MemberType is null) return;

			g.Line("[Export] public ", MemberType, " ", MemberPathNameOrDefault);
			g.BlockBrace(() =>
			{
				g.Line("get => ", MemberName, ";");
				g.Line("set { _hasBeenSet", MemberName, " = true; ", MemberName, " = value; }");
			});

			g.Line("private bool _hasBeenSet", MemberName, ";");
		}

		public override bool WritesConstructorStatements => Default is { Length: > 0 };

		public override void WriteConstructorStatement(SourceStringBuilder g)
		{
			if (MemberType is null) return;
			if (MemberName is null) return;

			g.Line("if (Engine.EditorHint)");
			g.BlockBrace(() =>
			{
				WriteAssignment(g);
			});
		}

		public override bool WritesOnReadyStatements => Default is { Length: > 0 } && !OrNull;

		public override void WriteOnReadyStatement(SourceStringBuilder g)
		{
			if (MemberType is null) return;
			if (MemberName is null) return;

			if (Default is { Length: >0 })
			{
				g.Line("if (!_hasBeenSet", MemberName, ")");
				g.BlockBrace(() =>
				{
					WriteAssignment(g);
				});
			}

			if (!OrNull)
			{
				g.Line("if (", MemberName, " == null) " +
					"throw new NullReferenceException($\"",
					MemberName, " is null, but OnReadyLoad didn't say OrNull=true. (Default = '",
					Default ?? "null", "') ",
					"(Node = '{Name}' '{this}')\");");
			}
		}

		private void WriteAssignment(SourceStringBuilder g)
		{
			if (MemberType is null) return;
			if (MemberName is null) return;
			if (Default is not { Length: >0 }) return;

			g.Line(MemberPathNameOrDefault, " = GD.Load",
				"<", MemberType, ">",
				"(", SyntaxFactory.Literal(Default).ToString(), ");");
		}

		private string MemberPathNameOrDefault => MemberPathName ?? (MemberName ?? "") + "Resource";
	}
}
