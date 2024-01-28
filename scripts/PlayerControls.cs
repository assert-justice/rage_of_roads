using Godot;
using System;

[Tool]
public partial class PlayerControls : Resource
{
	[Export] public int PlayerNumber { get; set; } = 0;
	[Export] public string Accelerate { get; set; }
	[Export] public string Brake { get; set; }
	[Export] public string Left { get; set; }
	[Export] public string Right { get; set; }
	[Export] public string Pause { get; set; }
	[Export] public string EBrake { get; set; }
	[Export] public string FireLeft { get; set; }
	[Export] public string FireRight { get; set; }
	
	public PlayerControls() : this(0, null, null, null, null, null, null, null, null) {}

	public PlayerControls(int playerNumber, string accelerate, string brake, string left, string right, string pause, string eBrake, string fireLeft, string fireRight)
	{
		PlayerNumber = playerNumber;
		Accelerate = accelerate;
		Brake = brake;
		Left = left;
		Right = right;
		Pause = pause;
		EBrake = eBrake;
	}
	
}
