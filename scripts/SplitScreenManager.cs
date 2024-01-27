using Godot;
using System;
using System.Collections.Generic;

public partial class SplitScreenManager : Node
{
	private int _numberOfPlayers = 3;
	struct Player
	{
		public SubViewport Viewport;
		public Camera2D Camera2D;
		public Node PlayerNode;

		public Player(SubViewport viewport, Camera2D camera2D, Node playerNode)
		{
			Viewport = viewport;
			Camera2D = camera2D;
			PlayerNode = playerNode;
		}
	}

	private List<Player> _playersInfo;

	public override void _Ready()
	{
		_playersInfo = new List<Player>();
		for (int i = 0; i < 4; i++)
		{
			
			Player tempPlayer = new Player(
				GetNode<SubViewport>($"GridContainer/SubViewportContainer{i.ToString()}/SubViewport"),
				GetNode<Camera2D>($"GridContainer/SubViewportContainer{i.ToString()}/SubViewport/Camera2D"),
				GetNode<Node>($"GridContainer/SubViewportContainer0/SubViewport/Game/Player{i.ToString()}"));
			GD.Print(tempPlayer.PlayerNode);
			if (i > 0)
			{
				tempPlayer.Viewport.World2D = _playersInfo[0].Viewport.World2D;
			}

			if (i > _numberOfPlayers)
			{
				
				tempPlayer.PlayerNode.QueueFree();
				tempPlayer.Viewport.QueueFree();
				tempPlayer.Camera2D.QueueFree();
				continue;
			}
			_playersInfo.Add(tempPlayer);
		}

		foreach (var playerInfo in _playersInfo)
		{
			RemoteTransform2D remoteTransform2D = new RemoteTransform2D();
			remoteTransform2D.RemotePath = playerInfo.Camera2D.GetPath();
			playerInfo.PlayerNode.AddChild(remoteTransform2D);
		}

		switch (_numberOfPlayers)
		{
			case 1: //Just load level
				break;
			case 2:
				foreach (var playerInfo in _playersInfo)
				{
					playerInfo.Viewport.Size = new Vector2I(960, 1080);
				}
				break;
			case 3: //Design discussion about what we should do 
				break;
		}
	}		


}
