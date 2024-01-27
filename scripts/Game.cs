using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	public List<PlayerCar> GetPlayerCars(int playerCount){
		var cars = new List<PlayerCar>
		{
			GetNode<PlayerCar>("Player"),
			GetNode<PlayerCar>("Player2"),
			GetNode<PlayerCar>("Player3"),
			GetNode<PlayerCar>("Player4"),
		};
		while(cars.Count > playerCount) cars.RemoveAt(cars.Count-1);
		return cars;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
