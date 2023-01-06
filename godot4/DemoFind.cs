using Godot;
using GodotOnReady.Attributes;

public partial class DemoFind : Node
{
	[OnReadyFind] public Control _nodeExportMask;
	[OnReadyFind("Somewhere")] public Control _nodeDefaults;
	[OnReadyFind("Somewhere", NonRecursive = true)] public Control _nodeNoRecursive;
	[OnReadyFind("Somewhere", Unowned = true)] public Control _nodeUnowned;
	[OnReadyFind("Somewhere", NonRecursive = true, Unowned = true)] public Control _nodeUnownedNonRecursive;
}
