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
		public PrefabFile Prefab { get; set; }
		public string Name { get; set; }
		public int StartPlacement { get; set; }

		public Participant( PrefabFile prefab, string name = "", int startPosition = RaceStartingPosition.FIRST_PLACE )
		{
			Prefab = prefab;
			Name = name;
			StartPlacement = startPosition;
		}
	}

	public static RaceInformation Current { get; set; }
	public Scene RaceScene { get; private set; }
	public RaceDefinition Definition { get; set; }
	public List<Participant> Participants { get; set; }
	public Action OnParticipantsLoaded { get; set; }

	private Dictionary<Participant,GameObject> participantObjects = new();
	private Dictionary<GameObject, RaceStartingPosition> participantStartPositions = new();
	public RaceInformation(RaceDefinition definition, List<Participant> participants, bool createParticipants = true)
	{
		if ( Current != null )
		{
			Current.Stop();
		}

		Current = this;

		GameManager.ActiveScene.Load(definition.Scene);
		RaceScene = GameManager.ActiveScene;

		Definition = definition;
		Participants = participants;

		if(createParticipants)
		{
			CreateParticipantObjects();
		}
	}

	public void CreateParticipantObjects()
	{
		var startingPositions = RaceScene.GetAllComponents<RaceStartingPosition>();
		if ( !startingPositions.Any() )
		{
			Log.Error( "No starting positions placed in scene, cant place participant objects!" );
			return;
		}

		foreach ( var participantInfo in Participants )
		{
			GameObject participantObject = new();
			participantObject.SetPrefabSource( participantInfo.Prefab.ResourcePath );
			participantObject.UpdateFromPrefab();
			participantObject.Name = participantInfo.Name;

			RaceStartingPosition start = startingPositions.Where( p => p.Placement == participantInfo.StartPlacement ).FirstOrDefault();
			if ( start == null )
			{
				start = startingPositions.FirstOrDefault();
			}

			Initialise( participantInfo, participantObject, start );
		}
	}

	private void Initialise(Participant participant, GameObject obj, RaceStartingPosition start)
	{
		obj.Transform.World = start.Transform.World;

		if(obj.Components.TryGet<RaceParticipant>(out var participantInstance))
		{
			participantInstance.DisplayName = participant.Name;
		}

		if(!participantObjects.ContainsKey(participant))
		{
			participantObjects.Add( participant, obj );
		}

		if(!participantStartPositions.ContainsKey(obj))
		{
			participantStartPositions.Add( obj, start );
		}
	}

	/// <summary>
	/// Reset participant objects into their initial state.
	/// </summary>
	public void ResetParticipantObjects()
	{
		foreach( (Participant data, GameObject obj) in participantObjects)
		{
			RaceStartingPosition start = participantStartPositions[obj];
			Initialise( data, obj, start );
		}
	}

	public void DestroyParticipantObjects()
	{
		foreach((Participant data, GameObject obj) in participantObjects)
		{
			obj.Destroy();
		}

		participantObjects.Clear();
		participantStartPositions.Clear();
	}

	/// <summary>
	/// Resets current race
	/// </summary>
	public void Stop()
	{
		RaceScene?.Destroy();
	}
}
