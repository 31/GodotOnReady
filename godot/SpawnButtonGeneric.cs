using GodotOnReady.Attributes;

public partial class SpawnButtonGeneric<T> : SpawnButton
{
	[OnReady] private void SetupText()
	{
		Text = $"My type is {typeof(T)}!";
	}
}
