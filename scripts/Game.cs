using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	[Export]
	public Resource aiScript;
	[Export]
	public Resource[] controlResources;
	public List<PlayerCar> GetPlayerCars(int playerCount){
		var cars = new List<PlayerCar>
		{
			GetNode<PlayerCar>("Player"),
			GetNode<PlayerCar>("Player2"),
			GetNode<PlayerCar>("Player3"),
			GetNode<PlayerCar>("Player4"),
		};
		var res = new List<PlayerCar>();
		for (int idx = 0; idx < 4; idx++)
		{
			if(idx < playerCount){
				res.Add(cars[idx]);
				cars[idx].Controls = controlResources[idx];
			}
			else{
				cars[idx].SetScript(aiScript);
			}
			// cars[idx].SpriteId = idx;
		}
		return res;
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
