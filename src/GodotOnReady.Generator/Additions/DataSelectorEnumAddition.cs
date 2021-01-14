using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis;
using System;

namespace GodotOnReady.Generator.Additions
{
	public class DataSelectorEnumAddition : PartialClassAddition
	{
		public IFieldSymbol[] Fields { get; }
		public string EnumName { get; }

		public DataSelectorEnumAddition(
			IFieldSymbol[] fields,
			AttributeSite site)
			: base(site.Class)
		{
			Fields = fields;

			EnumName = "ATTRIBUTE_NAME_ARGUMENT_NOT_FOUND";
			foreach (var constructorArg in site.Attribute.ConstructorArguments)
			{
				if (constructorArg.Value is string name)
				{
					EnumName = name;
				}
			}
		}

		public override Action<SourceStringBuilder>? DeclarationWriter => g =>
		{
			g.Line("public static ", Class.Name, " Get(", EnumName, " key)");
			g.BlockBrace(() =>
			{
				g.Line("switch (key)");
				g.BlockBrace(() =>
				{
					foreach (var field in Fields)
					{
						g.Line("case ", EnumName, ".", field.Name, ": return ", field.Name, ";");
					}
				});
				g.Line("throw new ArgumentOutOfRangeException(\"key\");");
			});
		};

		public override Action<SourceStringBuilder>? OutsideClassStatementWriter => g =>
		{
			g.Line();
			g.Line("public enum ", EnumName);
			g.BlockBrace(() =>
			{
				foreach (var field in Fields)
				{
					g.Line(field.Name, ",");
				}
			});

			g.Line("public static class ", EnumName, "Extensions");
			g.BlockBrace(() =>
			{
				g.Line(
					"public static ", Class.Name,
					" GetData(this ", EnumName, " v) => ", Class.Name, ".Get(v);");
			});
		};
	}
}
