using System;

namespace GodotOnReady.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class OnReadyPathAttribute : Attribute
	{
		public string Default { get; set; }

		public bool OrNull { get; set; }

		public OnReadyPathAttribute(string @default = "")
		{
			Default = @default;
		}
	}
}
