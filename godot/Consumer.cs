using Godot;
using GodotOnReady.Attributes;

public partial class Consumer : Control
{
	[InjectAncestorValue] private Provider _provider;

	// Not gettable: matches both Provided2 and Provided4 fields.
	// [InjectAncestorValue(typeof(Provider))] private Provider.Provided2 _p2;

	// Even though Provider only has Provided4, it's a Provided3 so it still satisfies this.
	[InjectAncestorValue(typeof(Provider))] private Provider.Provided3 _p3;

	[InjectAncestorValue(typeof(Provider))] private Provider.Provided3 P3Prop { get; set; }

	[InjectAncestorValue(typeof(Provider), nameof(Provider))] private Provider.Provided3 _p3VerySpecific;

	[OnReady] private void Ready()
	{
		GD.Print("Whole provider: ", _provider);
		GD.Print("Provider p3: ", _p3, " as prop: ", P3Prop);
	}
}
