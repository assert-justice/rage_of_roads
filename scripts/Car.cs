using Godot;

public partial class Car : CharacterBody2D
{
	[Export]
	float AccelPower = 300;
	[Export]
	float BreakPower = 800;
	[Export]
	float EBreakPower = 800;
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
	protected bool eBrake = false;
	AudioStreamPlayer2D engineSoundPlayer;

	public override void _Ready()
	{
		engineSoundPlayer = GetNode<AudioStreamPlayer2D>("EngineSound");
	}
	public override void _Process(double delta)
	{
		float dt = (float) delta;
		HandleInput(dt);
		Move(dt);
	}

	virtual protected void HandleInput(float dt){
		// speed = 0;
		turn = 0;
		gasPedal = 0;
		breakPedal = 0;
		eBrake = false;
		// turn = Input.GetAxis("left", "right");
		// gasPedal = Input.GetActionStrength("accelerate");
		// breakPedal = Input.GetActionStrength("break");
	}
	void ClampSpeed(){
		var min = eBrake ? 0 : -MinSpeed;
		speed = Mathf.Clamp(speed, min, MaxSpeed);
	}

	void Move(float dt){
		float accel = gasPedal * AccelPower - breakPedal * BreakPower;
		if(eBrake) accel -= EBreakPower;
		// split acceleration in two for time step reasons
		speed += accel * dt * 0.5f;
		ClampSpeed();
		float deltaAngle = dt * turn * TurnSpeed * speed / MaxSpeed;
		Rotate(deltaAngle);
		Velocity = Transform.BasisXform(Vector2.Right * speed);
		bool didCollide = MoveAndSlide();
		if(didCollide){
			speed = Velocity.Length();
		}
		speed += accel * dt * 0.5f;
		ClampSpeed();
	}
}

