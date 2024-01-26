using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export]
	public Vector2 Velocity = new Vector2();
	[Export]
	public float DamageValue = 5;
	public override void _Process(double delta)
	{
		base._Process(delta);
		Position += Velocity * (float)delta;
	}
	
	private void _on_body_entered(Node2D body)
	{
		if(body is Car car){
			car.Damage(DamageValue);
		}
		else if(body is Turret turret){
			turret.Damage(DamageValue);
		}
	}
}
