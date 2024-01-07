using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public abstract class RacerPickup : Component, Component.ITriggerListener
{
	public const float DEFAULT_RESPAWN_TIME = 10f;
	[Property] public float RespawnTime { get; set; } = DEFAULT_RESPAWN_TIME;
	public bool Available { get; set; } = true;
	public TimeSince TimeSincePickup { get; set; }
	public abstract bool OnPickup( VehicleController vehicle );
	protected override void OnUpdate()
	{
		if(!Available)
		{
			if(TimeSincePickup > RespawnTime)
			{
				Available = true;
				OnBecomeAvailable();
			}
		}
	}
	protected virtual void OnBecomeAvailable()
	{
	}
	protected virtual void OnBecomeUnavailable()
	{
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( !Available )
			return;

		var vehicle = other.Components.GetInAncestorsOrSelf<VehicleController>();
		if ( !OnPickup( vehicle ) )
			return;

		Available = false;
		TimeSincePickup = 0;
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
	}
}
