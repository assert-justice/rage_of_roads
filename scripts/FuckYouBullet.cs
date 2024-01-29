using Godot;
using System;

public partial class FuckYouBullet : Bullet
{
	[Export] float Fuse = 1;
	public override void _Process(double delta)
	{
		base._Process(delta);
		if(Fuse > 0){
			Fuse -= (float)delta;
			if(Fuse <= 0) Detonate();
		}
	}
	void Detonate(){
		Fuse = 0;
		Velocity = Vector2.Zero;
		// GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", false);
		GetNode<GpuParticles2D>("GPUParticles2D").Emitting = true;
		GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Playing = true;
	}
	private void _on_gpu_particles_2d_finished()
	{
		QueueFree();
	}


	private void _on_body_entered(Node2D body)
	{
		if(body is Car car){car.Damage(DamageValue);}
	}
	private void _on_area_2d_body_entered(Node2D body)
	{
		Detonate();
	}
}

