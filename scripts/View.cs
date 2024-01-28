using Godot;
using System;

public partial class View : Control
{
	public Control GetUI(){
		return GetNode<Control>("UI");
	}
	public Camera2D GetCamera(){
		return GetNode<Camera2D>("SubViewportContainer/SubViewport/Camera2D");
	}
	public SubViewport GetSubViewport(){
		return GetNode<SubViewport>("SubViewportContainer/SubViewport");
	}
	public void AddGame(Game game){
		GetSubViewport().AddChild(game);
	}
}
