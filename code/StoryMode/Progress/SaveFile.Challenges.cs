using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class SaveFile
{
	public Dictionary<ChallengeDefinition, ChallengeState> GetUnlockedChallenges()
	{
		return ChallengeStates.Where( kv => kv.Value > ChallengeState.Hidden )
							.ToDictionary( kv => ChallengeDefinition.Get( kv.Key ), kv => kv.Value );
	}
	[ActionGraphIgnore] public Dictionary<string, ChallengeState> ChallengeStates { get; set; } = new();
	public ChallengeState GetChallengeState( string challenge )
	{
		if ( ChallengeStates.TryGetValue( challenge, out ChallengeState state ) )
		{
			return state;
		}

		return ChallengeState.Hidden;
	}
	public ChallengeState GetChallengeState( ChallengeDefinition challenge ) => GetChallengeState( challenge.Id );
	public Dictionary<ChallengeDefinition, ChallengeState> GetChallengeStateAll()
	{
		return ChallengeStates.ToDictionary(kv => ChallengeDefinition.Get( kv.Key ), kv => kv.Value);
	}
	public void SetChallengeState(string challenge, ChallengeState status)
	{
		if( ChallengeStates.ContainsKey(challenge))
		{
			ChallengeStates[challenge] = status;
		}
		else
		{
			ChallengeStates.Add(challenge, status);
		}
	}
	public void SetChallengeState( ChallengeDefinition challenge, ChallengeState state ) => SetChallengeState( challenge.Id, state );
}
