using System;

namespace GodotOnReady.Attributes
{
	/// <summary>
	/// Calls the applied 0-argument method during the generated '_Ready' method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class OnReadyAttribute : Attribute
	{
		/// <summary>
		/// Sets the relative order of this method vs. others with this attribute. When deciding
		/// which order to call OnReady methods, the generator first sorts by Order, then by
		/// declaration order. All OnReadyGet initialization happens after the last '-1' Order
		/// OnReady and before the first '0' Order OnReady.
		/// </summary>
		public int Order { get; set; }
	}
}
