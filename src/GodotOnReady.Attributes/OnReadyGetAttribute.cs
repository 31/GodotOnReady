using System;

namespace GodotOnReady.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class OnReadyGetAttribute : Attribute
	{
		public string Default { get; set; }

		public bool OrNull { get; set; }

		public OnReadyGetAttribute(string @default = "")
		{
			Default = @default;
		}
	}
}
