using Sandbox.Engine;
using Sandbox.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Category("Race")]
[EditorHandle( "materials/gizmo/charactercontroller.png" )]
[Icon( "flag" )]
public sealed partial class RaceManager : Component
{
	public static RaceManager Current { get; private set; }
	[Property] public RaceCheckpoint StartCheckpoint {get;set;}
	public List<RaceParticipant> Participants { get; private set; } = new();
	public TimeUntil TimeUntilRaceStart { get; private set; }
	public TimeSince TimeSinceRaceStart { get; private set; }
	public bool HasStarted { get; private set; } = false;
	public bool HasLoaded { get; private set; } = false;
	public SoundEvent GetRaceMusic()
	{
		return RaceMusic.RaceMusic;
	}
	public float GetRaceMusicVolume()
	{
		return RaceMusic.RaceMusicVolume;
	}
	public float GetRaceWaitTime()
	{
		return RaceMusic.RaceStartWait;
	}
	public int GetMaxLaps()
	{
		return RaceContext.CurrentParameters.MaxLaps;
	}
	protected override void OnAwake()
	{
		if(Current != null)
		{
			Current.Destroy();
		}
		Current = this;
		HasStarted = false;
	}

	private void StartCountdown()
	{
		const float RACE_START_COUNTDOWN = 3f;
		HasStarted = false;
		TimeUntilRaceStart = RACE_START_COUNTDOWN + GetRaceWaitTime();
		Music.Play( GetRaceMusic(), GetRaceMusicVolume() );
	}

	private void StartRace()
	{
		HasStarted = true;
		TimeSinceRaceStart = 0;
	}

	public override void Reset()
	{
		SetupRace();
	}
	private void SetupRace()
	{
		Participants?.Clear();
		ResetParticipants();

		foreach(var vehicle in Scene.GetAllComponents<VehicleController>())
		{
			vehicle.Reset();
		}

		foreach ( var item in Scene.GetAllComponents<ItemPickup>() )
		{
			item.GameObject.Enabled = !IsTimeTrial;
		}

		RaceContext?.ResetParticipantObjects();
		Participants = Scene.GetAllComponents<RaceParticipant>().ToList();
		InitialiseParticipants( Participants);
		foreach(var participant in Participants)
		{
			participant.PassCheckpoint( StartCheckpoint, true );
		}

		StartCountdown();
	}

	public void InitialiseParticipants( List<RaceParticipant> participants )
	{
		foreach ( var participant in participants )
		{
			participantRaceCompletion.Add( participant, 0f );
			participantLastOrder.Add( participant, 0 );
			participantLapTimes.Add( participant, new() );
			participantLastLap.Add( participant, 0 );
		}
	}
	public void ResetParticipants()
	{
		participantRaceCompletion.Clear();
		participantLastOrder.Clear();
		participantLastLap.Clear();
		participantLapTimes.Clear();
		finishedParticipants.Clear();
	}
	protected override void OnFixedUpdate()
	{
		if(!HasLoaded && RaceContext.FinishedLoading)
		{
			HasLoaded = true;

			OrderCheckpoints();
			SetupRace();
		}

		if(TimeUntilRaceStart && !HasStarted)
		{
			StartRace();
		}

		UpdateCompletion();
	}

	protected override void DrawGizmos()
	{
		const float START_POSITION_RADIUS = 36f;
		const float CHECKPOINT_TEXT_OFFSET = 128f;
		const float TEXT_SIZE = 24f;
		const float WORLD_TEXT_SIZE = 16f;

		if(StartCheckpoint == null)
		{
			Gizmo.Draw.Color = Color.Red;
			Gizmo.Draw.ScreenText( "No start point set for race manager!", new( 200 ), size: TEXT_SIZE );
			return;
		}

		Color startColor = Color.Orange;
		Color checkpointColor = Color.Yellow;
		Vector3 startPosition = Transform.World.PointToLocal( StartCheckpoint.Transform.Position );

		Gizmo.Draw.Color = startColor;
		Gizmo.Draw.LineSphere( startPosition, START_POSITION_RADIUS );

		if ( !checkpointOrder.Any() ) return;

		foreach(var kv in checkpointOrder)
		{
			Vector3 checkpointPosition = Transform.World.PointToLocal( kv.Key.Transform.Position );
			Gizmo.Draw.Color = checkpointColor;
			Gizmo.Draw.Text( $"{kv.Key.GameObject.Name} [{kv.Value}]", kv.Key.Transform.World.WithPosition(checkpointPosition + Vector3.Up * CHECKPOINT_TEXT_OFFSET), size: WORLD_TEXT_SIZE );
		}
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(checkpointOrder, participantRaceCompletion);
	}
}
