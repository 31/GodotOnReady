using Godot;

public class NodeImplementingIShout : Node, IShout
{
	public void Shout()
	{
		GD.Print("I AM VERY HAPPY.");
	}
}
