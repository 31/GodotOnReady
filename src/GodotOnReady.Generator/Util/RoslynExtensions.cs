using Microsoft.CodeAnalysis;

namespace GodotOnReady.Generator.Util
{
	internal static class RoslynExtensions
	{
		public static string ToFullDisplayString(this ISymbol s)
		{
			return s.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		}

		public static string? GetSymbolNamespaceName(this INamedTypeSymbol iTypeNode)
		{
			string? namespaceName = null;
			INamespaceSymbol? ns = iTypeNode.ContainingNamespace;

			while (ns?.ContainingNamespace is { } containing)
			{
				if (namespaceName == null)
				{
					namespaceName = ns.Name;
				}
				else
				{
					namespaceName = ns.Name + "." + namespaceName;
				}

				ns = containing;
			}

			return namespaceName;
		}
	}
}
