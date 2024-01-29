using System;
using Godot;

public partial class PlayerCar : Car
{
	[Export] 
	public Resource Controls;
	[Export]
	public float ShakeIntensity = 1;
	[Export]
	public float CrashSensitivity = 600;
	Control ui;
	Label uiText;
	Camera2D camera;
	RemoteTransform2D cameraTransform;
	Vector2 cameraStartPosition;
	Vector2 cameraNextPosition;
	Vector2 cameraLastPosition;
	float shakeDuration = 0;
	float shakeTime = 0.1f;
	float shakeClock = 0;
	float shakeRange = 0;
	View view;
	public override void _Ready()
	{
		base._Ready();
		AddToGroup("Player");
	}
	public override void _Process(double delta)
	{
		float dt = (float)delta;
		base._Process(delta);
		UIUpdate();
		HandleShake(dt);
	}
	protected override void Move(float dt)
	{
		var startV = Velocity;
		base.Move(dt);
		var endV = Velocity;
		// detect crashes
		if((startV - endV).Length() > CrashSensitivity){
			SetShake(0.3f, 10, false);
		}
	}
	public override void Damage(float value)
	{
		base.Damage(value);
		SetShake(0.3f, 3, false);
	}
	Vector2 Lerp(float t, Vector2 a, Vector2 b){
		return new Vector2((b.X - a.X) * t + a.X, (b.Y - a.Y) * t + a.Y);
	}
	void HandleShake(float dt){
		if(shakeDuration <= 0) return;
		shakeDuration -= dt;
		if(shakeDuration <= 0){
			// end shake
			shakeClock = 0;
			cameraTransform.Position = cameraStartPosition;
			return;
		}
		if(shakeClock > 0){
			shakeClock -= dt;
			cameraTransform.Position = Lerp(shakeClock / shakeTime, cameraLastPosition, cameraNextPosition);
			return;
		}
		// pick a new camera position
		Vector2 offset = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1) * shakeRange * ShakeIntensity;
		cameraLastPosition = cameraNextPosition;
		cameraNextPosition = cameraStartPosition + offset;
		shakeClock = shakeTime;
		cameraTransform.Position = cameraLastPosition;
	} 
	void SetShake(float duration, float intensity, bool priority){
		// if low priority and camera is already shaking do not overwrite it
		if(!priority && shakeDuration > 0) return;
		shakeDuration = duration;
		shakeRange = intensity;
		shakeClock = 0;
		cameraLastPosition = cameraStartPosition;
		cameraNextPosition = cameraStartPosition;
	}
	public void SetView(View view){
		this.view = view;
		ui = view.GetUI();
		uiText = ui.GetNode<Label>("Panel/Label");
		camera = view.GetCamera();
		cameraTransform = new RemoteTransform2D
		{
			RemotePath = camera.GetPath(),
			Position = Vector2.Right * 300,
		};
		cameraStartPosition = cameraTransform.Position;
		AddChild(cameraTransform);
	}
	void UIUpdate(){
		var healthStr = ((int)health).ToString("D3");
		var rageStr = ((int)rage).ToString("D3");
		var ammoStr = ((int)ammo).ToString("D3");
		if(ammo < 0) ammoStr = "XXX";
		uiText.Text = $"Lives: {Lives}\nHealth: {healthStr}%\nAmmo: {ammoStr}%\nRage: {rageStr}%";
	}
	protected override void HandleInput(float dt)
	{
		if (Controls is PlayerControls playerControls){
			turn = Input.GetAxis(playerControls.Left, playerControls.Right);
			gasPedal = Input.GetActionStrength(playerControls.Accelerate);
			breakPedal = Input.GetActionStrength(playerControls.Brake);
			eBrake = Input.IsActionPressed(playerControls.EBrake);
			fireLeft = Input.IsActionPressed(playerControls.FireLeft);
			fireRight = Input.IsActionPressed(playerControls.FireRight);
		}
	}
	public override void Die()
	{
		// view.QueueFree();
		if(GetTree().GetNodesInGroup("Player").Count == 1){
			GetTree().Root.GetChild<MenuManager>(0).Lost();
		}
		base.Die();
	}
}
