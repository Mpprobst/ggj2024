using Godot;
using System;
using System.Threading.Tasks;

public partial class Pad : Area2D
{
	[Export] Color[] Colors;
	
	private float Lifetime;
	private bool IsReady;
	
	private Timer timer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.BodyEntered += OnPadBodyEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{		
		//var timer = GetNode<Timer>("Timer");
		//GD.Print($"timer: {timer.TimeLeft}");
	}
	
	//[Signal] public delegate void OnPadCompleteEventHandler(bool success);
	
	
	public async void Initialize(float lifetime, float delay)
	{
		GD.Print("Init pad in ", delay);
		IsReady = false;
		Lifetime = lifetime;
		
		Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Visible = false;
		//await ToSignal(GetTree().CreateTimer(delay), "timeout");
		int ms = Mathf.RoundToInt(delay * 1000);
		await Task.Delay(ms);
		Enable();
		return;
		
		if (delay == 0)
		{
			Enable();
			return;
		}
		
		timer = GetNode<Timer>("Timer");
		timer.Timeout += Enable;
		timer.Start(delay);
	}
	
	private async void Enable()
	{
		IsReady = true;
		Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
		Color randColor = Colors[GD.Randi() % Colors.Length];
		sprite.Modulate = randColor;
		sprite.Visible = true;
		
		GD.Print("pad enabled for ", Lifetime);
		int ms = Mathf.RoundToInt(Lifetime * 1000);
		await Task.Delay(ms);
		
		//await ToSignal(GetTree().CreateTimer(Lifetime), "timeout");
		OnTimerTimeout();
		return;
		//var timer = GetNode<Timer>("Timer");
		timer.Timeout -= Enable;
		timer.Timeout += OnTimerTimeout;
		timer.Stop();
		timer.Start(Lifetime);
	}

	private void OnPadBodyEntered(Node2D body)
	{
		PlayerController pc = body as PlayerController;
		if (pc != null && IsReady)
		{
			Kill(true);
		}
	}
	
	private void OnTimerTimeout()
	{

			Kill(false);
	}
	
	// also kill if it has been alive for too long
	public void Kill(bool success)
	{
		// NEED TO TELL THE SEQUENE OF THE STATUS
		if (!IsReady) return;
		IsReady = false;
		GD.Print("kill " + Name);
		Sequence seq = GDExtensions.GetParentOfType<Sequence>(this);//  GetParent().GetNode<Sequence>("Sequence");
		seq.PadComplete(success);
		//EmitSignal(SignalName.OnPadComplete, success);
		QueueFree();
	}
}
