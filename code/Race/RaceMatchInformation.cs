using Sandbox.Diagnostics;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class RaceMatchInformation
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
	public const int MAX_PLAYERCOUNT = 16;
	public static RaceMatchInformation Current { get; set; }
	public RaceDefinition Definition { get; set; }
	public RaceParameters Parameters { get; set; }
	public List<Participant> Participants { get; set; }
	public Action OnParticipantsLoaded { get; set; }
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

	private Dictionary<Participant,GameObject> participantObjects = new();
	public RaceMatchInformation(RaceDefinition definition, List<Participant> participants, RaceParameters parameters = null, bool createParticipants = true)
	{
		Assert.NotNull( definition );
		Assert.NotNull( participants );

		Current = this;
		//multiplayer = LobbyManager.MultiplayerActive;
		multiplayer = false;

		GameManager.ActiveScene.LoadFromFile( definition.Scene.ResourcePath );

		Definition = definition;
		Participants = participants;
		if ( parameters != null && !parameters.Equals(RaceParameters.Default) && parameters != definition.Parameters )
		{
			Parameters = parameters;
		}
		else
		{
			Parameters = definition.Parameters;
		}

		if (createParticipants)
		{
			CreateParticipantObjects();
		}

		StartMenu.Close();
		InitialiseMode();
		if(multiplayer)
		{
			foreach(var obj in GameManager.ActiveScene.GetAllObjects(false))
			{
				obj.BreakFromPrefab();
			}
		}
	}

	public void CreateParticipantObjects()
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

		obj.NetworkSpawn();

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

		var startingPositions = GameManager.ActiveScene.GetAllComponents<RaceStartingPosition>();
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
		RaceManager manager = GameManager.ActiveScene.GetAllComponents<RaceManager>().FirstOrDefault();
		if ( manager == null )
			return;

		if(Mode == RaceMode.TimeTrial)
		{
			manager.IsTimeTrial = true;
		}
	}

	public void DestroyParticipantObjects()
	{
		foreach((Participant data, GameObject obj) in participantObjects)
		{
			obj.Destroy();
		}

		participantObjects.Clear();
		objectsCreated = false;
	}

	/// <summary>
	/// Stops current race, boot back to menu
	/// </summary>
	public void Stop()
	{
		DestroyParticipantObjects();
		// TODO: Return to lobby
		StartMenu.Open();
		Current = null;
	}
}
