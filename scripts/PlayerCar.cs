using Godot;

public partial class PlayerCar : Car
{
	Label uiText;
	public override void _Ready()
	{
		base._Ready();
		uiText = GetTree().GetFirstNodeInGroup("UIText") as Label;
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		UIUpdate();
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
		turn = Input.GetAxis("left", "right");
		gasPedal = Input.GetActionStrength("accelerate");
		breakPedal = Input.GetActionStrength("brake");
		eBrake = Input.IsActionPressed("e_brake");
		fireLeft = Input.IsActionPressed("fire_left");
		fireRight = Input.IsActionPressed("fire_right");
	}
}
