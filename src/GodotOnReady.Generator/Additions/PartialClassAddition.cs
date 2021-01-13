using Microsoft.CodeAnalysis;

namespace GodotOnReady.Generator.Additions
{
	public abstract class PartialClassAddition
	{
		protected PartialClassAddition(INamedTypeSymbol @class)
		{
			Class = @class;
		}

		public INamedTypeSymbol Class { get; }

		public int Order { get; set; }

		public virtual void WriteDeclaration(SourceStringBuilder g) { }

		public virtual bool WritesConstructorStatements => false;
		public virtual void WriteConstructorStatement(SourceStringBuilder g) { }

		public virtual bool WritesOnReadyStatements => false;
		public virtual void WriteOnReadyStatement(SourceStringBuilder g) { }
	}
}
