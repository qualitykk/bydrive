using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class SaveFile
{
	private Dictionary<string, ChallengeState> ChallengeStates { get; set; } = new();
	public Dictionary<ChallengeDefinition, ChallengeState> GetUnlockedChallenges()
	{
		return ChallengeStates.Where( kv => kv.Value > ChallengeState.Hidden )
							.ToDictionary( kv => ChallengeDefinition.Get( kv.Key ), kv => kv.Value );
	}
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
	public void SetChallengeState(string challenge, ChallengeState state)
	{
		var definition = ChallengeDefinition.Get( challenge );
		if( ChallengeStates.ContainsKey(challenge))
		{
			ChallengeStates[challenge] = state;
		}
		else
		{
			ChallengeStates.Add(challenge, state);
		}

		if ( definition != null )
			ShowUnlockMessage( definition, state );
	}

	private void ShowUnlockMessage(ChallengeDefinition definition, ChallengeState state)
	{
		Log.Info( $"ShowUnlockMessage {definition} {state}" );
		if ( state == ChallengeState.InProgress )
		{
			Popup.Add( new PopupPage( "Challenge", $"Received a challenge: {definition.Title}", UI.Colors.Popup.Positive ) );
		}
		else if(state == ChallengeState.Complete)
		{
			Popup.Add( new PopupPage( "Challenge", $"Challenge Completed: {definition.Title}", UI.Colors.Popup.Positive ) );
		}
	}
	public void SetChallengeState( ChallengeDefinition challenge, ChallengeState state ) => SetChallengeState( challenge.Id, state );
}
