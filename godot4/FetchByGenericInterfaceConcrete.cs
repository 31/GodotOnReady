using Godot;
using GodotOnReady.Attributes;

public partial class FetchByGenericInterfaceConcrete : FetchByGenericInterface<NodeImplementingIShout>
{
	[OnReady] private void TalkAboutTheButton()
	{
		GD.Print("This shout implementer fetched by generic interface:");
		F.Shout();
	}
}
