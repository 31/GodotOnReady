using Godot;
using GodotOnReady.Attributes;

public partial class OnReadyFindDemo : Node
{
    [OnReadyFind("TestNode")] public Control _someNode;
    [OnReadyFind("TestNode", Owned = true)] public Control _someNode2;
    [OnReadyFind("TestNode", Owned = true, Recursive = true)] public Control _someNode3;
    [OnReadyFind("TestNode", Owned = false, Recursive = true)] public Control _someNode4;
    [OnReadyFind("TestNode", Owned = false, Recursive = false)] public Control _someNode5;

    public void Test()
    {
        FindNode("test", false, false);
    }
}
