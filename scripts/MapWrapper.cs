using Godot;

[Tool]
public partial class MapWrapper : Resource
{
	[Export] public string MapName { get; set; } = "[default name]";
    [Export] public PackedScene MapScene { get; set; }
	
}
