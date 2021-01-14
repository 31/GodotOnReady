using Godot;
using GodotOnReady.Attributes;
using GodotOnReadyDev;
using System;

public partial class MyGui : VBoxContainer
{
	[OnReadyGet(nameof(OptionalButton), OrNull = true)] public Button OptionalButton { get; set; }

	[OnReadyGet(OrNull = true)] public Button FullyOptionalButton { get; set; }

	[OnReadyGet("LineEdit")] public LineEdit AddLineBox { get; set; }

	[OnReady]
	public void InitializeInput()
	{
		AddLineBox.Text = $"Started up at {DateTime.UtcNow.ToLongDateString()}";
		AddLineBox.Connect("text_entered", this, nameof(LineEdit_text_entered));
		AddLineBox.GrabFocus();
	}

	[OnReadyGet] private Tree _myTree;
	private TreeItem _root;

	[OnReady]
	public void InitializeTreeRoot()
	{
		_root = _myTree.CreateItem();
		_myTree.HideRoot = true;
	}

	public void LineEdit_text_entered(string text)
	{
		AddLineBox.Text = string.Empty;

		TreeItem checkRoot = _root;

		var dirParts = text.Split("/");

		foreach (var dirPart in dirParts)
		{
			TreeItem nest = null;

			TreeItem check = checkRoot.GetChildren();
			while (check != null)
			{
				if (check.GetText(0) == dirPart)
				{
					nest = check;
				}

				check = check.GetNext();
			}

			if (nest is null)
			{
				nest = _myTree.CreateItem(checkRoot);
				nest.SetText(0, dirPart);
			}

			checkRoot = nest;
		}
	}

	[OnReady] public void TryEnum()
	{
		GD.Print(DemoEnum.A, ": ", DemoEnum.A.GetData().Extended);
		GD.Print(DemoEnum.B, ": ", DemoEnum.B.GetData().Extended);
	}
}
