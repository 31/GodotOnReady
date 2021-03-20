using Godot;
using GodotOnReady.Attributes;

public partial class SpawnButtonDerived : SpawnButton
{
	[OnReadyGet] public HBoxContainer _other { get; set; }

	[OnReady] public void R()
	{
		GD.Print($"{nameof(SpawnButtonDerived)} {this} is ready!");
	}

	public override void OnPress()
	{
		base.OnPress();
		_other.AddChild(_scene.Instance());
	}
}
