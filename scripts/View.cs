using Godot;
using System;

public partial class View : Control
{
	PlayerCar playerCar;
	AnimatedSprite2D Face;
	AnimatedSprite2D Vial;
	AnimatedSprite2D HealthBar;
	AnimatedSprite2D BreathBar;
	// public Vector2 ViewSize;
	public override void _Ready()
	{
		base._Ready();
		Face = GetNode<AnimatedSprite2D>("UI/Readouts/Face");
		Vial = GetNode<AnimatedSprite2D>("UI/Readouts/Vial");
		Vial.Play();
		HealthBar = GetNode<AnimatedSprite2D>("UI/Readouts/HealthBar");
		BreathBar = GetNode<AnimatedSprite2D>("UI/Readouts/BreathBar");
		// AnimatedSprite2D[] sprites = {Face, Vial, HealthBar, BreathBar};
		// float x = 0;
		// for (int idx = 0; idx < sprites.Length; idx++)
		// {
		// 	var spriteSize = sprites[idx].SpriteFrames.GetFrameTexture(sprites[idx].Animation, 0).GetSize();
		// 	var y = ViewSize.Y - spriteSize.Y;
		// 	sprites[idx].Position = new Vector2(x + spriteSize.X/2, y+spriteSize.Y/2);
		// 	x += spriteSize.Y;
		// 	GD.Print(x, " ", y);
		// }
	}
	public void SetDimensions(Vector2 Size){
		Face = GetNode<AnimatedSprite2D>("UI/Readouts/Face");
		HealthBar = GetNode<AnimatedSprite2D>("UI/Readouts/HealthBar");
		Vial = GetNode<AnimatedSprite2D>("UI/Readouts/Vial");
		BreathBar = GetNode<AnimatedSprite2D>("UI/Readouts/BreathBar");
		AnimatedSprite2D[] sprites = {Face, HealthBar, Vial, BreathBar};
		var faceSize = Face.SpriteFrames.GetFrameTexture(Face.Animation, 0).GetSize();
		// float x = 0;
		var y = Size.Y - faceSize.Y;
		Face.Position = new Vector2(faceSize.X/2, y + faceSize.Y/2);
		HealthBar.Position = new Vector2(faceSize.X/2, y + faceSize.Y/2);
		Vial.Position = new Vector2(faceSize.X, y+faceSize.Y/2);
		Vial.Scale = Vector2.One*0.5f;
		BreathBar.Position = new Vector2(faceSize.X, y);
		BreathBar.Scale = Vector2.One*0.5f;
		// for (int idx = 0; idx < sprites.Length; idx++)
		// {
		// 	var spriteSize = sprites[idx].SpriteFrames.GetFrameTexture(sprites[idx].Animation, 0).GetSize();
		// 	var y = Size.Y - spriteSize.Y;
		// 	sprites[idx].Position = new Vector2(x + spriteSize.X/2, y+spriteSize.Y/2);
		// 	if(idx > 0) x += spriteSize.Y;
		// }
	}
	public void SetPlayer(PlayerCar car){
		playerCar = car;
		car.SetView(this);
		GetNode<Minimap>("UI/Minimap").ParentCar = car;
	}
	public void SetStats(float rage, float ammo, float health){
		Face.Frame = (int)(Face.SpriteFrames.GetFrameCount("default") * rage);
		HealthBar.Frame = (int)(HealthBar.SpriteFrames.GetFrameCount("default") * (1-health));
		BreathBar.Frame = (int)(BreathBar.SpriteFrames.GetFrameCount("default") * (1-ammo));
		var vFrame = Vial.Frame;
		var vProgress = Vial.FrameProgress;
		Vial.Animation = $"lvl{Face.Frame + 1}";
		Vial.Frame = vFrame;
		Vial.FrameProgress = vProgress;
		// Face.Frame = (int)(Face.SpriteFrames.GetFrameCount("default") * rage);
	}
	public Control GetUI(){
		return GetNode<Control>("UI");
	}
	public Camera2D GetCamera(){
		return GetNode<Camera2D>("SubViewportContainer/SubViewport/Camera2D");
	}
	public SubViewport GetSubViewport(){
		return GetNode<SubViewport>("SubViewportContainer/SubViewport");
	}
	public void AddGame(Game game){
		GetSubViewport().AddChild(game);
	}
}
