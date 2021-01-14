using System;

namespace GodotOnReady.Attributes
{
	/// <summary>
	/// Generates code to initialize this property or field when the node is ready, and make the
	/// initialization path configurable in the Godot editor. This attribute works on properties and
	/// fields of types that subclass either Node or Resource.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class OnReadyGetAttribute : Attribute
	{
		public string Default { get; }

		/// <summary>
		/// Allows nulls and unexpected node types in the ready method without throwing exceptions.
		/// Effectively makes the OnReadyGet optional.
		/// </summary>
		public bool OrNull { get; set; }

		/// <summary>
		/// Prevents exporting a generated property. This disables configuration from the Godot
		/// scene. This can be used to hide properties that should not be configurable, while
		/// continuing to automatically set the value in the generated ready method.
		/// </summary>
		public bool Private { get; set; }

		/// <param name="default">
		/// The default initialization path that will be loaded when the node is ready unless
		/// configured otherwise in the Godot editor. The reset button in the Godot editor restores
		/// this value.
		/// </param>
		public OnReadyGetAttribute(string @default = "")
		{
			Default = @default;
		}
	}
}
