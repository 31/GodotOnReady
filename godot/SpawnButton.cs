using Godot;
using GodotOnReady.Attributes;

public partial class SpawnButton : Button
{
	[OnReadyLoad("res://Subgui.tscn")] public PackedScene _scene;

	public void OnPress()
	{
		GetParent().AddChild(_scene.Instance());
	}
}
