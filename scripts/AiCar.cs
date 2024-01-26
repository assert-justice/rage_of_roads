using System;
using Godot;

public enum AiState{
	idle,
	roaming,
	reversing,
	pursuing,
	attacking,
	acquiring,
	fleeing,
}

/*
# Ai Tactics

## Idle

In idle state do nothing, easy enough.

## Roaming

In roaming state pick new visible positions at random and drive to them.
If stopped as if by an obstacle, [reverse].
If a valid enemy is detected enter [pursuit] state.
If health is below retreat threshold and health items are available [acquire] health. Otherwise flee
If a power up is in view [acquire] it.

## Reversing

Reverse and turn until destination is in sight or car is stopped. Ether way, go back to roaming.

## Pursuing

In pursuing state try to get within range of target. Expect target to be defined.
If the target is visible, set destination to target position and drive toward it.
If target is not visible proceed to the last known target location.
If at last known location and target is not visible then [roam]
If target is within attack range [attack] it.
If health is below retreat threshold and health items are available [acquire] health. Otherwise flee
If a power up is in view [acquire] it.

## Attacking

In attacking state use weapons to destroy target.
If rage meter is full use rage attack.
If target is outside of attack range or not visible [pursue].
If health is below retreat threshold and health items are available [acquire] health. Otherwise flee

## Acquiring

In acquiring state drive toward an item.
If item is not visible (i.e. it has been collected) then [roam].
*/

public partial class AiCar : Car{
	[Export]
	public AiState state;
	Node2D target;
	Vector2 targetLastPosition = new Vector2();
	Vector2 destination = new Vector2();
	float stalledTime = 0;
	float newDestDeadline = 5;
	float timeEnRoute = 0;
	[Export]
	float destinationRadius = 100;
	Node2D navNode;
	public override void _Ready()
	{
		base._Ready();
		SetState(AiState.roaming);
		navNode = GetNode<Node2D>("NavNode");
		RemoveChild(navNode);
		// GetParent().AddChid(navNode);
		GetParent().CallDeferred("add_child", navNode);
		// GetParent().add
		navNode.Position = Vector2.Zero;
	}
	void SetState(AiState state){
		// GD.Print(state);
		this.state = state;
		// switch (state)
		// {
		// 	case AiState.roaming:
		// 		destination = Position;
		// 		// destination = Vector2.Zero;
		// 		break;
		// 	case AiState.reversing:
		// 		break;
		// 	case AiState.idle:
		// 		// intentional fall through
		// 	default:
		// 		break;
		// }
	}
	bool AtDestination(){
		return (Position - destination).Length() < destinationRadius;
	}
	void DriveToDestination(){
		// turn wheel to face dest
		var nd = (destination - Position).Normalized();
		var dot = nd.Dot(Transform.BasisXform(Vector2.Down));
		gasPedal = 1;
		if(dot > 0.1) turn = 1;
		else if(dot < -0.1) turn = -1;
		// else{gasPedal = 1;}
	}
	void ReverseToFaceDestination(){
		// turn wheel to face dest
		var nd = (destination - Position).Normalized();
		var dot = nd.Dot(Transform.BasisXform(Vector2.Down));
		if(dot > 0.1) turn = -1;
		else if(dot < -0.1) turn = 1;
		else {
			SetState(AiState.roaming);
			// state = AiState.roaming;
			// speed = 100;
			SetSpeed(100);
			return;
		}
		gasPedal = 0;
		breakPedal = 1;
		// else{gasPedal = 1;}
	}
	void RandomizeDestination(){
		// speed = 300;
		SetSpeed(300);
		for (int i = 0; i < 10; i++)
			{
				// pick angle
				float angle = GD.Randf() * (float)Math.PI * 2.0f;
				// pick distance
				float distance = GD.Randf() * 2000 + 500;
				float destY = (float)Math.Sin(angle) * distance;
				float destX = (float)Math.Cos(angle) * distance;
				var dest = new Vector2(destX, destY) + Position;
				var spaceState = GetWorld2D().DirectSpaceState;
				var query = PhysicsRayQueryParameters2D.Create(Position, dest);
				var res = spaceState.IntersectRay(query);
				if(res.Count == 0){
					// GD.Print(dest);
					destination = dest;
					timeEnRoute = 0;
					navNode.Position = destination;
					break;
				}
			}
	}
	protected override void HandleInput(float dt)
	{
		base.HandleInput(dt);
		if(GetSpeed() < 100){
			stalledTime += dt;
		}
		else{
			stalledTime = 0;
		}
		switch (state)
		{
			case AiState.roaming:
				if(stalledTime > newDestDeadline) {
					// SetState(AiState.roaming);
					// destination = Position;
					// GD.Print("give up");
					RandomizeDestination();
					stalledTime = 0;
					break;
				}
				if(AtDestination() || timeEnRoute > newDestDeadline){
					// find new destination
					RandomizeDestination();
					break;
				}
				timeEnRoute += dt;
				if(GetSpeed() < 100){
					SetState(AiState.reversing);
					break;
				}
				DriveToDestination();
				break;
			case AiState.reversing:
				if(stalledTime > newDestDeadline) {
					stalledTime = 0;
					SetState(AiState.roaming);
					RandomizeDestination();
					// GD.Print("give up 2");
					SetSpeed(300);
					break;
				}
				ReverseToFaceDestination();
				break;
			case AiState.idle:
				// intentional fall through
			default:
				break;
		}
		// if(Input.IsMouseButtonPressed(MouseButton.Left)){
		// 	Position = GetViewport().GetMousePosition();
		// 	SetState(AiState.roaming);
		// }
	}
}
