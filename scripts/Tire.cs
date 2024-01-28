using Godot;
using System;

public partial class Tire : AnimatedSprite2D
{
	public Car ParentCar;
	Line2D skid;
	public void BeginSkid(){
		if(skid is not null) EndSkid();
        skid = new Line2D
        {
            DefaultColor = Colors.Black,
        };
		ParentCar.GetParent().AddChild(skid);
    }
	public void EndSkid(){
		skid = null;
	}
    public override void _Process(double delta)
    {
        base._Process(delta);
		if(skid is null) return;
		skid.AddPoint(GlobalPosition);
    }
}
