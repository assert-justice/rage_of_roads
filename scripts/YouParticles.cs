using Godot;
using System;

public partial class YouParticles : Node
{
	[Export] private ShapeCast2D _shapeCast2D;

	[Export] private float _durationTimer = 1f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_shapeCast2D = GetNode<ShapeCast2D>("ShapeCast2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
