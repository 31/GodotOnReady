using Godot;
using GodotOnReady.Attributes;

public partial class FetchByGenericButton : FetchByGeneric<Button>
{
	[OnReady] private void TalkAboutTheButton() => GD.Print("Button text is:", F.Text);
}
