using Sandbox.VR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			Log.Error( $"No definition with id {id} found!" );
		}
		Log.Info( definition.Parameters );
		return definition;
	}
	public delegate void CompletionContext( SaveFile save );
	[Hide] public string Id => $"{GetType()}:{ResourceName}";
	public string Title { get; set; }
	public string Description { get; set; }
	[Hide] public bool IsRace => Track != default;
	[Category("Race")] public RaceDefinition Track { get; set; }
	[Category( "Race" )] public RaceParameters Parameters { get; set; }
	[Category( "Race" )] public List<Participant> Participants { get; set; }
	[Category( "Race" )] public CompletionContext OnComplete { get; set; }
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

		StartRace.Challenge( challenge, VehicleDefinition.GetDefault() );
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
