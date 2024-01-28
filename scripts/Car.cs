using System;
using System.Collections.Generic;
using Godot;

enum CarState{
	Normal,
	Drifting,
	Skidding,
}

public enum PowerupType{
	Energy,
	Whiskey,
	Megaphone,
	Burger,
}
public partial class Car : CharacterBody2D
{
	[Export]
	public int SpriteId = 0;
	[ExportGroup("Movement")]
	[Export]
	float AccelPower = 500;
	[Export]
	float BreakPower = 800;
	[Export]
	float EBreakPower = 800;
	[Export]
	protected float MaxSpeed = 1600;
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
	[Export]
	float BoostThreshold = 0.75f;
	[Export]
	float BoostPower = 1600;

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
	[ExportGroup("Powerups")]
	[Export]
	float EnergyTime = 10;
	float energyClock = 0;
	[Export]
	float MegaphoneTime = 10;
	float megaphoneClock = 0;
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
	float timeBoosting = 0;
	protected float angularVelocity;
	protected bool fireLeft = false;
	protected bool fireRight = false;
	protected bool fireAlt = false;
	// audio
	AudioStreamPlayer2D engineSoundPlayer;
	AudioStreamPlayer2D tireSoundPlayer;
	// guns
	protected Gun leftGun;
	protected Gun rightGun;
	// segments
	List<Segment> segments = new List<Segment>();
	List<Tire> tires = new List<Tire>();
	// particles
	GpuParticles2D leftBoostParticles;
	GpuParticles2D rightBoostParticles;
	GpuParticles2D leftDriftParticles;
	GpuParticles2D rightDriftParticles;
	GpuParticles2D burgerParticles;
	GpuParticles2D whiskeyParticles;
	GpuParticles2D energyParticles;
	GpuParticles2D megaphoneParticles;
	GpuParticles2D postBoostParticles;

	public override void _Ready()
	{
		engineSoundPlayer = GetNode<AudioStreamPlayer2D>("Sfx/EngineSound");
		tireSoundPlayer = GetNode<AudioStreamPlayer2D>("Sfx/TireSound");
		leftGun = GetNode<Gun>("LeftGun");
		rightGun = GetNode<Gun>("RightGun");
		leftDriftParticles = GetNode<GpuParticles2D>("Particles/LeftDriftParticles");
		rightDriftParticles = GetNode<GpuParticles2D>("Particles/RightDriftParticles");
		leftBoostParticles = GetNode<GpuParticles2D>("Particles/LeftBoostParticles");
		rightBoostParticles = GetNode<GpuParticles2D>("Particles/RightBoostParticles");
		burgerParticles = GetNode<GpuParticles2D>("Particles/BurgerParticles");
		energyParticles = GetNode<GpuParticles2D>("Particles/EnergyParticles");
		megaphoneParticles = GetNode<GpuParticles2D>("Particles/MegaphoneParticles");
		whiskeyParticles = GetNode<GpuParticles2D>("Particles/WhiskeyParticles");
		postBoostParticles = GetNode<GpuParticles2D>("Particles/PostBoostParticles");
		AddToGroup("Car");
		startPosition = Position;
		// SetSpriteId(SpriteId);
		GetNode<AnimatedSprite2D>("Segments/EngineHole/Engine").Play();
		foreach (var node in GetNode("Segments").GetChildren())
		{
			if(node is Segment segment){
				segments.Add(segment);
			}
			else if(node is Tire tire){
				tires.Add(tire);
				tire.Play();
				tire.ParentCar = this;
			}
		}
		var idx = GetTree().GetNodesInGroup("Car").IndexOf(this);
		SetSpriteId(idx);
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
		float engineSpeed = Math.Clamp(Velocity.Length() / MaxSpeed, 0.5f, 1);
		GetNode<AnimatedSprite2D>("Segments/EngineHole/Engine").SpeedScale = engineSpeed;
		// win handling, it's bad ok?
		var numCars = GetTree().GetNodesInGroup("Car").Count;
		if(numCars == 1) {
			string[] color = {"red", "green", "blue", "yellow"};
			string text = $"The {color[SpriteId]} player wins!";
			GetTree().Root.GetChild<MenuManager>(0).Won(text);
		}
		float speed = GetSpeed() / MaxSpeed;
		foreach (var tire in tires)
		{
			tire.SpeedScale = speed;
		}
		float tireAngle = -turn * 6.0f;
		tires[0].Rotation = tireAngle;
		tires[1].Rotation = tireAngle;
	}
	public void SetSpriteId(int spriteId){
		SpriteId = spriteId;
		foreach (var segment in segments)
		{
			segment.SetCar(SpriteId);
		}
	}
	public bool AddPowerup(PowerupType powerupType){
		switch (powerupType)
		{
			case PowerupType.Energy:
				energyClock = EnergyTime;
				energyParticles.Emitting = true;
				return true;
			case PowerupType.Whiskey:
				if(rage == 100) return false;
				rage = 100;
				whiskeyParticles.Emitting = true;
				return true;
			case PowerupType.Megaphone:
				megaphoneClock = MegaphoneTime;
				megaphoneParticles.Emitting = true;
				return true;
			case PowerupType.Burger:
				if(health == 100) return false;
				health = 100;
				burgerParticles.Emitting = true;
				foreach (var segment in segments)
				{
					segment.SetIsDamaged(false);
				}
				return true;
			default:
			break;
		}
		return false;
	}
	public void Damage(float value){
		health -= value;
		if(health < 0){
			// make the car visually explode or whatever
			Lives -= 1;
			Position = startPosition;
			health = 100;
			if(Lives == 0){
				Die();
			}
			foreach (var segment in segments)
			{
				segment.SetIsDamaged(false);
			}
		}
	}
	public virtual void Die(){
		QueueFree();
	}
	bool isSliding(){
		return state == CarState.Drifting || state == CarState.Skidding;
	}

