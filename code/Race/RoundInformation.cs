using Sandbox.Diagnostics;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class RoundInformation
{
	public delegate void RaceFinished( RoundInformation information, List<RoundParticipant> participants );
	public const int MAX_PLAYERCOUNT = 16;
	public static RoundInformation Current { get; set; }
	public List<RaceSetup> Races { get; set; }
	public bool IsMultiRace => Races != null && Races.Count > 1;
	public List<RoundParticipant> Participants { get; set; }
	
	public int CurrentIndex { get; set; }
	private RaceSetup currentSetup => Races.ElementAtOrDefault( CurrentIndex );
	public TrackDefinition CurrentDefinition => currentSetup.Track;
	public RaceParameters CurrentParameters => currentSetup.Parameters ?? CurrentDefinition.Parameters;
	public TrackMusicParameters CurrentMusic => currentSetup.Music ?? CurrentDefinition.Music;
	public Dictionary<string, string> CurrentVariables => currentSetup.Variables;
	public List<InputRecorder> VehicleInputRecorders { get; set; } = new();
	public Action OnParticipantsCreated { get; set; }
	public RaceFinished OnRaceFinished { get; set; }
	public RaceFinished OnAllRacesFinished { get; set; }
	[Obsolete]
	public RaceMode Mode => CurrentParameters.Mode;
	public bool FinishedLoading 
	{ 
		get
		{
			return objectsCreated;
		} 
	}
	private bool objectsCreated = false;
	private bool multiplayer;
	/// <summary>
	/// Total participant score, used in multi-race only.
	/// </summary>
	private Dictionary<RoundParticipant, int> participantScore = new();

	private Dictionary<RoundParticipant, GameObject> participantObjects = new();
	private Dictionary<GameObject, RoundParticipant> objectParticipants = new();
	public RoundInformation( List<RaceSetup> races, List<RoundParticipant> participants)
	{
		Assert.NotNull( races );
		Assert.NotNull( participants );

		Races = races;
		Participants = participants;
	}
	public RoundInformation(RaceSetup race, List<RoundParticipant> participants) : this( new List<RaceSetup> () { race}, participants ) { }
	public RoundInformation( TrackDefinition track, List<RoundParticipant> participants ) : this( new RaceSetup() { Track = track }, participants ) { }
	public void Start()
	{
		Assert.NotNull( CurrentDefinition );
		Assert.NotNull( CurrentDefinition.Scene );
		Assert.NotNull( Participants );

		Current = this;
		ResetGlobals();
		Game.ActiveScene.LoadFromFile( CurrentDefinition.Scene.ResourcePath );

		if ( CurrentVariables != null && CurrentDefinition.Variables.Any() )
		{
			foreach ( var raceVariables in CurrentDefinition.Variables )
			{
				if ( raceVariables.Required )
				{
					if ( !CurrentVariables.ContainsKey( raceVariables.Key ) )
					{
						throw new InvalidOperationException( $"Cant start track {CurrentDefinition} without required parameter {raceVariables.Key}" );
					}
				}
			}
		}

		var toggles = Game.ActiveScene.Components.GetAll<TrackVariableToggle>( FindMode.EverythingInSelfAndDescendants );
		if ( toggles.Any() )
		{
			foreach ( var variableToggle in toggles )
			{
				if ( CurrentVariables.TryGetValue( variableToggle.Key, out string actualValue ) )
				{
					bool result = true;
					if ( variableToggle.FullMatch )
					{
						result = actualValue == variableToggle.Value;
					}
					else
					{
						result = actualValue.Contains( variableToggle.Value );
					}

					if ( variableToggle.Invert )
					{
						result = !result;
					}
					variableToggle.GameObject.Enabled = result;
				}
				else
				{
					variableToggle.GameObject.Enabled = variableToggle.Invert;
				}
			}
		}

		CreateParticipantObjects();
		OnParticipantsCreated?.Invoke();
		InitialiseMode();

		/*
		if ( multiplayer )
		{

			foreach ( var obj in Game.ActiveScene.GetAllObjects( false ) )
			{
				obj.BreakFromPrefab();
			}
		}
		*/
	}
	/// <summary>
	/// Applies results, win logic, etc
	/// </summary>
	/// <param name="results">Finished participants ordered by placement (first = first place, etc)</param>
	public void Finish( List<RoundParticipant> results )
	{
		if ( results == null )
			results = new();

		foreach(var participant in Participants.Except(results) )
		{
			results.Add( participant );
		}


		int racerCount = results.Count;
		for ( int i = 0; i < racerCount; i++ )
		{
			RoundParticipant p = results[i];
			if ( p == null ) continue;

			int addedScore = racerCount - i;
			if ( IsMultiRace )
			{
				addedScore = CalculateAddedScore( i + 1, racerCount );
			}

			if ( participantScore.ContainsKey( p))
			{
				participantScore[p] += addedScore;
			}
			else
			{
				participantScore.Add(p, addedScore);
			}
		}

		// TODO: Win/Lose music?
		//Music.Play( Soundtrack.RACE_WIN );
		Log.Warning( $"TODO: PLAY WIN MUSIC!" );
	}
	public void NextOrStop()
	{
		CurrentIndex++;

		if ( CurrentIndex >= Races.Count )
		{
			Stop();
			return;
		}

		if (OnRaceFinished != null)
		{
			var participantFinishes = Participants.OrderByDescending( GetScore );
			OnRaceFinished.Invoke( this, participantFinishes.ToList() );
		}
		else
		{
			DestroyParticipantObjects();
			Start();
		}
	}

	/// <summary>
	/// Stops current race, boot back to menu
	/// </summary>
	public void Stop()
	{
		DestroyParticipantObjects();
		if ( OnAllRacesFinished != null )
		{
			var participantFinishes = Participants.OrderByDescending( GetScore );
			OnAllRacesFinished.Invoke( this, participantFinishes.ToList() );
		}
		else
		{
			StartMenu.Open();
		}

		Current = null;
	}
	public IReadOnlyDictionary<RoundParticipant, int> GetAllScores() => participantScore.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly();
	public int GetScore( RoundParticipant p )
	{
		if ( participantScore.TryGetValue( p, out int score ) )
		{
			return score;
		}

		return 0;
	}

	public RoundParticipant GetParticipant(GameObject obj)
	{
		if ( objectParticipants.TryGetValue( obj, out RoundParticipant p ) ) return p;
		return null;
	}
	public RoundParticipant GetParticipant( Component c ) => GetParticipant( c.GameObject );
	/// <summary>
	/// Calculate score to add for a won race.
	/// </summary>
	/// <param name="placement">Racer placement with 1 = first place</param>
	/// <param name="playercount">Total amount of racers in race</param>
	/// <returns></returns>
	public int CalculateAddedScore(int placement, int playercount)
	{
		const int RACE_MAX_SCORE = 20;
		float placementFraction = (playercount-placement+1) / (float)playercount;
		float score = placementFraction * RACE_MAX_SCORE;

		return score.CeilToInt();
	}
	private void CreateParticipantObjects()
	{
		foreach ( var participantInfo in Participants )
		{
			GameObject participantObject = BuildParticipantObject( participantInfo );

			Initialise( participantInfo, participantObject );

		}
		objectsCreated = true;
	}

	private GameObject BuildParticipantObject( RoundParticipant participant )
	{
		string name = participant.Player.Name;
		GameObject obj = participant.Vehicle.Build();
		obj.Name = name;
		if(multiplayer)
		{
			obj.NetworkMode = NetworkMode.Object;
			if(!participant.Player.IsBot)
			{
				obj.Network.AssignOwnership( participant.Player.Connection );
			}
		}

		VehicleController vehicle = obj.Components.GetInDescendantsOrSelf<VehicleController>();
		if(vehicle == null)
		{
			Log.Error( "Prefabs for vehicles MUST include a vehicle controller!" );
			obj.Destroy();

			return null;
		}

		var participantComponent = obj.Components.Create<RaceParticipant>();
		participantComponent.DisplayName = name;

		if ( !participantObjects.ContainsKey( participant ) )
		{
			participantObjects.Add( participant, obj );
			objectParticipants.Add( obj, participant );
		}

		GameObject participantObject;
		if ( participant.Player.IsBot )
		{
			participantObject = CreateBotObjects( vehicle );
		}
		else
		{
			participantObject = CreatePlayerObjects( vehicle, participant );
		}

		var input = participantObject.Components.GetInDescendantsOrSelf<VehicleInputComponent>();
		input.Participant = participantComponent;
		input.VehicleController = vehicle;

		if(multiplayer)
		{
			obj.NetworkSpawn();
		}

		return obj;
	}
	private GameObject CreateBotObjects(VehicleController controller)
	{
		const string BOT_PREFAB = "prefabs/race_bot.prefab";

		GameObject obj = new();
		obj.ApplyPrefab( BOT_PREFAB );
		obj.Name = "Bot ()";

		return obj;
	}
	private GameObject CreatePlayerObjects(VehicleController controller, RoundParticipant participant )
	{
		const string PLAYER_PREFAB = "prefabs/race_player.prefab";
		if ( !participant.Player.IsLocal )
			return null;

		GameObject obj = new();
		obj.ApplyPrefab( PLAYER_PREFAB );
		obj.Name = $"Player ({participant.Player.Name})";

		// TODO: This is stupid, we need to make an event system ASAP!
		if(Story.Active)
		{
			var upgrades = CurrentSave.GetUnlockedUpgrades();
			if(upgrades != null && upgrades.Any())
			{
				foreach(var upgrade in upgrades)
				{
					upgrade.OnRaceStart?.Invoke( controller );
				}
			}
		}

		return obj;
	}

	/// <summary>
	/// Reset participant objects into their initial state.
	/// </summary>
	public void ResetParticipantObjects()
	{
		foreach( (RoundParticipant data, GameObject obj) in participantObjects)
		{
			Initialise( data, obj );
		}
	}

	private void Initialise( RoundParticipant participant, GameObject obj )
	{
		Assert.NotNull( obj, "Cant initialise a null object!" );
		Assert.NotNull( participant, "Cant initialise with no participant!!" );

		var startingPositions = Game.ActiveScene.GetAllComponents<TrackStartingPosition>();
		if ( !startingPositions.Any() )
		{
			Log.Error( "No starting positions placed in scene, cant place participant objects!" );
			return;
		}

		TrackStartingPosition start = startingPositions.Where( p => p.Placement == participant.StartPlacement ).FirstOrDefault();
		if ( start == null )
		{
			start = startingPositions.FirstOrDefault();
		}

		/*
		 * THIS SANITY CHECK CRASHES THE GAME... WTF?
		 * 
		BBox bounds = obj.GetBounds();

		var tr = Game.ActiveScene.Trace.Ray( start.WorldPosition, Vector3.Down * 512f )
										.WithTag( "Solid" )
										.Size(bounds)
										.Run();

		Vector3 spawnPosition = tr.EndPosition + Vector3.Up * bounds.Center.z;
		obj.Transform.World = new(spawnPosition, start.WorldRotation);
		*/

		obj.Transform.World = start.Transform.World;
	}

	private void DestroyParticipantObjects()
	{
		foreach ( (RoundParticipant data, GameObject obj) in participantObjects )
		{
			obj.Destroy();
		}

		participantObjects.Clear();
		objectParticipants.Clear();
		objectsCreated = false;
	}

	private void InitialiseMode()
	{
		if(CurrentParameters.Mode == RaceMode.TimeTrial)
		{
			Race.OnRaceStart += OnStartTimeTrial;
			Race.OnParticipantFinished += OnRacerFinishedTimeTrial;
		}
	}
	private void OnStartTimeTrial()
	{
		var vehicles = Game.ActiveScene.GetAllComponents<VehicleController>();
		foreach(var vehicle in vehicles)
		{
			InputRecorder recorder = VehicleInputRecorders.FirstOrDefault( r => r.Vehicle == vehicle );
			if ( recorder == null )
			{
				recorder = new( vehicle );
				VehicleInputRecorders.Add( recorder );
			}

			recorder.Start();
		}
	}
	private void OnRacerFinishedTimeTrial( RaceParticipant participant )
	{
		InputRecorder recorder = VehicleInputRecorders.FirstOrDefault( r => r.Vehicle == participant.GetVehicle() );
		recorder?.Stop();

		var finish = Race.GetParticipantFinish( participant );
		SaveTimeTrial( participant, finish.LapTimes, recorder );
	}

	void SaveTimeTrial(RaceParticipant participant, List<float> lapTimes, InputRecorder input)
	{
		long participantSteamId = (long?)participant.Network.OwnerConnection?.SteamId ?? Game.SteamId;
		TimeTrialRecording data = new( participantSteamId, CurrentDefinition, input.Vehicle.Definition, CurrentVariables, input.Timestamps, lapTimes );
		TimeTrialRecording.Write( data );

		TimeTrialLeaderboard.SubmitTime( CurrentDefinition.ResourceName, CurrentVariables, lapTimes.Sum() );
	}
}

public class RoundParticipant
{
	public VehicleBuilder Vehicle { get; set; }
	public Player Player { get; set; }
	public int StartPlacement { get; set; }
	public RoundParticipant( VehicleBuilder vehicle, Player ply, int startPosition = TrackStartingPosition.FIRST_PLACE )
	{
		Vehicle = vehicle;
		Player = ply;
		StartPlacement = startPosition;
	}

	public override string ToString()
	{
		return Player.Name;
	}
}
