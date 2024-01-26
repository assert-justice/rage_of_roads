using System;
using Godot;

public partial class Car : CharacterBody2D
{
	[ExportGroup("Movement")]
	[Export]
	float AccelPower = 500;
	[Export]
	float BreakPower = 800;
	[Export]
	float EBreakPower = 800;
	[Export]
	float MaxSpeed = 1600;
	[Export]
	float MinSpeed = 1000;
	[Export]
	float TurnSpeed = 2;
	[Export]
	float Drag = 100;
	[Export]
	float SkidSpeed = 600;

	[ExportGroup("Resources")]
	// resources
	[Export]
	public int Lives = 3;
	[Export]
	float RageRegen = 10;
	[Export]
	float AmmoRegen = 30;
	[Export]
	float FireCost = 5;
	public float ammo = 100;
	public float health = 100;
	public float rage = 0;
	
	// movement and input
	Vector2 startPosition;
	protected float speed = 0;
	protected float turn = 0;
	protected float gasPedal = 0;
	protected float breakPedal = 0;
	protected bool eBrake = false;
	protected bool skidding = false;
	protected float angularVelocity;
	protected bool fireLeft = false;
	protected bool fireRight = false;
	protected bool fireAlt = false;
	float fireLeftClock = 0;
	float fireRightClock = 0;
	// audio
	AudioStreamPlayer2D engineSoundPlayer;
	AudioStreamPlayer2D tireSoundPlayer;
	// guns
	Gun leftGun;
	Gun rightGun;

	public override void _Ready()
	{
		engineSoundPlayer = GetNode<AudioStreamPlayer2D>("Sfx/EngineSound");
		tireSoundPlayer = GetNode<AudioStreamPlayer2D>("Sfx/TireSound");
		leftGun = GetNode<Gun>("LeftGun");
		rightGun = GetNode<Gun>("RightGun");
		AddToGroup("Car");
		startPosition = Position;
	}
	public override void _Process(double delta)
	{
		float dt = (float) delta;
		HandleInput(dt);
		Move(dt);
		HandleAudio();
		rage += RageRegen * dt; if(rage > 100) rage = 100;
		ammo += AmmoRegen * dt; if(ammo > 100) ammo = 100;
		Fire();
	}
	public void Damage(float value){
		// health -= value;
		if(health < 0){
			Lives -= 1;
			Position = startPosition;
			health = 100;
		}
	}

	virtual protected void HandleInput(float dt){
		// speed = 0;
		turn = 0;
		gasPedal = 0;
		breakPedal = 0;
		eBrake = false;
	}
	void ClampSpeed(){
		// var min = eBrake ? 0 : -MinSpeed;
		speed = Mathf.Clamp(speed, -MinSpeed, MaxSpeed);
	}
	void HandleAudio(){
		if(skidding && !tireSoundPlayer.Playing) tireSoundPlayer.Play();
		else if(!skidding && tireSoundPlayer.Playing) tireSoundPlayer.Stop();
			engineSoundPlayer.PitchScale = 1 + Velocity.Length() / MaxSpeed;
		// if(GetSpeed() > 0){
		// 	if(!engineSoundPlayer.Playing) engineSoundPlayer.Play();
		// }
	}
	public float GetSpeed(){
		return Transform.BasisXformInv(Velocity).X;
	}

	void Move(float dt){
		bool didCollide;
		if(skidding){
			// maintain but damp velocity and angular velocity
			var vMag = Velocity.Length();
			float vDamp = 400;
			if(!eBrake || vMag < vDamp){
				skidding = false;
				speed = Transform.BasisXformInv(Velocity).X;
				// handle kick
				if(gasPedal > 0) speed += 400;
				else if(breakPedal > 0) speed -= 400;
				// GD.Print(speed);
				return;
			}
			vMag -= vDamp * dt;
			Velocity = Velocity.Normalized() * vMag;
			// speed = Velocity.Length();
			// if(speed > vDamp) speed -= vDamp * dt;
			// else if(speed < -vDamp) speed += vDamp * dt;
			// else{skidding = false;}
			// Velocity = Transform.BasisXform(Vector2.Right * speed);
			Rotate(angularVelocity * dt);
			didCollide = MoveAndSlide();
			if(didCollide){
				speed = Velocity.Length();
				speed = Transform.BasisXformInv(Velocity).X;
			}
			// if(!eBrake) skidding = false;
			return;
		}
		float accel = gasPedal * AccelPower - breakPedal * BreakPower;
		if(gasPedal == 0 && speed > 0) accel -= Drag;
		// if(eBrake) accel -= EBreakPower;
		if(eBrake){
			accel = accel > 0 ? accel - EBreakPower : accel + EBreakPower;
		}
		if(eBrake && Math.Abs(speed) > SkidSpeed && Math.Abs(turn) > 0.7) {
			skidding = true;
			return;
			// place skid marks
		}
		// split acceleration in two for time step reasons
		speed += accel * dt * 0.5f;
		ClampSpeed();
		angularVelocity = turn * TurnSpeed * speed / MaxSpeed;
		Rotate(angularVelocity * dt);
		Velocity = Transform.BasisXform(Vector2.Right * speed);
		didCollide = MoveAndSlide();
		if(didCollide){
			speed = Velocity.Length();
		}
		speed += accel * dt * 0.5f;
		ClampSpeed();
	}

	void Fire(){
		if(fireLeft && ammo > 0) {
			if(leftGun.Fire()) ammo -= FireCost;
			if(ammo < 0) ammo = -30;
		}
		if(fireRight && ammo > 0) {
			if(rightGun.Fire()) ammo -= FireCost;
			if(ammo < 0) ammo = -30;
		}
	}
}

