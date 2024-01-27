using Godot;
using System;
using Godot.Collections;

public partial class PlayerController : CharacterBody2D
{
	public const float Speed = 10000.0f;
	public const float Acceleration = 250.0f;
	private Vector2 CurrDirection;
	private bool IsStopped = false;
	private bool IsFlying = true;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public PlayerController()
	{
		GD.Print("Hello world!");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero && !IsStopped) 
		{
			CurrDirection = direction;
		}

		Velocity = Velocity.MoveToward(CurrDirection * Speed, Acceleration);
		//MoveAndSlide();
		
		var collision = MoveAndCollide(Velocity * (float)delta);
		
		if (collision != null)
		{
			if (IsFlying)	// hit a wall
			{
				HitWall(collision);
			}
			else if (!IsStopped)	// on wall, hit something
			{
				HitCorner();
			}
		}
		else if (IsStopped)
		{
			IsStopped = false;
			IsFlying = true;
		}
	}
	
	private void HitWall(KinematicCollision2D collision)
	{
		//GD.Print("collide ", ((Node)collision.GetCollider()).Name);
		// just needs to go in the opposite dimension as the wall, but with
		Vector2 hit = collision.GetPosition();	// PLAYER COLLIDER MUST BE ROTATED 45 deg for this to work
		Vector2 diff = hit - GlobalPosition;
		// now swap diff x,y
		float x = Mathf.Abs(diff.X);
		float y = Mathf.Abs(diff.Y);
		diff = Vector2.Zero;
		if (x < y)
			diff.X = 1;
		else
			diff.Y = 1;
		
		// only care about the velocity in this direction
		Velocity = Velocity * diff;

		CurrDirection = new Vector2(Mathf.Clamp(Velocity.X, -1, 1), Mathf.Clamp(Velocity.Y, -1, 1));
		IsFlying = false;
	}
	
	private void HitCorner()
	{
		CurrDirection = Vector2.Zero;
		Velocity = Vector2.Zero;
		IsStopped = true;
		IsFlying = false;
	}

}
