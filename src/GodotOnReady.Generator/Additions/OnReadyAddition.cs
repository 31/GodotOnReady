using Microsoft.CodeAnalysis;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyAddition : PartialClassAddition
	{
		public IMethodSymbol? Method { get; set; }

		public OnReadyAddition(AttributeData attribute)
		{
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
			if (Method is null) return;

			g.Line(Method.Name, "();");
		}
	}
}
