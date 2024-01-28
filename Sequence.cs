using Godot;
using System;
using System.Threading.Tasks;

public partial class Sequence : Node
{
	// probably just an array of numbers and a float to know how long to keep the pad alive

	[Export] private int[] Goals;	// if goal ID is outside bounds, then it is considered a rest
	[Export] private float Lifetimes = 1f;
	
	private int goalCt;
	private int padCt;
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
		goalCt = 0;
		padCt = 0;
		BPM = bpm;
		float noteLength = 60f / BPM;
		GD.Print("start sequence at ", bpm);
		PackedScene PadResource = GD.Load<PackedScene>("res://pad.tscn");

		int playerPos = 0;
		PlayerController player = GDExtensions.GetChildOfType<PlayerController>(GetTree().Root);

		Vector2 corner = player.Position;//new Vector2(Mathf.Clamp(player.Position.X, -1, 1), Mathf.Clamp(player.Position.Y, -1, 1));
		if (corner.X < 0 && corner.Y < 0)
			playerPos = 0;
		if (corner.X > 0 && corner.Y < 0)
			playerPos = 2;
		if (corner.X > 0 && corner.Y > 0)
			playerPos = 4;
		if (corner.X < 0 && corner.Y > 0)
			playerPos = 6;
		
		// prevents first point spawning on the player
		int goalOffset = 0;
		if (Goals[0] == playerPos)
			goalOffset = 2;
		
		LevelManager levelManager = GDExtensions.GetParentOfType<LevelManager>(this);
		for (int i = 0; i < Goals.Length; i++)
		{
			// add offset relative to player
			int g = Goals[i];
			if (g >= 0)		// preserves -1 as a rest
				g += playerPos;
			Vector2 pos = levelManager.GetSpawnPos(g);
			if (pos == Vector2.Zero) continue;

			Pad pad = PadResource.Instantiate<Pad>();
			pad.Position = pos;
			float delay = (i * noteLength) + noteLength * 2f;
			//pad.OnPadCompleteEventHandler += PadComplete;
			pad.Initialize(noteLength, delay);
			
			//GetParent().CallDeffered(Node.MethodName.AddChild, pad);
			AddChild(pad);
		}
	}
	
	public void PadComplete(bool success)
	{
		padCt += 1;
		if (success)
		{
			goalCt++;
		}
		
		if (padCt >= Goals.Length)
		{
			if (goalCt >= Goals.Length)
				Finish();
			else  // restart
				StartSequence(BPM);
		}
	}
	
	private void Finish()
	{
		GD.Print("Sequence complete!");
		LevelManager levelManager = GDExtensions.GetParentOfType<LevelManager>(this);// GetNode<LevelManager>("LevelManager");
		levelManager.PlayNextSequence();
	}
}
