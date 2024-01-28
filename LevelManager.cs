using Godot;
using Godot.Collections;
using System;

public partial class LevelManager : Node2D
{
	private Array<Node> SpawnLocations;
	private Array<Sequence> Sequences;
	
	int CurrSequence;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SpawnLocations = GetNode("SpawnPoints").GetChildren();
		var sequenceNodes = GetNode("Sequences").GetChildren();
		Sequences = new Array<Sequence>();
		foreach (var seq in sequenceNodes)
			Sequences.Add(seq as Sequence);

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
		CurrSequence = 0;
		Sequences[0].StartSequence(Game.BPM);
	}
	
	public void PlayNextSequence()
	{
		CurrSequence += 1;
		if (CurrSequence % 2 == 0)
				Game.BPM += 10f;
		if (CurrSequence < Sequences.Count)
			Sequences[CurrSequence].StartSequence(Game.BPM);
		else
			GD.Print("Game Over!");
	}
	
	public Vector2 GetSpawnPos(int spawnerID)
	{
		if (spawnerID < 0) 
			return Vector2.Zero;
		if (spawnerID >= SpawnLocations.Count)
			spawnerID = spawnerID % SpawnLocations.Count;
		return (SpawnLocations[spawnerID] as Node2D).GlobalPosition;
	}
}
