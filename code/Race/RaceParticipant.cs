using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon("timer")]
[Alias("RaceCompletion")]
public class RaceParticipant : Component
{
	/// <summary>
	/// Name for the racer displayed on the UI, auto-generates name if empty
	/// </summary>
	[Property] public string DisplayName { get; set; }
	/// <summary>
	/// Bypass key checkpoint checks
	/// </summary>
	[Property] public bool BypassChecks { get; set; } = false;
	public RaceCheckpoint LastKeyCheckpoint { get; private set; }
	/// <summary>
	/// Last checkpoint key or non-key checkpoint, used to determine standings in the race.
	/// </summary>
	public RaceCheckpoint LastCheckpoint { get; private set; }
	public List<RaceCheckpoint> NextKeyCheckpoints { get; private set; } = new();
	public List<RaceCheckpoint> PreviousKeyCheckpoints { get; private set; } = new();
	public float GetCompletion() => Race?.GetParticipantCompletion( this ) ?? 0f;
	public int GetLap() => Race?.GetParticipantLap( this ) ?? 0;
	protected override void OnAwake()
	{
		if(string.IsNullOrEmpty(DisplayName))
		{
			DisplayName = GenerateName();
		}
	}
	public void Respawn()
	{
		if(LastKeyCheckpoint != null)
		{
			Transform.World = LastKeyCheckpoint.GetWorldRespawn();
			/*
			if(Components.TryGet(out Rigidbody body, FindMode.EnabledInSelfAndChildren))
			{
				body.Velocity = Vector3.Zero;
				body.AngularVelocity = Vector3.Zero;
			}
			*/
		}
	}
	public void PassCheckpoint(RaceCheckpoint checkpoint, bool forceLast = false)
	{
		if(!CanPass(checkpoint) && !forceLast)
		{
			return;
		}

		if(checkpoint.IsRequired)
		{
			LastKeyCheckpoint = checkpoint;
			NextKeyCheckpoints = FindNextKeyCheckpoints( checkpoint );
			PreviousKeyCheckpoints = FindPreviousKeyCheckpoints( checkpoint );
		}

		Race.CheckpointPassed( this, checkpoint );
		LastCheckpoint = checkpoint;
	}

	private List<RaceCheckpoint> FindNextKeyCheckpoints(RaceCheckpoint checkpoint)
	{
		if ( !checkpoint.NextCheckpoints.Any() )
			return new();

		List<RaceCheckpoint> keyCheckpoints = new();

		foreach(var next in checkpoint.NextCheckpoints)
		{
			if(next.IsRequired)
			{
				keyCheckpoints.Add(next);
			}
			else
			{
				keyCheckpoints.AddRange( FindNextKeyCheckpoints( next ) );
			}
		}

		return keyCheckpoints;
	}

	private List<RaceCheckpoint> FindPreviousKeyCheckpoints( RaceCheckpoint checkpoint )
	{
		if ( !checkpoint.PreviousCheckpoints.Any() )
			return new();

		List<RaceCheckpoint> keyCheckpoints = new();

		foreach ( var previous in checkpoint.PreviousCheckpoints )
		{
			if ( previous.IsRequired )
			{
				keyCheckpoints.Add( previous );
			}
			else
			{
				keyCheckpoints.AddRange( FindPreviousKeyCheckpoints( previous ) );
			}
		}

		return keyCheckpoints;
	}

	private bool CanPass(RaceCheckpoint checkpoint)
	{
		if ( BypassChecks ) return true;

		return !checkpoint.IsRequired || 
			(NextKeyCheckpoints.Any() && NextKeyCheckpoints.Contains(checkpoint)) || 
			(PreviousKeyCheckpoints.Any() && PreviousKeyCheckpoints.Contains(checkpoint));
	}
	internal void OnFinished()
	{
		// Do we want to do something here?
	}

	protected override void DrawGizmos()
	{
		const float TEXT_OFFSET = 8f;
		const float TEXT_SPACE = 32f;
		const float TEXT_SIZE = 14f;
		Color textColor = Color.Green;

		Gizmo.Draw.Color = textColor;
		Gizmo.Draw.Text(DisplayName, new(Vector3.Up * TEXT_OFFSET), size: TEXT_SIZE);

		if(Race != null)
		{
			Gizmo.Draw.Text( $"Completion: {GetCompletion()}", new( Vector3.Up * (TEXT_OFFSET + TEXT_SPACE) ), size: TEXT_SIZE );
			Gizmo.Draw.Text( $"LastCheckpoint: {LastCheckpoint.GameObject.Name}", new( Vector3.Up * (TEXT_OFFSET + TEXT_SPACE * 2)), size: TEXT_SIZE );
		}
	}

	int botNumber = 1;
	private string GenerateName()
	{
		string name = $"Bot {botNumber}";
		botNumber++;

		return name;
	}
}
