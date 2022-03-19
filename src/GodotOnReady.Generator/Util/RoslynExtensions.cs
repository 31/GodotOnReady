using Microsoft.CodeAnalysis;
using System.Linq;

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

		public static bool IsOfBaseType(this ITypeSymbol? type, ITypeSymbol baseType)
		{
			if (type is ITypeParameterSymbol p)
			{
				return p.ConstraintTypes.Any(ct => ct.IsOfBaseType(baseType));
			}

			while (type != null)
			{
				if (SymbolEqualityComparer.Default.Equals(type, baseType))
				{
					return true;
				}

				type = type.BaseType;
			}

			return false;
		}

		public static bool IsInterface(this ITypeSymbol? type)
		{
			if (type is ITypeParameterSymbol p)
			{
				return p.ConstraintTypes.Any(ct => ct.TypeKind == TypeKind.Interface);
			}
			return type?.TypeKind == TypeKind.Interface;
		}
	}
}
