using Godot;

public partial class PlayerCar : Car
{
	[Export] 
	Resource Controls;
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
		if (Controls is PlayerControls playerControls)
		{
			turn = Input.GetAxis(playerControls.Left, playerControls.Right);
			gasPedal = Input.GetActionStrength(playerControls.Accelerate);
			breakPedal = Input.GetActionStrength(playerControls.Brake);
			eBrake = Input.IsActionPressed(playerControls.EBrake);
			fireLeft = Input.IsActionPressed(playerControls.FireLeft);
			fireRight = Input.IsActionPressed(playerControls.FireRight);
		}
	}
}
