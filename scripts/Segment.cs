using Godot;
using System;

public partial class Segment : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	public void SetCar(int carIdx){
		Frame = carIdx;
	}
	public void SetIsDamaged(bool isDamaged){
		var frame = Frame;
		Animation = isDamaged ? "damaged" : "default";
		GetNode<AnimatedSprite2D>("Outline").Frame = isDamaged ? 1 : 0;
		Frame = frame;
	}
	private void _on_area_2d_area_entered(Area2D area)
	{
		if(area is Bullet) SetIsDamaged(true);
	}
}
