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
	[OnReadyGet("Example", Export = true)] public Node _exampleDefaultExportTrue;
	[OnReadyGet("Example", OrNull = true, Export = true)] public Node _exampleDefaultOrNullExportTrue;

	[OnReadyGet("res://tex.png")] public PackedScene _exampleDefaultPackedScene;
	[OnReadyGet("res://tex.png", OrNull = true)] public PackedScene _exampleDefaultOrNullTruePackedScene;
	[OnReadyGet("res://tex.png", Export = true)] public PackedScene _exampleDefaultExportTruePackedScene;
	[OnReadyGet("res://tex.png", OrNull = true, Export = true)] public PackedScene _exampleDefaultOrNullExportTruePackedScene;

	[OnReady(Order = -5)] public void OrderMinus5DefinedEarly() { }
}
