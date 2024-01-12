using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class RaceInformation
{
	public struct Participant
	{
		public VehicleDefinition Vehicle { get; set; }
		public Player Player { get; set; }
		public int StartPlacement { get; set; }
		public Participant( VehicleDefinition vehicle, Player ply, int startPosition = RaceStartingPosition.FIRST_PLACE )
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

	public static RaceInformation Current { get; set; }
	public Scene Scene => GameManager.ActiveScene;
	public RaceDefinition Definition { get; set; }
	public List<Participant> Participants { get; set; }
	public Action OnParticipantsLoaded { get; set; }

	private Dictionary<Participant,GameObject> participantObjects = new();
	public RaceInformation(RaceDefinition definition, List<Participant> participants, bool createParticipants = true)
	{
		if ( Current != null )
		{
			Current.Stop();
		}

		Current = this;

		GameManager.ActiveScene.LoadFromFile(definition.Scene.ResourcePath);

		Definition = definition;
		Participants = participants;

		if(createParticipants)
		{
			CreateParticipantObjects();
		}
	}

	public void CreateParticipantObjects()
	{
		foreach ( var participantInfo in Participants )
		{
			GameObject participantObject = BuildParticipantObject( participantInfo );

			Initialise( participantInfo, participantObject );
		}
	}

	private GameObject BuildParticipantObject(Participant participant)
	{
		string name = participant.Player.Name;
		GameObject obj = ResourceHelper.CreateObjectFromResource( participant.Vehicle );
		obj.Name = name;

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
			CreatePlayerObjects( obj, vehicle );
		}

		var input = obj.Components.GetInDescendantsOrSelf<VehicleInputComponent>();
		input.ParticipantInstance = participantComponent;
		input.VehicleController = vehicle;

		return obj;
	}
	private void CreateBotObjects(GameObject parent, VehicleController controller)
	{
		const string BOT_VEHICLE_PREFAB = "prefabs/bot_race.prefab";

		GameObject obj = new();
		obj.ApplyPrefab( BOT_VEHICLE_PREFAB );
		obj.Parent = parent;
		obj.Name = "Bot";
	}
	private void CreatePlayerObjects(GameObject parent, VehicleController controller)
	{
		const string PLAYER_VEHICLE_PREFAB = "prefabs/player_race.prefab";

		GameObject obj = new();
		obj.ApplyPrefab( PLAYER_VEHICLE_PREFAB );
		obj.Parent = parent;
		obj.Name = "Player";

		var camera = obj.Components.GetInDescendantsOrSelf<CameraComponent>();
		controller.Camera = camera;
	}

	private void Initialise(Participant participant, GameObject obj)
	{
		Assert.NotNull( obj, "Cant initialise a null object!" );
		Assert.NotNull( participant, "Cant initialise with no participant!!" );

		var startingPositions = Scene.GetAllComponents<RaceStartingPosition>();
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

	public void DestroyParticipantObjects()
	{
		foreach((Participant data, GameObject obj) in participantObjects)
		{
			obj.Destroy();
		}

		participantObjects.Clear();
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
