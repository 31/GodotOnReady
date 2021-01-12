using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyGetAddition : PartialClassAddition
	{
		public string MemberType { get; set; }
		public string MemberName { get; set; }
		public string SuffixlessExportPropertyName { get; set; }

		public GettableBaseType BaseType { get; }

		public string? Default { get; set; }

		public bool OrNull { get; set; }

		public bool Private { get; set; }

		private OnReadyGetAddition(
			AttributeData attribute,
			ITypeSymbol memberTypeSymbol,
			string memberName,
			string suffixlessExportPropertyName,
			GettableBaseType baseType)
		{
			MemberType = memberTypeSymbol.ToFullDisplayString();
			MemberName = memberName;
			SuffixlessExportPropertyName = suffixlessExportPropertyName;
			BaseType = baseType;

			foreach (var constructorArg in attribute.ConstructorArguments)
			{
				if (constructorArg.Value is string @default)
				{
					Default = @default;
				}
			}

			foreach (var namedArg in attribute.NamedArguments)
			{
				if (namedArg.Key == "Default" && namedArg.Value.Value is string @default)
				{
					Default = @default;
				}
				else if (namedArg.Key == "OrNull" && namedArg.Value.Value is bool orNull)
				{
					OrNull = orNull;
				}
				else if (namedArg.Key == "Private" && namedArg.Value.Value is bool @private)
				{
					Private = @private;
				}
			}
		}

		public OnReadyGetAddition(
			AttributeData attribute,
			IPropertySymbol propertySymbol,
			GettableBaseType baseType)
			: this(
				attribute,
				propertySymbol.Type,
				propertySymbol.Name,
				propertySymbol.Name, baseType) { }

		public OnReadyGetAddition(
			AttributeData attribute,
			IFieldSymbol fieldSymbol,
			GettableBaseType baseType)
			: this(
				attribute,
				fieldSymbol.Type,
				fieldSymbol.Name,
				GetSuffixlessExportPropertyNameForMemberName(fieldSymbol.Name), baseType) { }

		public override void WriteDeclaration(SourceStringBuilder g)
		{
			string export = Private ? "" : "[Export] ";

			switch (BaseType)
			{
				case GettableBaseType.Resource:
					g.Line(export, "public ", MemberType, " ", ExportPropertyName);
					g.BlockBrace(() =>
					{
						g.Line("get => ", MemberName, ";");
						g.Line("set { _hasBeenSet", MemberName, " = true; ", MemberName, " = value; }");
					});

					g.Line("private bool _hasBeenSet", MemberName, ";");

					break;

				case GettableBaseType.Node:
					g.Line(export, "public NodePath ", ExportPropertyName, " { get; set; }");

					if (Default is { Length: >0 })
					{
						g.BlockTab(() =>
						{
							g.Line("= ", SyntaxFactory.Literal(Default).ToString(), ";");
						});
					}

					break;
			}
		}

		public override bool WritesConstructorStatements => BaseType == GettableBaseType.Resource;

		public override void WriteConstructorStatement(SourceStringBuilder g)
		{
			switch (BaseType)
			{
				case GettableBaseType.Resource:
					g.Line("if (Engine.EditorHint)");
					g.BlockBrace(() =>
					{
						WriteAssignment(g);
					});
					break;
			}
		}

		public override bool WritesOnReadyStatements => true;

		public override void WriteOnReadyStatement(SourceStringBuilder g)
		{
			switch (BaseType)
			{
				case GettableBaseType.Resource:
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
							MemberName, " is null, but OnReadyLoad not OrNull=true. (Default = '",
							Default ?? "null", "') ",
							"(Node = '{Name}' '{this}')\");");
					}

					break;

				case GettableBaseType.Node:
					if (OrNull)
					{
						g.Line("if (", ExportPropertyName, " != null)");
						g.BlockBrace(() => WriteGetNodeLine(g));
					}
					else
					{
						WriteGetNodeLine(g);
					}

					break;
			}
		}

		private void WriteGetNodeLine(SourceStringBuilder g)
		{
			g.Line(MemberName, " = GetNode" +
				(OrNull ? "OrNull" : "") +
				"<", MemberType, ">" +
				"(", ExportPropertyName, ");");
		}

		private void WriteAssignment(SourceStringBuilder g)
		{
			if (Default is not { Length: >0 }) return;

			g.Line(MemberName, " = GD.Load",
				"<", MemberType, ">",
				"(", SyntaxFactory.Literal(Default).ToString(), ");");
		}

		private static string GetSuffixlessExportPropertyNameForMemberName(string fieldName)
		{
			// Handle field name convention: _ prefix with lowercase name.
			string pathName = fieldName.TrimStart('_');
			pathName =
				pathName[0].ToString().ToUpperInvariant() +
				pathName.Substring(1);

			return pathName;
		}

		private string ExportPropertyName => SuffixlessExportPropertyName + BaseType switch
		{
			GettableBaseType.Resource => "Resource",
			GettableBaseType.Node => "Path",
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}
