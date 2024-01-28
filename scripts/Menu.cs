using Godot;
using System;

public partial class Menu : Control
{
	[Export]
	public string name = "[default name]"; 
	public void Focus(){
		foreach (var childNode in GetChildren())
		{
			if(childNode is Control child && child.FocusMode == FocusModeEnum.All){
				child.GrabFocus();
				return;
			}
		}
	}
}
