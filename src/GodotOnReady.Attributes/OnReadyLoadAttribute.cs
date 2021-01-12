using System;

namespace GodotOnReady.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class OnReadyLoadAttribute : Attribute
	{
		public string Default { get; set; }

		public bool OrNull { get; set; }

		public OnReadyLoadAttribute(string @default = "")
		{
			Default = @default;
		}
	}
}
