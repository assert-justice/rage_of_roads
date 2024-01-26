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
	float MinSpeed = 300;
	[Export]
	float TurnSpeed = 2;
	[Export]
	float Drag = 100;
	[Export]
	float SkidSpeed = 600;
	[ExportGroup("Weapons")]
	[Export]
	PackedScene bulletScene;
	[Export]
	float weaponCooldown = 0.1f;
	
	// movement and input
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
	}
	public override void _Process(double delta)
	{
		float dt = (float) delta;
		HandleInput(dt);
		Move(dt);
		HandleAudio();
		Fire(dt);
	}

	virtual protected void HandleInput(float dt){
		// speed = 0;
		turn = 0;
		gasPedal = 0;
		breakPedal = 0;
		eBrake = false;
	}
	void ClampSpeed(){
		var min = eBrake ? 0 : -MinSpeed;
		speed = Mathf.Clamp(speed, min, MaxSpeed);
	}
	void HandleAudio(){
		if(skidding && !tireSoundPlayer.Playing) tireSoundPlayer.Play();
		else if(!skidding && tireSoundPlayer.Playing) tireSoundPlayer.Stop();
		if(speed > 0){
			if(!engineSoundPlayer.Playing) engineSoundPlayer.Play();
			engineSoundPlayer.PitchScale = 1 + speed / MaxSpeed;
		}
	}

	void Move(float dt){
		bool didCollide;
		if(skidding){
			// maintain but damp velocity and angular velocity
			float vDamp = 400;
			if(speed > vDamp) speed -= vDamp * dt;
			else if(speed < -vDamp) speed += vDamp * dt;
			else{skidding = false;}
			Velocity = Transform.BasisXform(Vector2.Right * speed);
			Rotate(angularVelocity * dt);
			didCollide = MoveAndSlide();
			if(didCollide){
				speed = Velocity.Length();
			}
			if(!eBrake) skidding = false;
			return;
		}
		float accel = gasPedal * AccelPower - breakPedal * BreakPower;
		if(gasPedal == 0 && speed > 0) accel -= Drag;
		if(eBrake) accel -= EBreakPower;
		if(eBrake && speed > SkidSpeed && Math.Abs(turn) > 0.7) {
			skidding = true;
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

	void Fire(float dt){
		if(fireLeft) leftGun.Fire();
		if(fireRight) rightGun.Fire();
	}
}

