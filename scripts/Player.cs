using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	float AccelPower = 300;
	[Export]
	float BreakPower = 800;
	[Export]
	float MaxSpeed = 800;
	[Export]
	float MinSpeed = 300;
	[Export]
	float TurnSpeed = 2;
	[Export]
	AudioStream EngineSound;
	
	// movement and input
	float speed = 0;
	float turn = 0;
	float gasPedal = 0;
	float breakPedal = 0;
	// camera
	Camera2D camera;
	// Sfx
	AudioStreamPlayer2D engineSoundPlayer;

	public override void _Ready()
	{
		camera = GetNode<Camera2D>("Camera2D");
		engineSoundPlayer = GetNode<AudioStreamPlayer2D>("EngineSound");
	}
	public override void _Process(double delta)
	{
		float dt = (float) delta;
		HandleInput();
		Move(dt);
	}

	void HandleInput(){
		turn = Input.GetAxis("left", "right");
		gasPedal = Input.GetActionStrength("accelerate");
		breakPedal = Input.GetActionStrength("break");
	}

	void Move(float dt){
		float accel = gasPedal * AccelPower - breakPedal * BreakPower;
		// split acceleration in two for time step reasons
		speed += accel * dt * 0.5f;
		speed = Mathf.Clamp(speed, -MinSpeed, MaxSpeed);
		speed = speed > MaxSpeed ? MaxSpeed : speed;
		float deltaAngle = dt * turn * TurnSpeed * speed / MaxSpeed;
		Rotate(deltaAngle);
		var vel = new Vector2(speed * dt, 0);
		Velocity = Transform.BasisXform(vel);
		MoveAndCollide(Velocity);
		// finish accelerating
		speed += accel * dt * 0.5f;
		speed = Mathf.Clamp(speed, -MinSpeed, MaxSpeed);
		if(speed > 0 && !engineSoundPlayer.Playing) engineSoundPlayer.Play();
		else if(speed <= 0 && engineSoundPlayer.Playing) engineSoundPlayer.Stop();
	}
}

