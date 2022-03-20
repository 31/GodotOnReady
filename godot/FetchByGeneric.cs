using Godot;
using GodotOnReady.Attributes;

public partial class FetchByGeneric<T> : Node where T : class
{
	[OnReadyGet] public T F { get; set; }
}

public partial class FetchByGenericLabel : FetchByGeneric<Label>
{
	[OnReady] private void TalkAboutIt() => GD.Print("Label text is:", F.Text);
}

public partial class FetchByGenericShout : FetchByGeneric<IShout>
{
	[OnReady] private void ShoutIt() => F.Shout();
}
