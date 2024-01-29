
using System;
using Godot;

public partial class Gun : Node2D{
	[Export]
	float Cooldown = 0.1f;
	// [Export]
	// PackedScene BulletScene;
	[Export] 
	PackedScene[] BulletScenes;
	[Export]
	float BulletSpeed = 1800;
	[Export]
	Vector2 BulletSpawnOffset = Vector2.Right*100;
	float fireClock = 0;
	bool canFire = true;
	CharacterBody2D parent;
	public override void _Ready()
	{
		parent = GetParent<CharacterBody2D>();
	}
	public override void _Process(double delta)
	{
		float dt = (float) delta;
		if(fireClock > 0){
			fireClock -= dt;
		}
		canFire = fireClock <= 0;
	}
	public bool Fire(bool increaseScale, float megaPhoneScale){
		if(!canFire) return false;
		// if(BulletScene is null) return false;
		int idx = (int)Math.Floor(GD.Randf() * BulletScenes.Length);
		var bulletScene = BulletScenes[idx];
		canFire = false;
		fireClock = Cooldown;
		var bullet = bulletScene.Instantiate<Bullet>();
		bullet.Position = GlobalPosition + GlobalTransform.BasisXform(BulletSpawnOffset);
		if (increaseScale) bullet.Scale = (bullet.Scale) * megaPhoneScale; 
		// bullet.Rotation = GlobalRotation;
		parent.GetParent().AddChild(bullet);
		bullet.Velocity = GlobalTransform.BasisXform(Vector2.Right) * BulletSpeed + parent.Velocity;
		return true;
	}

	// public bool FireFuckBomb()
	// {
	// 	if(!canFire) return false;
	// 	// if(FuckBulletScene is null) return false;
	// 	GD.Print("Firing fuck");
	// 	canFire = false;
	// 	fireClock = Cooldown;
	// 	// var bullet = FuckBulletScene.Instantiate<FuckBullet>();
	// 	bullet.Position = GlobalPosition + GlobalTransform.BasisXform(BulletSpawnOffset);
	// 	parent.GetParent().AddChild(bullet);
	// 	bullet.Velocity = GlobalTransform.BasisXform(Vector2.Right) * BulletSpeed + parent.Velocity;
	// 	return true;
	// }
}
