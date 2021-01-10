using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyPathAddition : PartialClassAddition
	{
		public string? MemberType { get; set; }
		public string? MemberName { get; set; }
		public string? MemberPathName { get; set; }

		public string? Default { get; set; }

		public bool OrNull { get; set; }

		public OnReadyPathAddition(AttributeData attribute)
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

			g.Line("[Export] public NodePath ", MemberPathNameOrDefault, " { get; set; }");

			if (Default is { Length: >0 })
			{
				g.BlockTab(() =>
				{
					g.Line("= ", SyntaxFactory.Literal(Default).ToString(), ";");
				});
			}
		}

		public override void WriteOnReadyStatement(SourceStringBuilder g)
		{
			if (MemberType is null) return;
			if (MemberName is null) return;

			g.Line(MemberName, " = GetNode" +
				(OrNull ? "OrNull" : "") +
				"<", MemberType, ">" +
				"(", MemberPathNameOrDefault, ");");
		}

		private string MemberPathNameOrDefault => MemberPathName ?? (MemberName ?? "") + "Path";
	}
}
