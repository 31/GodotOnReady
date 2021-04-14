using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis;
using System;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyGetNodeCacheAddition : PartialClassAddition
	{
		private INamedTypeSymbol CacheSymbol { get; set; }

		public OnReadyGetNodeCacheAddition(INamedTypeSymbol cacheSymbol, INamedTypeSymbol @class) : base(@class)
		{
			CacheSymbol = cacheSymbol;
			Order = 0;
		}

		public override Action<SourceStringBuilder>? OnReadyStatementWriter => g =>
		{
			g.Line();
			g.Line("var nodeCache = new ", CacheSymbol.ToFullDisplayString(), "();");
		};
	}
}
