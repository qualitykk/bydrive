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
	const string UNSET_NAME_PLACEHOLDER = "Player";
	[Property] public string DisplayName { get; set; } = UNSET_NAME_PLACEHOLDER;
	/// <summary>
	/// Bypass key checkpoint checks
	/// </summary>
	[Property] public bool BypassChecks { get; set; } = false;
	public RaceCheckpoint LastCheckpoint { get; private set; }
	List<RaceCheckpoint> NextCheckpoints { get; set; } = new();
	public float GetCompletion() => Race?.GetRaceCompletion( this ) ?? 0f;

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

	protected override void DrawGizmos()
	{
		base.DrawGizmos();
	}
}
