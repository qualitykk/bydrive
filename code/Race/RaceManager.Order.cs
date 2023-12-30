using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redrome;

public partial class RaceManager
{
	private Dictionary<RaceCheckpoint, int> checkpointOrder = new();
	private int maxCheckpointOrder = 0;
	public void OrderCheckpoints()
	{
		if ( StartCheckpoint == null )
		{
			Log.Warning( "Race Manager requires starting checkpoint, running without race manager!" );
			return;
		}

		checkpointOrder.Clear();
		checkpointOrder.Add( StartCheckpoint, 0 );

		List<RaceCheckpoint> nextCheckpoints = StartCheckpoint.NextCheckpoints;
		int currentOrder = 1;
		bool foundStart = false;
		while ( nextCheckpoints.Any() )
		{
			List<RaceCheckpoint> pointsToContinue = nextCheckpoints.ToList();
			for ( int i = 0; i < nextCheckpoints.Count; i++ )
			{
				RaceCheckpoint checkpoint = nextCheckpoints[i];

				if ( checkpoint == StartCheckpoint )
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
			Log.Info( "Checkpoints Ordered!" );
			maxCheckpointOrder = checkpointOrder.Values.Max();
		}
	}
}
