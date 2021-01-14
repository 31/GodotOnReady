using System;

namespace GodotOnReady.Attributes
{
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
