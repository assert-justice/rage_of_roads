using Godot;
using System;
using System.Collections.Generic;

public partial class SplitScreenManager : Node
{
	private int _numberOfPlayers = 4;
	struct Player
	{
		public Viewport Viewport;
		public Camera2D Camera2D;
		public Node PlayerNode;

		public Player(Viewport viewport, Camera2D camera2D, Node playerNode)
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
		for (int i = 0; i < _numberOfPlayers; i++)
		{
			Player tempPlayer = new Player(
				GetNode<Viewport>($"GridContainer/SubViewportContainer{i.ToString()}/SubViewport"),
				GetNode<Camera2D>($"GridContainer/SubViewportContainer{i.ToString()}/SubViewport/Camera2D"),
				GetNode<Node>($"GridContainer/SubViewportContainer0/SubViewport/Game/Player{i.ToString()}"));
			GD.Print(tempPlayer.PlayerNode);
			if (i > 0)
			{
				tempPlayer.Viewport.World2D = _playersInfo[0].Viewport.World2D;
			}
			_playersInfo.Add(tempPlayer);
		}

		foreach (var playerInfo in _playersInfo)
		{
			RemoteTransform2D remoteTransform2D = new RemoteTransform2D();
			remoteTransform2D.RemotePath = playerInfo.Camera2D.GetPath();
			playerInfo.PlayerNode.AddChild(remoteTransform2D);
		}
	}		


}
