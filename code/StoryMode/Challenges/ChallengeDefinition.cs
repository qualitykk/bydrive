using Sandbox.VR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bydrive;

/// <summary>
/// Story mode challenge
/// </summary>
[GameResource( "Story Mode Challenge", "chal", "A challenge in story mode." )]
[Icon( "work" )]
public class ChallengeDefinition : GameResource
{
	public struct Participant
	{
		[Hide] public string Name => Character?.Name ?? NameOverride;
		[Hide] public VehicleDefinition Vehicle => Character?.GetVehicle() ?? VehicleOverride;
		[Hide] public int StartPosition => StartPositionOverride;
		public CharacterDefinition Character { get; set; }
		public string NameOverride { get; set; }
		public VehicleDefinition VehicleOverride { get; set; }
		public int StartPositionOverride { get; set; }
		public bool Show { get; set; }
		public override string ToString()
		{
			return $"{Name}={StartPosition}";
		}
	}
	public static IReadOnlyList<ChallengeDefinition> All => _all;
	private static readonly List<ChallengeDefinition> _all = new();
	private static readonly Dictionary<string, ChallengeDefinition> _allByDefinition = new();
	public static ChallengeDefinition Get(string id)
	{
		if(!_allByDefinition.TryGetValue( id, out var definition))
		{
			Log.Warning( $"No definition with id {id} found!" );
			return null;
		}
		return definition;
	}
	[Hide] public string Id => $"{GetType()}:{ResourceName}";
	public string Title { get; set; }
	[TextArea] public string Description { get; set; }
	[Title("Reward"), TextArea] public string RewardDisplay { get; set; }
	public Story.CompletionProgress OnComplete { get; set; }
	/// <summary>
	/// Called when save file is updated
	/// </summary>
	public Story.UnlockCheck ShouldUnlock { get; set; }
	[Category("Race")] public List<RaceSetup> Races { get; set; }
	[Category( "Race" )] public List<Participant> Participants { get; set; }
	[JsonIgnore, Hide] public bool IsSingle => IsRace && Races.Count == 1;
	[JsonIgnore, Hide] public bool IsRace => Races != default;
	public IEnumerable<Participant> GetVisibleParticipants()
	{
		return Participants.Where( p => p.Show );
	}
	protected override void PostLoad()
	{
		if(_allByDefinition.ContainsKey( Id ))
		{
			throw new InvalidOperationException( "Challenge ID must be unique!" );
		}
		_allByDefinition.Add(Id, this);
		_all.Add( this );
	}
	[ConCmd("st_challenge_play")]
	private static void Command_PlayChallenge(string id)
	{
		var challenge = Get( id );

		if(!challenge.IsRace)
		{
			Log.Warning( "Selected challenge is not a race, cant start it!" );
			return;
		}

		StartRace.Challenge( challenge, VehicleBuilder.ForDefinition(VehicleDefinition.GetDefault()) );
	}

	[ConCmd("st_challenge_setstate")]
	private static void Command_SetChallengeUnlocked(string id, int state)
	{
		if(!Story.Active)
		{
			Log.Warning( "Story mode not active, cant unlock challenge!" );
			return;
		}

		var challenge = Get( id );

		if ( !challenge.IsRace )
		{
			Log.Warning( "Selected challenge is not a race, cant start it!" );
			return;
		}

		CurrentSave.SetChallengeState( challenge, (ChallengeState)state );
	}

	[ConCmd("st_challenge_dump")]
	private static void Command_DumpChallenges()
	{
		Log.Info( "===[ Challenges ]===" );
		foreach(var challenge in _all)
		{
			Log.Info( $"> {challenge.Id}: {challenge} ({challenge.IsRace})" );
		}
	}
}
