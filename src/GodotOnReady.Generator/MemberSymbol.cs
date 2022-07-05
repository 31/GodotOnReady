using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace GodotOnReady.Generator
{
	public record MemberSymbol(ITypeSymbol Type, string Name, ISymbol Symbol)
	{
		public static MemberSymbol Create(IPropertySymbol member) => new(
			member.Type, member.Name, member);

		public static MemberSymbol Create(IFieldSymbol member) => new(
			member.Type, member.Name, member);

		public static IEnumerable<MemberSymbol> CreateAll(ITypeSymbol type) =>
			type.GetMembers().OfType<IPropertySymbol>().Select(Create).Concat(
				type.GetMembers().OfType<IFieldSymbol>().Select(Create));
	}
}
