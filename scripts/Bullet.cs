using Godot;
using System;

public partial class Bullet : Area2D
{
    [Export]
    public Vector2 Velocity = new Vector2();
    public override void _Process(double delta)
    {
        base._Process(delta);
        Position += Velocity * (float)delta;
    }
}
