using Godot;
using GodotOnReady.Attributes;

public partial class Demo : Node
{
	[OnReady(Order = 5)] public void Order5DefinedEarly() { }

	[OnReadyGet] public Node DefaultOnReadyGetProperty { get; set; }
	[OnReadyGet] public Node _defaultOnReadyGetField;
	[OnReadyGet] public Node DefaultOnReadyGetFieldAbnormalConvention;

	[OnReadyGet] public PackedScene DefaultOnReadyGetPropertyPackedScene { get; set; }
	[OnReadyGet] public PackedScene _defaultOnReadyGetFieldPackedScene;
	[OnReadyGet] public PackedScene DefaultOnReadyGetFieldAbnormalConventionPackedScene;

	[OnReady] public void Order0DefinedInTheMiddle() { }

	[OnReadyGet("Example")] public Node _exampleDefault;
	[OnReadyGet("Example", OrNull = true)] public Node _exampleDefaultOrNullTrue;
	[OnReadyGet("Example", Private = true)] public Node _exampleDefaultPrivateTrue;
	[OnReadyGet("Example", OrNull = true, Private = true)] public Node _exampleDefaultOrNullPrivateTrue;

	[OnReadyGet("res://tex.png")] public PackedScene _exampleDefaultPackedScene;
	[OnReadyGet("res://tex.png", OrNull = true)] public PackedScene _exampleDefaultOrNullTruePackedScene;
	[OnReadyGet("res://tex.png", Private = true)] public PackedScene _exampleDefaultPrivateTruePackedScene;
	[OnReadyGet("res://tex.png", OrNull = true, Private = true)] public PackedScene _exampleDefaultOrNullPrivateTruePackedScene;

	[OnReady(Order = -5)] public void OrderMinus5DefinedEarly() { }
}
