using Godot;
using System;

public partial class View : Control
{
	PlayerCar playerCar;
	public void SetPlayer(PlayerCar car){
		playerCar = car;
		car.SetView(this);
		GetNode<Minimap>("UI/Minimap").ParentCar = car;
	}
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
