using Microsoft.CodeAnalysis;

namespace GodotOnReady.Generator
{
	public record MemberSymbol(ITypeSymbol Type, string Name, ISymbol Symbol)
	{
		public static MemberSymbol Create(IPropertySymbol member) => new(
			member.Type, member.Name, member);

		public static MemberSymbol Create(IFieldSymbol member) => new(
			member.Type, member.Name, member);
	}
}
