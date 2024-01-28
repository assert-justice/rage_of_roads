using Godot;

public partial class PlayerCar : Car
{
	[Export] 
	public Resource Controls;
	Control ui;
	Label uiText;
	Camera2D camera;
	RemoteTransform2D cameraTransform;
	View view;
	public override void _Ready()
	{
		base._Ready();
		AddToGroup("Player");
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		UIUpdate();
		// cameraTransform.Position = Vector2.Right * GetSpeed() / MaxSpeed * 300;
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
