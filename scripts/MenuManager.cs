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
	[Export] public MapWrapper[] Maps;
	[Export] public bool Censored = false;
	float ScreenShakeIntensity = 1;
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
		SetMenu("title");
		GetTree().Paused = true;
		var levelSelect = GetNode("MapSelect");
		for(int idx = 0; idx < Maps.Length; idx++)
		{
			var button = new Button
			{
				Text = Maps[idx].MapName
			};
			int id = idx;
			button.Pressed += delegate(){LoadMap(id);};
			levelSelect.AddChild(button);
		}
		levelSelect.MoveChild(levelSelect.GetNode("BackButton"), -1);
	}
	private void LoadMap(int mapIndex){
		GD.Print(mapIndex); 
		GameScene = Maps[mapIndex].MapScene;
		Launch();
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
		if(name == "options"){
			currentMenu.GetNode<CheckBox>("CheckButton").ButtonPressed = Censored;
		}
	}
	void Launch(){
		HideMenus();
		var game = GameScene.Instantiate<Game>();
		var players = game.GetPlayerCars(playerCount);
		var views = new List<View>();
		List<Rect2> dimensions = new List<Rect2>();
		var windowSize = GetWindow().Size;
		Vector2 viewSize;
		float borderThickness = 10;
		switch (playerCount)
		{
			case 1:
			dimensions = new List<Rect2>{
				new Rect2(0,0,windowSize),
			};
			break;
			case 2:
			viewSize = new Vector2(windowSize.X / 2 - borderThickness, windowSize.Y);
			dimensions = new List<Rect2>{
				new Rect2(0,0,viewSize),
				new Rect2(viewSize.X + borderThickness,0,viewSize),
			};
			break;
			case 3:
			viewSize = new Vector2(windowSize.X / 2 - borderThickness, windowSize.Y / 2 - borderThickness);
			dimensions = new List<Rect2>{
				new Rect2(0,0,viewSize),
				new Rect2(viewSize.X + borderThickness,0,viewSize),
				new Rect2(0,viewSize.Y + borderThickness,viewSize),
			};
			break;
			case 4:
			viewSize = new Vector2(windowSize.X / 2 - borderThickness, windowSize.Y / 2 - borderThickness);
			dimensions = new List<Rect2>{
				new Rect2(0,0,viewSize),
				new Rect2(viewSize.X + borderThickness,0,viewSize),
				new Rect2(0,viewSize.Y + borderThickness,viewSize),
				new Rect2(viewSize.X + borderThickness,viewSize.Y+borderThickness,viewSize),
			};
			break;
			default:
			GD.PrintErr("Invalid number of players");
			break;
		}
		for (int idx = 0; idx < playerCount; idx++)
		{
			var view = ViewScene.Instantiate<View>();
			var dimension = dimensions[idx];
			viewContainer.AddChild(view);
			view.SetDimensions(dimension.Size);
			views.Add(view);
			view.SetPlayer(players[idx]);
			players[idx].ShakeIntensity = ScreenShakeIntensity;
			if(idx == 0){
				view.AddGame(game);
			}
			else{
				view.GetSubViewport().World2D = views[0].GetSubViewport().World2D;
			}
			// var dimension = dimensions[idx];
			view.Position = dimension.Position;
			view.SetSize(dimension.Size);
			// view.ViewSize = dimension.Size;
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
	public void Won(string text){
		EndGame();
		GetNode<Label>("ResultsScreen/Results").Text = text;
		SetMenu("results");
	}
	public void Lost(){
		EndGame();
		SetMenu("loss");
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
		SetMenu("map_select");
	}
	private void _on_two_player_button_down()
	{
		// Replace with function body.
		playerCount = 2;
		SetMenu("map_select");
	}
	private void _on_three_player_button_down()
	{
		// Replace with function body.
		playerCount = 3;
		SetMenu("map_select");
	}
	private void _on_four_player_button_down()
	{
		// Replace with function body.
		playerCount = 4;
		SetMenu("map_select");
	}
	void SetBusVolume(string busName, double value){
		var busId = AudioServer.GetBusIndex(busName);
		var db = Math.Log10(value / 100) * 10;
		AudioServer.SetBusVolumeDb(busId, (float)db);
	}
	private void _on_main_audio_slider_value_changed(double value)
	{
		SetBusVolume("Master", value);
	}
	private void _on_music_slider_value_changed(double value)
	{
		SetBusVolume("Music", value);
	}


	private void _on_sfx_slider_value_changed(double value)
	{
		SetBusVolume("Sfx", value);
	}


	private void _on_voice_slider_value_changed(double value)
	{
		SetBusVolume("Voice", value);
	}
	private void _on_screen_shake_slider_value_changed(double value)
	{
		// Replace with function body.
		ScreenShakeIntensity = (float)value / 100;
		// change player cars if any exist
		foreach (var node in GetTree().GetNodesInGroup("Player")){
			if(node is PlayerCar player) player.ShakeIntensity = ScreenShakeIntensity;
		}
	}
	private void _on_start_button_button_down()
	{
		// Replace with function body.
		SetMenu("main");
	}
}
