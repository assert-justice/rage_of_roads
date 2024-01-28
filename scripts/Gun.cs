
using Godot;

public partial class Gun : Node2D{
	[Export]
	float Cooldown = 0.1f;
	[Export]
	PackedScene BulletScene;
	[Export] 
	PackedScene FuckBulletScene;
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
	public bool Fire(){
		if(!canFire) return false;
		if(BulletScene is null) return false;
		canFire = false;
		fireClock = Cooldown;
		var bullet = BulletScene.Instantiate<Bullet>();
		bullet.Position = GlobalPosition + GlobalTransform.BasisXform(BulletSpawnOffset);
		// bullet.Rotation = GlobalRotation;
		parent.GetParent().AddChild(bullet);
		bullet.Velocity = GlobalTransform.BasisXform(Vector2.Right) * BulletSpeed + parent.Velocity;
		return true;
	}

	public bool FireFuckBomb()
	{
		if(FuckBulletScene is null) return false;
		GD.Print("Firing fuck");
		canFire = false;
		fireClock = Cooldown;
		var bullet = FuckBulletScene.Instantiate<FuckBullet>();
		bullet.Position = GlobalPosition + GlobalTransform.BasisXform(BulletSpawnOffset);
		parent.GetParent().AddChild(bullet);
		bullet.Velocity = GlobalTransform.BasisXform(Vector2.Right) * BulletSpeed + parent.Velocity;
		return true;
	}
}
