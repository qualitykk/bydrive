using Sandbox.Diagnostics;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class RaceInformation
{
	public class Participant
	{
		public VehicleBuilder Vehicle { get; set; }
		public Player Player { get; set; }
		public int StartPlacement { get; set; }
		public Participant( VehicleBuilder vehicle, Player ply, int startPosition = RaceStartingPosition.FIRST_PLACE )
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
	public delegate void RaceFinished( RaceInformation information, List<Participant> participants );
	public const int MAX_PLAYERCOUNT = 16;
	public static RaceInformation Current { get; set; }
	public List<RaceSetup> Races { get; set; }
	public bool IsMultiRace => Races != null && Races.Count > 1;
	public List<Participant> Participants { get; set; }
	
	public int CurrentIndex { get; set; }
	private RaceSetup currentSetup => Races.ElementAtOrDefault( CurrentIndex );
	public TrackDefinition CurrentDefinition => currentSetup.Track;
	public RaceParameters CurrentParameters => currentSetup.Parameters ?? CurrentDefinition.Parameters;
	public TrackMusicParameters CurrentMusic => currentSetup.Music ?? CurrentDefinition.Music;
	public Dictionary<string, string> CurrentVariables => currentSetup.Variables;
	public Action OnParticipantsCreated { get; set; }
	public RaceFinished OnRaceFinished { get; set; }
	public RaceFinished OnAllRacesFinished { get; set; }
	public bool FinishedLoading 
	{ 
		get
		{
			return objectsCreated;
		} 
	}
	private bool objectsCreated = false;
	public RaceMode Mode { get; set; }
	private bool multiplayer;
	/// <summary>
	/// Total participant score, used in multi-race only.
	/// </summary>
	private Dictionary<Participant, int> participantScore = new();

	private Dictionary<Participant,GameObject> participantObjects = new();
	private Dictionary<GameObject, Participant> objectParticipants = new();
	public RaceInformation( List<RaceSetup> races, List<Participant> participants)
	{
		Assert.NotNull( races );
		Assert.NotNull( participants );

		Races = races;
		Participants = participants;
	}
	public RaceInformation(RaceSetup race, List<Participant> participants) : this( new List<RaceSetup> () { race}, participants ) { }
	public RaceInformation( TrackDefinition track, List<Participant> participants ) : this( new RaceSetup() { Track = track }, participants ) { }
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

		CreateParticipantObjects();
		OnParticipantsCreated?.Invoke();
		InitialiseMode();

		if ( multiplayer )
		{
			foreach ( var obj in Game.ActiveScene.GetAllObjects( false ) )
			{
				obj.BreakFromPrefab();
			}
		}

		var variableToggles = Game.ActiveScene.GetAllComponents<TrackVariableToggle>();
		if(variableToggles.Any())
		{
			foreach ( var variableToggle in variableToggles)
			{
				if(CurrentVariables.TryGetValue(variableToggle.Key, out string actualValue))
				{
					variableToggle.GameObject.Enabled = variableToggle.Value == actualValue;
				}
				else
				{
					variableToggle.GameObject.Enabled = false;
				}
			}
		}
	}
	/// <summary>
	/// Applies results, win logic, etc
	/// </summary>
	/// <param name="results">Finished participants ordered by placement (first = first place, etc)</param>
	public void Finish( List<Participant> results )
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
			Participant p = results[i];
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
		Music.Play( Soundtrack.RACE_WIN );
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
	public IReadOnlyDictionary<Participant, int> GetAllScores() => participantScore.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly();
	public int GetScore( Participant p )
	{
		if ( participantScore.TryGetValue( p, out int score ) )
		{
			return score;
		}

		return 0;
	}

	public Participant GetParticipant(GameObject obj)
	{
		if ( objectParticipants.TryGetValue( obj, out Participant p ) ) return p;
		return null;
	}
	public Participant GetParticipant( Component c ) => GetParticipant( c.GameObject );
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

	private GameObject BuildParticipantObject(Participant participant)
	{
		string name = participant.Player.Name;
		GameObject obj = participant.Vehicle.Build();
		obj.Name = name;
		if(multiplayer)
		{
			obj.Networked = true;
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

		if ( participant.Player.IsBot )
		{
			CreateBotObjects( obj, vehicle );
		}
		else
		{
			CreatePlayerObjects( obj, vehicle, participant );
		}

		var input = obj.Components.GetInDescendantsOrSelf<VehicleInputComponent>();
		input.ParticipantInstance = participantComponent;
		input.VehicleController = vehicle;

		if(multiplayer)
		{
			obj.NetworkSpawn();
		}

		return obj;
	}
	private void CreateBotObjects(GameObject parent, VehicleController controller)
	{
		const string BOT_PREFAB = "prefabs/race_bot.prefab";

		GameObject obj = new();
		obj.ApplyPrefab( BOT_PREFAB );
		obj.Parent = parent;
		obj.Name = "Bot";
	}
	private void CreatePlayerObjects(GameObject parent, VehicleController controller, Participant participant)
	{
		const string PLAYER_PREFAB = "prefabs/race_player.prefab";
		if ( !participant.Player.IsLocal )
			return;

		GameObject obj = new();
		obj.ApplyPrefab( PLAYER_PREFAB );
		obj.Parent = parent;
		obj.Name = participant.Player.Name;

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
	}

	/// <summary>
	/// Reset participant objects into their initial state.
	/// </summary>
	public void ResetParticipantObjects()
	{
		foreach( (Participant data, GameObject obj) in participantObjects)
		{
			Initialise( data, obj );
		}
	}

	private void Initialise( Participant participant, GameObject obj )
	{
		Assert.NotNull( obj, "Cant initialise a null object!" );
		Assert.NotNull( participant, "Cant initialise with no participant!!" );

		var startingPositions = Game.ActiveScene.GetAllComponents<RaceStartingPosition>();
		if ( !startingPositions.Any() )
		{
			Log.Error( "No starting positions placed in scene, cant place participant objects!" );
			return;
		}

		RaceStartingPosition start = startingPositions.Where( p => p.Placement == participant.StartPlacement ).FirstOrDefault();
		if ( start == null )
		{
			start = startingPositions.FirstOrDefault();
		}

		obj.Transform.World = start.Transform.World;
	}

	private void InitialiseMode()
	{
		RaceManager manager = Game.ActiveScene.GetAllComponents<RaceManager>().FirstOrDefault();
		if ( manager == null )
			return;

		if(Mode == RaceMode.TimeTrial)
		{
			manager.IsTimeTrial = true;
		}
	}

	private void DestroyParticipantObjects()
	{
		foreach((Participant data, GameObject obj) in participantObjects)
		{
			obj.Destroy();
		}

		participantObjects.Clear();
		objectParticipants.Clear();
		objectsCreated = false;
	}
}
