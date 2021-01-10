using Godot;
using GodotOnReady.Attributes;

public partial class MyControl : Control
{
	[OnReadyPath] private Button _button;
	[OnReadyPath] private Control _target;

	[OnReady]
	public void RunOnReady()
	{
		GD.Print("RunOnReady called!");

		_button.Connect("pressed", this, nameof(Button_pressed));
	}

	public void Button_pressed()
	{
		_target.MarginRight += 5f;
		_target.MarginLeft += 5f;
	}
}
