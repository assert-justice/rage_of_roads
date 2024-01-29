using Godot;
using System;

public partial class FuckBullet : Area2D
{
	[Export]
	public Vector2 Velocity = new Vector2();
	[Export]
	public float DamageValue = 100;
	[Export] public PackedScene YouParticles;

	public override void _Ready()
	{
		base._Ready();
	}
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
		var youParticles = YouParticles.Instantiate();
		GetParent().AddChild(youParticles);
		// youParticles.Position = Position;
		// youParticles.Emitting = true;
		// var instance = YouParticles.Instantiate<GpuParticles2D>();
		QueueFree();	
	}
}
