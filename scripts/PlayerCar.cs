using Godot;

public partial class PlayerCar : Car
{
	Label uiText;
	Camera2D camera;
	View view;
	public override void _Process(double delta)
	{
		base._Process(delta);
		UIUpdate();
	}
	public void SetView(View view){
		this.view = view;
		uiText = view.GetUILabel();
		camera = view.GetCamera();
		camera.Zoom = Vector2.One * 0.75f;
		var remoteTransform2D = new RemoteTransform2D
		{
			RemotePath = camera.GetPath()
		};
		AddChild(remoteTransform2D);
	}
	void UIUpdate(){
		var healthStr = ((int)health).ToString("D3");
		var rageStr = ((int)rage).ToString("D3");
		var ammoStr = ((int)ammo).ToString("D3");
		if(ammo < 0) ammoStr = "XXX";
		// uiText.Text = $"Lives: {Lives}\nHealth: {healthStr}%\nAmmo: {ammoStr}%\nRage: {rageStr}%";
	}
	protected override void HandleInput(float dt)
	{
		turn = Input.GetAxis("left", "right");
		gasPedal = Input.GetActionStrength("accelerate");
		breakPedal = Input.GetActionStrength("brake");
		eBrake = Input.IsActionPressed("e_brake");
		fireLeft = Input.IsActionPressed("fire_left");
		fireRight = Input.IsActionPressed("fire_right");
	}
}
