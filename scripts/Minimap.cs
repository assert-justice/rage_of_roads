using Godot;
using System;
using System.Collections.Generic;

public partial class Minimap : Control
{
    List<Car> cars = new List<Car>();
    public Car ParentCar;
    public override void _Draw()
    {
        if(!IsInstanceValid(ParentCar)) return;
        base._Draw();
        // draw background
        var backgroundPosition = Vector2.One * Size / 2;
        var backgroundRadius = Size.X / 2;
        DrawCircle(backgroundPosition, backgroundRadius, Colors.Black);
        Color[] colors = {Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow};
        // draw cars
        foreach (var car in cars)
        {
            var offset = car.Position - ParentCar.Position;
            offset /= 4000;
            if(offset.Length() > 1) offset = offset.Normalized();
            offset *= Size.X / 2;
            DrawCircle(backgroundPosition + offset, 5, colors[car.SpriteId]);
        }
    }
    public override void _Process(double delta)
    {
        cars.Clear();
        foreach(var node in GetTree().GetNodesInGroup("Car")){
            if(node is Car car) cars.Add(car);
        }
        QueueRedraw();
    }
}
