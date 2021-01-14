using Microsoft.CodeAnalysis;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyAddition : PartialClassAddition
	{
		public IMethodSymbol Method { get; }

		public OnReadyAddition(
			IMethodSymbol method,
			AttributeData attribute,
			INamedTypeSymbol @class)
			: base(@class)
		{
			Method = method;

			foreach (var namedArg in attribute.NamedArguments)
			{
				if (namedArg.Key == "Order" && namedArg.Value.Value is int i)
				{
					Order = i;
				}
			}
		}

		public override bool WritesOnReadyStatements => true;

		public override void WriteOnReadyStatement(SourceStringBuilder g)
		{
			g.Line();
			g.Line(Method.Name, "();");
		}
	}
}
