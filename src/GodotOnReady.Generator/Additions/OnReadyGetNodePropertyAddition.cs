using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis.CSharp;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyGetNodePropertyAddition : OnReadyGetNodeAddition
	{
		public OnReadyGetNodePropertyAddition(MemberAttributeSite site) : base(site) { }

		protected override void WriteGetMemberBlock(SourceStringBuilder g)
		{
			if (Property is { Length: >0 } property)
			{
				g.Line(Member.Name, " = " +
					"(", Member.Type.ToFullDisplayString(), ") " +
					"GetNode", (OrNull ? "OrNull" : "") +
					"(", ExportPropertyName, ")?.Get(" +
					SyntaxFactory.Literal(property).ToString(), ");");
			}
		}
	}
}
