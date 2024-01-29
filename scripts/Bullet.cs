using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export]
	public Vector2 Velocity = new Vector2();
	[Export]
	public float DamageValue = 5;
	[Export]
	public AudioStream FireSfx;
	[Export]
	public AudioStream CensoredSfx;
	public override void _Ready()
	{
		base._Ready();
		// is censored
		var audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (GetTree().Root.GetChild<MenuManager>(0).Censored){
			audioPlayer.Stream = CensoredSfx;
			sprite.Animation = "censored";
		}
		else{
			audioPlayer.Stream = FireSfx;
		}
		audioPlayer.Play();
		sprite.Play();
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
		QueueFree();
	}
}
