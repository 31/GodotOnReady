using System;

namespace GodotOnReady.Attributes
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class OnReadyAttribute : Attribute
	{
		public int Order { get; set; }
	}
}