	virtual protected void HandleInput(float dt){
		// speed = 0;
		// turn = 0;
		// gasPedal = 0;
		// breakPedal = 0;
		// eBrake = false;
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
	void SetCarState(CarState carState){
		var wasSliding = isSliding();
		state = carState;
		if(!wasSliding && isSliding()){
			foreach (var tire in tires)
			{
				tire.BeginSkid();
			}
		}
		else if(wasSliding && !isSliding()){
			foreach (var tire in tires)
			{
				tire.EndSkid();
			}
		}
	}

	void Move(float dt){
		float speed = GetSpeed();
		float velocityMag = Velocity.Length();
		if(timeBoosting > BoostThreshold){
			if(!leftDriftParticles.Emitting){
				leftDriftParticles.Emitting = true;
				rightDriftParticles.Emitting = true;
			}
		}
		else if(leftDriftParticles.Emitting){
			leftDriftParticles.Emitting = false;
			rightDriftParticles.Emitting = false;
		}
		switch (state)
		{
			case CarState.Drifting:
				timeBoosting += dt;
				if(!eBrake) {
					SetCarState(CarState.Normal);
					if(timeBoosting > BoostThreshold){
						if(gasPedal > 0) SetSpeed(BoostPower);
						else if(breakPedal > 0) SetSpeed(-BoostPower);
						leftBoostParticles.Emitting = true;
						rightBoostParticles.Emitting = true;
						postBoostParticles.Emitting = true;
					}
					MoveAndSlide();
					break;
				}
				goto case CarState.Skidding;
			case CarState.Skidding:
				// maintain and damp velocity, maintain angular velocity
				Rotate(angularVelocity * dt);
				// is car still skidding?
				if(velocityMag < SkidDamping){
					SetCarState(CarState.Normal);
				}
				else{
					Velocity = Velocity.Normalized() * (velocityMag - SkidDamping * dt);
				}
				MoveAndSlide();
			break;
			case CarState.Normal:
				timeBoosting = 0;
				if(eBrake && velocityMag > SkidSpeed && Math.Abs(turn) > 0.7){
					SetCarState(CarState.Drifting);
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
				speed = GetSpeed();
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

