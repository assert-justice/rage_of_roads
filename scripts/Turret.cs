using System.Collections.Generic;
using Godot;

public partial class Turret : CharacterBody2D
{
	[Export]
	float fireRadius = 1600;
	[Export]
	float Health = 50;
	HashSet<Car> targets = new HashSet<Car>();
	Car target;
	Gun gun;
	bool alive = true;
	public void Damage(float value){
		if(!alive) return;
		Health -= value;
		if(Health < 0){
			alive = false;
			gun.QueueFree();
		}
	}
	public override void _Ready()
	{
		base._Ready();
		foreach (var node in GetTree().GetNodesInGroup("Car"))
		{
			targets.Add(node as Car);
		}
		gun = GetNode<Gun>("Gun");
	}
	public override void _Process(double delta)
	{
		if(!alive) return;
		foreach (var car in targets)
		{
			if(target is null) target = car;
			else if((Position - target.Position).Length() < (Position - car.Position).Length()) target = car;
		}
		gun.Rotation = GetAngleTo(target.Position);
		if((target.Position - Position).Length() < fireRadius) gun.Fire(false, 1);
	}
}
