using Godot;
using GodotOnReady.Attributes;

namespace GodotOnReadyDev
{
	public partial class ReadmeEx : Node
	{
		[OnReadyGet(OrNull = true)] private Button _button;

		[OnReady] private void ConnectButtonOnReady()
		{
			_button?.Connect("pressed", this, nameof(ButtonPressed));
		}

		public void ButtonPressed()
		{
			GD.Print("Button was pressed!");
		}
	}
}
