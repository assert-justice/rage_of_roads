using Godot;
using System;
using System.Collections.Generic;

public partial class Powerup : Area2D
{
	List<Sprite2D> icons = new List<Sprite2D>();
	[Export]
	PowerupType type = PowerupType.Energy;
	public override void _Ready()
	{
		foreach (var node in GetChildren())
		{
			if(node is Sprite2D sprite) icons.Add(sprite);
		}
		SetType(type);
	}
	public void SetType(PowerupType powerupType){
		type = powerupType;
		foreach (var sprite in icons)
		{
			sprite.Visible = false;
		}
		icons[(int)type].Visible = true;
	}
	private void _on_body_entered(Node2D body)
	{
		if(body is Car car){
			if(car.AddPowerup(type)) QueueFree();
		}
	}
}
