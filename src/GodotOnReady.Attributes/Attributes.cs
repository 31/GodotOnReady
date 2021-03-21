using System;

// All GodotOnReady attributes are in this single file to make it easy to copy directly into a game
// project and avoid a DLL dependency.

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
		public string Path { get; }

		/// <summary>
		/// If true, always generates a property with [Export], even if Path isn't set. The exported
		/// property can be used in the editor to change the Path.
		/// </summary>
		public bool Export { get; set; }

		/// <summary>
		/// Allows nulls and unexpected node types in the ready method without throwing exceptions.
		/// Effectively makes the OnReadyGet optional.
		/// </summary>
		public bool OrNull { get; set; }

		/// <param name="path">
		/// The path that will be loaded when the node is ready. If not set, a property is generated
		/// with [Export], so the path can be set in the Godot editor.
		/// </param>
		public OnReadyGetAttribute(string path = "")
		{
			Path = path;
		}
	}

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

	/// <summary>
	/// Generates an enum with one entry for each static readonly field in the marked class.
	/// Generates a lookup method on the marked class that returns the field based on the enum entry
	/// value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class GenerateDataSelectorEnumAttribute : Attribute
	{
		public string Name { get; }

		/// <param name="name">The name of the enum to generate.</param>
		public GenerateDataSelectorEnumAttribute(string name)
		{
			Name = name;
		}
	}
}
