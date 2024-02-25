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
		public CharacterDefinition Character { get; set; }
		public string NameOverride { get; set; }
		public VehicleDefinition VehicleOverride { get; set; }
		public int StartPositionOverride { get; set; }
	}
	public delegate void CompletionContext( SaveFile save );
	[Hide] public string Id => $"{GetType()}:{ResourceName}";
	public string Title { get; set; }
	public string Description { get; set; }
	[Category("Race")] public RaceDefinition Track { get; set; }
	[Category( "Race" )] public RaceParameters Parameters { get; set; }
	[Category( "Race" )] public List<Participant> Participants { get; set; }
	[Category( "Race" )] public CompletionContext OnComplete { get; set; }
}
