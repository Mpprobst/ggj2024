using Godot;
using Godot.Collections;
using System;

public partial class LevelManager : Node2D
{
	private Array<Node> SpawnLocations;
	private Array<Sequence> Sequences;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SpawnLocations = GetNode("SpawnPoints").GetChildren();
		var sequenceNodes = GetNode("Sequences").GetChildren();
		Sequences = new Array<Sequence>();
		foreach (var seq in sequenceNodes)
			Sequences.Add(seq as Sequence);
			
		//GD.Print($"found {Sequences.Count} sequences");
		
		var timer = GetNode<Timer>("Timer");
		timer.Timeout += StartGame;
		timer.Start(1f);
	} 

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void StartGame()
	{
		// Play the sequencer
		Sequences[0].StartSequence(80f);
	}
	
	public Vector2 GetSpawnPos(int spawnerID)
	{
		if (spawnerID < 0 || spawnerID >= SpawnLocations.Count) 
			return Vector2.Zero;
		return (SpawnLocations[spawnerID] as Node2D).GlobalPosition;
	}
}
