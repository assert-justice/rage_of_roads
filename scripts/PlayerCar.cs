using Godot;

public partial class PlayerCar : Car
{
	protected override void HandleInput(float dt)
	{
		turn = Input.GetAxis("left", "right");
		gasPedal = Input.GetActionStrength("accelerate");
		breakPedal = Input.GetActionStrength("brake");
		eBrake = Input.IsActionPressed("e_brake");
	}
}
