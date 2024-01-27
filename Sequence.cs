using Godot;
using System;

public partial class Sequence : Node
{
	// probably just an array of numbers and a float to know how long to keep the pad alive

	[Export] private int[] Goals;	// if goal ID is outside bounds, then it is considered a rest
	[Export] private float Lifetimes = 1f;
	
	private int goalCt;
	private float BPM;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void StartSequence(float bpm)
	{
		GD.Print("start sequence at ", bpm);
		// clean up spawned pads
		BPM = bpm;
		PackedScene PadResource = GD.Load<PackedScene>("res://pad.tscn");
		
		LevelManager levelManager = GDExtensions.GetParentOfType<LevelManager>(this);// GetNode<LevelManager>("LevelManager");
		float noteLength = 60f / BPM;
		for (int i = 0; i < Goals.Length; i++)
		{
			Vector2 pos = levelManager.GetSpawnPos(Goals[i]);
			if (pos == Vector2.Zero) continue;
			
			Pad pad = PadResource.Instantiate<Pad>();
			pad.Position = pos;
			float delay = i * noteLength + 1f;
			//pad.OnPadCompleteEventHandler += PadComplete;
			pad.Initialize(noteLength, delay);
			
			AddChild(pad);

		}
		// Pad.Initialize(Lifetimes, delay);
	}
	
	public void PadComplete(bool success)
	{
		if (success)
		{
			goalCt++;
			if (goalCt >= Goals.Length)
				Finish();
		}
		else
		{
			// restart
			StartSequence(BPM);
		}
	}
	
	private void Finish()
	{
		GD.Print("Sequence complete!");
	}
}
