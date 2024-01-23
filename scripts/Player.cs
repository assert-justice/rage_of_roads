using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	float AccelPower = 30;
	[Export]
	float BreakPower = 30;
	[Export]
	float MaxSpeed = 300;
	public override void _Ready()
	{
		//
	}
	public override void _Process(double delta)
	{
		float v = 0;
		if(Input.IsActionPressed("accelerate")){
			v = MaxSpeed;
		}
		v *= (float) delta;
		Velocity = new Vector2(v, 0);
		MoveAndCollide(Velocity);
	}
}
