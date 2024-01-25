using Godot;
using System;
using System.Collections.Generic;

public partial class MenuManager : Control
{
	Dictionary<string, Menu> menus = new Dictionary<string, Menu>();
	Stack<string> history = new Stack<string>();
	[Export]
	public PackedScene GameScene;
	// Node gameHolder;
	SubViewport gameHolder;
	Node game;
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
		SetMenu(startName);
		GetTree().Paused = true;
		gameHolder = GetNode<SubViewport>("SubViewportContainer/SubViewport");
		var dims = GetViewport().GetVisibleRect().Size;
		gameHolder.Size = new Vector2I((int)dims.X, (int)dims.Y);
	}
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause")){
			if(GetTree().Paused){
				if(game is not null){ // only unpause if there is a game actually running
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
		var camera = GetViewport().GetCamera2D();
		if(camera is not null) {
			var viewport = GetViewportRect();
			currentMenu.Position = camera.GlobalPosition - viewport.Size / 2;
		}
		else{
			currentMenu.Position = Vector2.Zero;
		}
		// currentMenu.Position = viewport.Position;
	}
	private void _on_play_button_button_down()
	{
		game = GameScene.Instantiate();
		GetTree().Paused = false;
		gameHolder.AddChild(game);
		HideMenus();
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
		// cleanup game
		if(game is not null){
			gameHolder.RemoveChild(game);
			game.QueueFree();
		}
	}
	private void _on_resume_button_button_down()
	{
		HideMenus();
		GetTree().Paused = false;
	}
}
