using Godot;
using GodotOnReady.Attributes;
using GodotOnReadyDev;

public partial class RandomNumberLabel : Label
{
	[InjectAncestorValue(typeof(MyGui), "Gui")] private NumberService _ns;

	[OnReady] private void SetText()
	{
		Text = $"Random number: {_ns.Random()}";
	}
}
