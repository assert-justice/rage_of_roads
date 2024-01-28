using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public partial class Game : Node2D
{
	[Export]
	public Resource aiScript;
	[Export]
	public Resource[] controlResources;
	List<Node2D> spawners = new List<Node2D>();
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
	public Node2D GetSpawn(){
		var cars = GetTree().GetNodesInGroup("Player").Select(c => c as Node2D);
		Node2D res = spawners[0];
		float bestDistance = 0;
		foreach (var spawner in spawners)
		{
			var dist = cars.Select(p => (p.Position - spawner.Position).Length()).Max();
			if(dist > bestDistance){
				bestDistance = dist;
				res = spawner;
			}
		}
		return res;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var item in GetTree().GetNodesInGroup("Car"))
		{
			if(item is Car car){
				car.SetGame(this);
			}
		}
		foreach (var item in GetTree().GetNodesInGroup("PlayerSpawner"))
		{
			if(item is Node2D node){
				spawners.Add(node);
				node.Visible = false;
				// node.vis
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
