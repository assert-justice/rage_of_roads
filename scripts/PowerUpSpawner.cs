using Godot;
using System;
using System.Diagnostics;
using Range = Godot.Range;

public partial class PowerUpSpawner : Area2D
{
	[Export] private PackedScene _powerUpScene;
	[Export] private float _powerUpTimer = 30;
	[Export] private int _maxHealthSpawns = 2;
	private Timer _timer;
	private int _healthSpawnCount = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = _powerUpTimer;
		SpawnNewPowerUp();
	}
	
	private void _on_body_entered(Node2D body)
	{
		if (body is Car car && _timer.IsStopped())
		{
			_timer.Start();
		} 
	}
	private void _on_timer_timeout()
	{
		_timer.Stop();
		_timer.WaitTime = _powerUpTimer;
		SpawnNewPowerUp();
	}

	private void SpawnNewPowerUp()
	{
		uint ranNum = (GD.Randi() % 4 + 1);
		if (ranNum == 1)
		{
			_healthSpawnCount++;
			if(_healthSpawnCount >= _maxHealthSpawns)
				ranNum = (GD.Randi() % 3 + 2);
		}
		GD.Print(ranNum);
		switch (ranNum)
		{
			case 1: SetupPowerup(PowerupType.Burger);
				break;
			case 2: SetupPowerup(PowerupType.Energy);
				break;
			case 3: SetupPowerup(PowerupType.Megaphone);
				break;
			case 4: SetupPowerup(PowerupType.Whiskey);
				break;
		}
	}
	private void SetupPowerup(PowerupType powerupType)
	{
		Powerup spawnedPowerup = _powerUpScene.Instantiate<Powerup>();
		AddChild(spawnedPowerup);
		spawnedPowerup.SetType(powerupType);
	}
}


