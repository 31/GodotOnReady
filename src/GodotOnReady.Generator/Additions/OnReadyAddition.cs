using Microsoft.CodeAnalysis;
using System;

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

		public override Action<SourceStringBuilder>? OnReadyStatementWriter => g =>
		{
			g.Line();
			g.Line(Method.Name, "();");
		};
	}
}
