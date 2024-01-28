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
	const float RESPAWN_TIME = 1.2f;
	const int RESPAWN_DAMAGE = 2;
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		var participant = other.Components.GetInAncestorsOrSelf<RaceParticipant>();
		if(participant != null)
		{
			Notifications.Add( participant, new( "Vehicle out of bounds, respawning...", UIColors.Notification.Danger, RESPAWN_TIME, "warning" ) );
			participant.RespawnIn(RESPAWN_TIME, RESPAWN_DAMAGE);
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
	}
}
