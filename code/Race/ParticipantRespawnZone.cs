using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon( "rv_hookup" )]
public class ParticipantRespawnZone : Component, Component.ITriggerListener
{
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if(other.Components.TryGet(out RaceParticipant participant))
		{
			participant.Respawn();
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
	}
}
