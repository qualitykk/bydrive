﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class RaceManager
{
	
	public IReadOnlyList<RaceParticipant> ParticipantPlacements => completionOrderedParticipants;
	public int LastPlace => Participants.Count;

	private Dictionary<RaceParticipant, float> participantRaceCompletion = new();
	private Dictionary<RaceParticipant, int> participantLastOrder = new();
	private List<ParticipantFinishInformation> finishedParticipants = new();
	public float GetParticipantCompletion( RaceParticipant participant )
	{
		if ( !participant.IsValid() )
		{
			return 0f;
		}

		if ( !participantRaceCompletion.TryGetValue( participant, out float completion ) )
		{
			return 0f;
		}

		return completion;
	}
	public int GetParticipantLap(RaceParticipant participant)
	{
		if ( !participant.IsValid() )
		{
			return 0;
		}

		if ( !participantRaceCompletion.TryGetValue( participant, out float progression ) )
		{
			return 0;
		}

		const float PROGRESSION_FORGIVENESS = 0.00001f;

		int lap = progression.SnapToGrid( PROGRESSION_FORGIVENESS ).FloorToInt() + 1;
		return lap.Clamp(1, RaceConfig.MaxLaps);
	}
	public int GetParticipantPlacement(RaceParticipant participant)
	{
		return completionOrderedParticipants.IndexOf( participant ) + 1;
	}


	public void UpdateCompletion()
	{
		float singleCheckpointFraction = 1 / ((float)maxCheckpointOrder + 1);
		foreach ( var participant in Participants )
		{
			RaceCheckpoint checkpoint = participant.LastCheckpoint;
			if ( checkpoint == null || !checkpointOrder.TryGetValue( checkpoint, out int order ) || !participantLastOrder.TryGetValue(participant, out int lastOrder) )
			{
				// Participant either hasnt passed any checkpoints or passed checkpoints arent valid this race.
				continue;
			}

			if(order != lastOrder)
			{
				int orderDelta = order - lastOrder;
				if(lastOrder == 0 && order == maxCheckpointOrder || order == 0 && lastOrder == maxCheckpointOrder)
				{
					// Special case for going over the finish line
					orderDelta = -MathF.Sign(orderDelta);
				}

				participantRaceCompletion[participant] += singleCheckpointFraction * orderDelta;
				participantLastOrder[participant] = order;
				UpdateTimeSplits( participant);
			}

			if ( IsFinished( participant ) && !finishedParticipants.Any( f => f.Participant == participant ) )
			{
				ParticipantFinished( participant );
			}
		}

		completionOrderedParticipants.Clear();
		foreach(var participant in Participants.OrderByDescending(GetParticipantCompletion).ThenBy(ClosestKeyCheckpointDistance))
		{
			completionOrderedParticipants.Add( participant );
		}
	}

	/// <summary>
	/// Gets 
	/// </summary>
	/// <param name="participant"></param>
	/// <returns></returns>
	private float ClosestKeyCheckpointDistance(RaceParticipant participant)
	{
		Vector3 position = participant.Transform.Position;
		var next = participant.NextKeyCheckpoints;
		return next.Min( n => n.Transform.Position.DistanceSquared( position ) );
	}

}
