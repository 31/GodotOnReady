using GodotOnReady.Generator.Util;
using Microsoft.CodeAnalysis.CSharp;

namespace GodotOnReady.Generator.Additions
{
	public class OnReadyGetNodePropertyAddition : OnReadyGetNodeAddition
	{
		public OnReadyGetNodePropertyAddition(MemberAttributeSite site) : base(site) { }

		protected override void WriteGetMemberBlock(SourceStringBuilder g)
		{
			if (Property is { Length: > 0 })
			{
				g.Line("if (!nodeCache.ContainsKey(", ExportPropertyName, "))");
				g.BlockBrace(() =>
				{
					g.Line("var node = GetNodeOrNull" +
						"(", ExportPropertyName, ");");

					g.Line("nodeCache[", ExportPropertyName, "] = node;");
					g.Line(Member.Name, " = node?.Get(", 
						SyntaxFactory.Literal(Property).ToString(), ") as ", 
						Member.Type.ToFullDisplayString(), ";");
				});
				g.Line("else");
				g.BlockBrace(() =>
				{
					g.Line(Member.Name, " = nodeCache[", ExportPropertyName, "]?.Get(", 
						SyntaxFactory.Literal(Property).ToString(), ") as ", 
						Member.Type.ToFullDisplayString(), ";");
				});
			}
		}
	}
}
