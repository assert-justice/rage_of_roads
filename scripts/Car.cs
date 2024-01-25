using Godot;

public partial class Car : CharacterBody2D
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
	protected float speed = 0;
	protected float turn = 0;
	protected float gasPedal = 0;
	protected float breakPedal = 0;
	// camera
	// Camera2D camera;
	// Sfx
	AudioStreamPlayer2D engineSoundPlayer;

	public override void _Ready()
	{
		// camera = GetNode<Camera2D>("Camera2D");
		engineSoundPlayer = GetNode<AudioStreamPlayer2D>("EngineSound");
	}
	public override void _Process(double delta)
	{
		float dt = (float) delta;
		HandleInput();
		Move(dt);
	}

	virtual protected void HandleInput(){
		// speed = 0;
		turn = 0;
		gasPedal = 0;
		breakPedal = 0;
		// turn = Input.GetAxis("left", "right");
		// gasPedal = Input.GetActionStrength("accelerate");
		// breakPedal = Input.GetActionStrength("break");
	}

	void Move(float dt){
		float accel = gasPedal * AccelPower - breakPedal * BreakPower;
		// split acceleration in two for time step reasons
		speed += accel * dt * 0.5f;
		speed = Mathf.Clamp(speed, -MinSpeed, MaxSpeed);
		speed = speed > MaxSpeed ? MaxSpeed : speed;
		float deltaAngle = dt * turn * TurnSpeed;
		// float deltaAngle = dt * turn * TurnSpeed * speed / MaxSpeed;
		Rotate(deltaAngle);
		Velocity = Transform.BasisXform(Vector2.Right * speed);
		bool didCollide = MoveAndSlide();
		if(didCollide){
			speed = Velocity.Length();
		}
		speed += accel * dt * 0.5f;
		speed = Mathf.Clamp(speed, -MinSpeed, MaxSpeed);
	}
}

