using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class RaceManager
{
	private Dictionary<TrackCheckpoint, int> checkpointOrder = new();
	private int maxCheckpointOrder = 0;
	public void OrderCheckpoints()
	{
		if ( StartCheckpointOptions == null )
		{
			Log.Warning( "Race Manager requires starting checkpoint, running without race manager!" );
			return;
		}

		checkpointOrder.Clear();
		checkpointOrder.Add( GetStartCheckpoint(), 0 );

		List<TrackCheckpoint> nextCheckpoints = GetStartCheckpoint().NextCheckpoints;
		Assert.NotNull(nextCheckpoints, $"Cant have race without checkpoints!");
		int currentOrder = 1;
		bool foundStart = false;
		while ( nextCheckpoints.Any() )
		{
			List<TrackCheckpoint> pointsToContinue = nextCheckpoints.ToList();
			for ( int i = 0; i < nextCheckpoints.Count; i++ )
			{
				TrackCheckpoint checkpoint = nextCheckpoints[i];

				if ( checkpoint == GetStartCheckpoint() )
				{
					pointsToContinue.Remove( checkpoint );
					foundStart = true;
					break;
				}

				if ( checkpointOrder.TryGetValue( checkpoint, out int storedOrder ) )
				{
					if ( storedOrder < currentOrder )
						checkpointOrder[checkpoint] = currentOrder;

					pointsToContinue.Remove( checkpoint );
				}
				else
				{
					checkpointOrder.Add( checkpoint, currentOrder );
				}
			}

			currentOrder++;
			nextCheckpoints = pointsToContinue.SelectMany( p => p.NextCheckpoints ).ToList();
		}

		if ( !foundStart )
		{
			Log.Warning( "Could not complete checkpoint ordering, checkpoints dont form a loop!" );
			return;
		}
		else
		{
			maxCheckpointOrder = checkpointOrder.Values.Max();
		}
	}
}
