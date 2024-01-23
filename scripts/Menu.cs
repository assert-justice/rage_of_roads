using Godot;
using System;

public partial class Menu : Control
{
	[Export]
	public string name = "[default name]"; 
	Control focusTarget;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var childNode in GetChildren())
		{
			if(childNode is Control child && child.FocusMode == FocusModeEnum.All){
				focusTarget = child;
				break;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void Focus(){
		focusTarget.GrabFocus();
	}
}
