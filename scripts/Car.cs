using System;
using Godot;

enum CarState{
	Normal,
	Drifting,
	Skidding,
}
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
	[Export]
	float SkidDamping = 400;

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
	CarState state = CarState.Normal;
	Vector2 startPosition;
	// protected float speed = 0;
	protected float turn = 0;
	protected float gasPedal = 0;
	protected float breakPedal = 0;
	protected bool eBrake = false;
	// protected bool skidding = false;
	protected float angularVelocity;
	protected bool fireLeft = false;
	protected bool fireRight = false;
	protected bool fireAlt = false;
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
	bool isSliding(){
		return state == CarState.Drifting || state == CarState.Skidding;
	}

	virtual protected void HandleInput(float dt){
		// speed = 0;
		turn = 0;
		gasPedal = 0;
		breakPedal = 0;
		eBrake = false;
	}
	void HandleAudio(){
		if(isSliding() && !tireSoundPlayer.Playing) tireSoundPlayer.Play();
		else if(!isSliding() && tireSoundPlayer.Playing) tireSoundPlayer.Stop();
		engineSoundPlayer.PitchScale = 1 + Velocity.Length() / MaxSpeed;
	}
	public float GetSpeed(){
		return Transform.BasisXformInv(Velocity).X;
	}
	public void SetSpeed(float speed){
		speed = Math.Clamp(speed, -MinSpeed, MaxSpeed);
		Velocity = Transform.BasisXform(Vector2.Right) * speed;
	}
		// GD.Print(Health);

	void Move(float dt){
		float speed = GetSpeed();
		float velocityMag = Velocity.Length();
		// GD.Print(state);
		switch (state)
		{
			case CarState.Drifting:
				if(!eBrake) state = CarState.Normal;
				goto case CarState.Skidding;
			case CarState.Skidding:
				// maintain and damp velocity, maintain angular velocity
				Rotate(angularVelocity * dt);
				// is car still skidding?
				if(velocityMag < SkidDamping){
					state = CarState.Normal;
				}
				else{
					Velocity = Velocity.Normalized() * (velocityMag - SkidDamping * dt);
				}
				MoveAndSlide();
			break;
			case CarState.Normal:
				if(eBrake && velocityMag > SkidSpeed && Math.Abs(turn) > 0.7){
					state = CarState.Drifting;
					MoveAndSlide();
					break;
				}
				float accel = gasPedal * AccelPower - breakPedal * BreakPower;
				// handle drag
				float dragAgg = Drag;
				if(eBrake) dragAgg += EBreakPower;
				if(speed > Drag) accel -= dragAgg;
				if(speed < -Drag) accel += dragAgg;
				// calculate and apply angular velocity
				angularVelocity = turn * TurnSpeed * speed / MaxSpeed;
				Rotate(angularVelocity * dt);
				// apply first half of accel
				speed += accel * dt * 0.5f;
				SetSpeed(speed);
				MoveAndSlide();
				// apply second half of accel
				speed += accel * dt * 0.5f;
				SetSpeed(speed);
				break;
			default:
			break;
		}
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

