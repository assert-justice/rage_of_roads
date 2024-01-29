using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
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
	[Export] private float _adjustedAcceleration = 800;

	[Export] private float _adjustedMaxSpeed = 1800;
	[Export]
	float EnergyTime = 10;
	float energyClock = 0;
	private bool _energyActive = false;
	[Export]
	float MegaphoneTime = 10;
	float megaphoneClock = 0;
	[Export] private float _megaPhoneScale = 2;
	[Export] private AudioStreamWav _energy;
	[Export] private AudioStreamWav _whiskey;
	[Export] private AudioStreamWav _megaphone;
	[Export] private AudioStreamWav _burger;
	private bool _megaPhoneActive = false;
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
	AudioStreamPlayer2D _powerUpSoundPlayer;
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
	Game game;

	public override void _Ready()
	{
		engineSoundPlayer = GetNode<AudioStreamPlayer2D>("Sfx/EngineSound");
		tireSoundPlayer = GetNode<AudioStreamPlayer2D>("Sfx/TireSound");
		_powerUpSoundPlayer = GetNode<AudioStreamPlayer2D>("Sfx/PowerupSounds");
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
	public void SetGame(Game game){
		this.game = game;
	}
	public override void _Process(double delta)
	{
		float dt = (float) delta;
		HandleInput(dt);
		Move(dt);
		HandleAudio();
		HandleTimers(dt);
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

	private void HandleTimers(float dt)
	{
		if (megaphoneClock >= 0)
			megaphoneClock -= dt;
		if (energyClock >= 0)
			energyClock -= dt;
		if (megaphoneClock <= 0)
			_megaPhoneActive = false;
		if (energyClock <= 0)
		{
			// Yeah this shit is hardcoded, don't fuckin @ me
			AccelPower = 500;
			MaxSpeed = 1600;
		}
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
				AccelPower = _adjustedAcceleration;
				MaxSpeed = _adjustedMaxSpeed;
				_powerUpSoundPlayer.Stream = _energy;
				_powerUpSoundPlayer.Play();
				energyParticles.Emitting = true;
				return true;
			case PowerupType.Whiskey:
				rage = 100;
				_powerUpSoundPlayer.Stream = _whiskey;
				_powerUpSoundPlayer.Play();
				whiskeyParticles.Emitting = true;
				return true;
			case PowerupType.Megaphone:
				megaphoneClock = MegaphoneTime;
				_megaPhoneActive = true;
				_powerUpSoundPlayer.Stream = _megaphone;
				_powerUpSoundPlayer.Play();
				megaphoneParticles.Emitting = true;
				return true;
			case PowerupType.Burger:
				health = 100;
				_powerUpSoundPlayer.Stream = _burger;
				_powerUpSoundPlayer.Play();
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
	public virtual void Damage(float value){
		health -= value;
		if(health < 0){
			// make the car visually explode or whatever
			Lives -= 1;
			Position = game.GetSpawn().Position;
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
		if(Velocity != Vector2.Zero) engineSoundPlayer.Play();
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

	protected virtual void Move(float dt){
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
		if (rage < 100)
		{
			if (fireLeft && ammo > 0)
			{
				if (leftGun.Fire(_megaPhoneActive, _megaPhoneScale)) ammo -= FireCost;
				if (ammo < 0) ammo = -30;
			}

			if (fireRight && ammo > 0)
			{
				if (rightGun.Fire(_megaPhoneActive, _megaPhoneScale)) ammo -= FireCost;
				if (ammo < 0) ammo = -30;
			}
			return;
		}
		else
		{
			if (fireLeft)
			{
				if (leftGun.FireFuckBomb()) rage = 0;
			}

			if (fireRight)
			{
				if (rightGun.FireFuckBomb()) rage = 0;
			}
		}
	}
}

