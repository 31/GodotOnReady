using Godot;
using GodotOnReady.Attributes;

public partial class SpawnButton : Button
{
	[OnReadyGet("res://Subgui.tscn", Private = true)] public PackedScene _scene;

	public void OnPress()
	{
		GetParent().AddChild(_scene.Instance());
	}
}
