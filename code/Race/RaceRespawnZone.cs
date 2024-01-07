using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Group("Race")]
[Icon( "rv_hookup" )]
[Alias("ParticipantRespawnZone", "RaceParticipantRespawnZone")]
public class RaceRespawnZone : Component, Component.ITriggerListener
{
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		var participant = other.Components.GetInAncestorsOrSelf<RaceParticipant>();
		if(participant != null)
		{
			participant.Respawn();
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
	}
}
