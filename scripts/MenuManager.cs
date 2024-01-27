using Godot;
using System;
using System.Collections.Generic;

public partial class MenuManager : Control
{
	Dictionary<string, Menu> menus = new Dictionary<string, Menu>();
	Stack<string> history = new Stack<string>();
	[Export]
	public PackedScene GameScene;
	[Export]
	public PackedScene ViewScene;
	// Node gameHolder;
	Control viewContainer;
	Node game;
	int playerCount = 1;
	public override void _Ready()
	{
		// scan for child menu nodes
		string startName = null;
		foreach (var childNode in GetChildren())
		{
			if(childNode is Menu child){
				if(startName is null) startName = child.name;
				menus.Add(child.name, child);
			}
		}
		if(startName is null){
			GD.PrintErr("No valid menu children detected!");
			return;
		}
		viewContainer = GetNode<Control>("ViewContainer");
		SetMenu(startName);
		GetTree().Paused = true;
	}
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause")){
			if(GetTree().Paused){
				if(viewContainer.GetChildren().Count > 0){ // only unpause if there is a game actually running
					HideMenus();
					GetTree().Paused = false;
				}
			}
			else{
				GetTree().Paused = true;
				SetMenu("pause");
			}
		}
	}
	void HideMenus(){
		foreach (var menu in menus.Values)
		{
			menu.Visible = false;
		}
	}
	public void SetMenu(string name){
		if(!menus.ContainsKey(name)){
			GD.PrintErr($"Invalid menu name '{name}'!");
			return;
		}
		history.Push(name);
		HideMenus();
		var currentMenu = menus[name];
		currentMenu.Visible = true;
		currentMenu.Focus();
	}
	void Launch(){
		HideMenus();
		var game = GameScene.Instantiate<Game>();
		var players = game.GetPlayerCars(playerCount);
		var views = new List<View>();
		List<Rect2> dimensions = new List<Rect2>();
		var windowSize = GetWindow().Size;
		Vector2 viewSize;
		switch (playerCount)
		{
			case 1:
			dimensions = new List<Rect2>{
				new Rect2(0,0,windowSize),
			};
			break;
			case 2:
			viewSize = new Vector2(windowSize.X / 2, windowSize.Y);
			dimensions = new List<Rect2>{
				new Rect2(0,0,viewSize),
				new Rect2(viewSize.X,0,viewSize),
			};
			break;
			case 3:
			viewSize = new Vector2(windowSize.X / 2, windowSize.Y / 2);
			dimensions = new List<Rect2>{
				new Rect2(0,0,viewSize),
				new Rect2(viewSize.X,0,viewSize),
				new Rect2(0,viewSize.Y,viewSize),
			};
			break;
			case 4:
			viewSize = new Vector2(windowSize.X / 2, windowSize.Y / 2);
			dimensions = new List<Rect2>{
				new Rect2(0,0,viewSize),
				new Rect2(viewSize.X,0,viewSize),
				new Rect2(0,viewSize.Y,viewSize),
				new Rect2(viewSize.X,viewSize.Y,viewSize),
			};
			break;
			default:
			GD.PrintErr("Invalid number of players");
			break;
		}
		for (int idx = 0; idx < playerCount; idx++)
		{
			var view = ViewScene.Instantiate<View>();
			viewContainer.AddChild(view);
			views.Add(view);
			players[idx].SetView(view);
			if(idx == 0){
				view.AddGame(game);
			}
			else{
				view.GetSubViewport().World2D = views[0].GetSubViewport().World2D;
			}
			var dimension = dimensions[idx];
			view.Position = dimension.Position;
			view.SetDeferred("size", dimension.Size);
			view.GetSubViewport().Size = (Vector2I)dimension.Size;
		}
		GetTree().Paused = false;
	}
	void EndGame(){
		foreach (var node in viewContainer.GetChildren())
		{
			node.QueueFree();
		}
		GetTree().Paused = true;
	}
	private void _on_play_button_button_down()
	{
		SetMenu("mode_menu");
	}
	private void _on_quit_button_button_down()
	{
		GetTree().Quit();
	}
	private void _on_options_button_button_down()
	{
		SetMenu("options");
	}
	private void _on_back_button_button_down()
	{
		if(history.Count < 2) return;
		history.Pop();
		var name = history.Pop();
		SetMenu(name);
	}
	private void _on_quit_to_title_button_down()
	{
		SetMenu("main");
		EndGame();
	}
	private void _on_resume_button_button_down()
	{
		HideMenus();
		GetTree().Paused = false;
	}
	private void _on_singleplayer_button_down()
	{
		// Replace with function body.
		playerCount = 1;
		Launch();
	}
	private void _on_two_player_button_down()
	{
		// Replace with function body.
		playerCount = 2;
		Launch();
	}
	private void _on_three_player_button_down()
	{
		// Replace with function body.
		playerCount = 3;
		Launch();
	}
	private void _on_four_player_button_down()
	{
		// Replace with function body.
		playerCount = 4;
		Launch();
	}
}



