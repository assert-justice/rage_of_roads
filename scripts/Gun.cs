
using Godot;

public partial class Gun : Node2D{
    [Export]
    float Cooldown = 0.1f;
    [Export]
    PackedScene BulletScene;
    [Export]
    float BulletSpeed = 1800;
    float fireClock = 0;
    bool canFire = true;
    CharacterBody2D parent;
    public override void _Ready()
    {
        parent = GetParent<CharacterBody2D>();
    }
    public override void _Process(double delta)
    {
        float dt = (float) delta;
        if(fireClock > 0){
            fireClock -= dt;
        }
        canFire = fireClock <= 0;
    }
    public bool Fire(){
        if(!canFire) return false;
        if(BulletScene is null) return false;
        canFire = false;
        fireClock = Cooldown;
        var bullet = BulletScene.Instantiate<Bullet>();
        bullet.Position = GlobalPosition;
        // bullet.Rotation = GlobalRotation;
        parent.GetParent().AddChild(bullet);
        bullet.Velocity = GlobalTransform.BasisXform(Vector2.Left) * BulletSpeed + parent.Velocity;
        return true;
    }
}