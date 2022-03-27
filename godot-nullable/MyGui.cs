using Godot;
using GodotOnReady.Attributes;

public partial class MyGui : Control
{
	[OnReadyGet] private Button? _b;
	[OnReadyGet] private CheckBox _c = null!;

	private float _t;

	public override void _Process(float delta)
	{
		base._Process(delta);

		_t += delta;
		if (_b != null)
		{
			_b.Text = $"Time: {_t:0.000}";
		}

		_c.Pressed = _t % 1 > 0.5;
	}
}
