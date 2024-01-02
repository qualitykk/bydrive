using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redrome;

[Icon("timer")]
[Alias("RaceCompletion")]
public class RaceParticipant : Component
{
	public const string UNSET_NAME_PLACEHOLDER = "Player";
	/// <summary>
	/// Name for the racer displayed on the UI, auto-generates name if empty
	/// </summary>
	[Property] public string DisplayName { get; set; } = UNSET_NAME_PLACEHOLDER;
	/// <summary>
	/// Bypass key checkpoint checks
	/// </summary>
	[Property] public bool BypassChecks { get; set; } = false;
	public RaceCheckpoint LastCheckpoint { get; private set; }
	List<RaceCheckpoint> NextCheckpoints { get; set; } = new();
	public float GetCompletion() => Race?.GetRaceCompletion( this ) ?? 0f;
	protected override void OnAwake()
	{
		if(string.IsNullOrEmpty(DisplayName))
		{
			DisplayName = GenerateName();
		}
	}
	int botNumber = 1;
	private string GenerateName()
	{
		string name = $"Bot {botNumber}";
		botNumber++;

		return name;
	}
	public void PassCheckpoint(RaceCheckpoint checkpoint, bool forceLast = false)
	{
		if(!CanPass(checkpoint) && !forceLast)
		{
			return;
		}

		LastCheckpoint = checkpoint;
		NextCheckpoints = checkpoint.NextCheckpoints;
		Race.CheckpointPassed(this, checkpoint);
		//Log.Info( $"Pass checkpoint {checkpoint}" );
	}

	private bool CanPass(RaceCheckpoint checkpoint)
	{
		if ( BypassChecks ) return true;

		return NextCheckpoints.Any() && NextCheckpoints.Contains(checkpoint);
	}
	internal void OnFinished()
	{
		// Do we want to do something here?
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();
	}

}
