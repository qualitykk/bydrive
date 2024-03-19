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
	[Property] public List<RaceCheckpoint> StartCheckpointOptions { get; set; } = new();
	[Property] public Action OnRaceStart { get; set; }
	[Property] public Action<RaceParticipant> OnParticipantFinished { get; set; }
	[Property] public Action OnAllFinished { get; set; }
	[Property] public Action OnReset { get; set; }
	public List<RaceParticipant> Participants { get; private set; } = new();
	public TimeUntil TimeUntilRaceStart { get; private set; }
	public TimeSince TimeSinceRaceStart { get; private set; }
	public bool HasStarted { get; private set; } = false;
	public bool HasCountdownStarted { get; private set; }
	public bool HasLoaded { get; private set; } = false;
	public bool HasSetup { get; private set; } = false;
	private RaceCheckpoint startCheckpoint;
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
	public RaceCheckpoint GetStartCheckpoint()
	{
		if ( StartCheckpointOptions == null || !StartCheckpointOptions.Any() ) return startCheckpoint;
		if ( startCheckpoint != null ) return startCheckpoint;
		startCheckpoint = StartCheckpointOptions?.FirstOrDefault( c => c?.GameObject?.Active == true );
		return startCheckpoint;
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

	public override void Reset()
	{
		startCheckpoint = null;
		Setup();
	}
	public void Setup(bool autoStart = false)
	{
		OnReset?.Invoke();
		IsTimeTrial = RaceContext.CurrentParameters.Mode == RaceMode.TimeTrial;

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
			participant.PassCheckpoint( GetStartCheckpoint(), true );
		}

		if(autoStart)
		{
			Start();
		}

		HasSetup = true;
	}

	public void Start()
	{
		var cameras = Scene.GetAllComponents<VehicleCamera>();
		foreach ( var camera in cameras )
		{
			CameraManager.MakeActive( camera );
		}

		UI.ShowRaceHUD();

		StartCountdown();
	}

	private void StartCountdown()
	{
		const float RACE_START_COUNTDOWN = 3f;
		HasStarted = false;
		HasCountdownStarted = true;
		TimeUntilRaceStart = RACE_START_COUNTDOWN + GetRaceWaitTime();
		Music.Play( GetRaceMusic(), GetRaceMusicVolume() );
	}

	private void FinishCountdown()
	{
		HasStarted = true;
		TimeSinceRaceStart = 0;

		OnRaceStart?.Invoke();
	}

	public void InitialiseParticipants( List<RaceParticipant> participants )
	{
		foreach ( var participant in participants )
		{
			participantRaceCompletion.Add( participant, 0f );
			participantLastOrder.Add( participant, 0 );
			participantLapFinishTimes.Add( participant, new() );
			participantLastLap.Add( participant, 0 );
		}
	}
	public void ResetParticipants()
	{
		participantRaceCompletion.Clear();
		participantLastOrder.Clear();
		participantLastLap.Clear();
		participantLapFinishTimes.Clear();
		finishedParticipants.Clear();
	}
	protected override void OnFixedUpdate()
	{
		if ( RaceContext == null ) return;

		if(!HasLoaded && RaceContext.FinishedLoading)
		{
			HasLoaded = true;

			OrderCheckpoints();
			Setup();
		}

		if ( !HasSetup ) return;

		if(TimeUntilRaceStart && HasCountdownStarted && !HasStarted)
		{
			FinishCountdown();
		}

		UpdateCompletion();
	}

	protected override void DrawGizmos()
	{
		const float START_POSITION_RADIUS = 36f;
		const float CHECKPOINT_TEXT_OFFSET = 128f;
		const float TEXT_SIZE = 24f;
		const float WORLD_TEXT_SIZE = 16f;

		if(StartCheckpointOptions == null)
		{
			Gizmo.Draw.Color = Color.Red;
			Gizmo.Draw.ScreenText( "No start point set for race manager!", new( 200 ), size: TEXT_SIZE );
			return;
		}

		Color startColor = Color.Orange;
		Color checkpointColor = Color.Yellow;
		var start = GetStartCheckpoint();
		Vector3 startPosition = Transform.World.PointToLocal( start?.Transform.Position ?? Vector3.Zero );
		//Gizmo.Draw.ScreenText( $"Start Checkpoint: {start}", new( 200 ), size: TEXT_SIZE );
;
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
