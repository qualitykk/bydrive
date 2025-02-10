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
	[Property] public SoundEvent PickupSound { get; set; }
	[Property, Title("On Available")] public Action OnAvailableAction { get; set; }
	[Property, Title( "On Unavailable" )] public Action OnUnavailableAction { get; set; }
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
		OnAvailableAction?.Invoke();
	}
	protected virtual void OnBecomeUnavailable()
	{
		OnUnavailableAction?.Invoke();
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( !Available )
			return;

		var vehicle = other.Components.GetInAncestorsOrSelf<VehicleController>();
		if ( vehicle == null || !OnPickup( vehicle ) )
			return;

		if(PickupSound != default)
		{
			SoundManager.Instance.Play( PickupSound, GameSoundChannel.Effect, WorldPosition );
		}

		Available = false;
		OnBecomeUnavailable();
		TimeSincePickup = 0;
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
	}

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Color.Yellow;

		Gizmo.Draw.Text( Available ? "Available" : $"Respawning in {MathF.Ceiling(RespawnTime - TimeSincePickup)}", global::Transform.Zero );
	}
}
