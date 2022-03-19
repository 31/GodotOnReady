using Godot;
using GodotOnReady.Attributes;

public partial class FetchByGenericInterface<T> : Node where T : class, IShout
{
	[OnReadyGet] public T F { get; set; }
}
