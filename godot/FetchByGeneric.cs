using Godot;
using GodotOnReady.Attributes;

public partial class FetchByGeneric<T> : Node where T : Node
{
	[OnReadyGet] public T F { get; set; }
}
