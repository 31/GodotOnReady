using Microsoft.CodeAnalysis;

namespace GodotOnReady.Generator.Additions
{
	public abstract class PartialClassAddition
	{
		public INamedTypeSymbol? Class { get; set; }

		public int Order { get; set; }

		public virtual void WriteDeclaration(SourceStringBuilder g) { }
		public virtual void WriteOnReadyStatement(SourceStringBuilder g) { }
	}
}
