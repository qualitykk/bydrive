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

	[ActionGraphNode( "savefile.challenge.get_state" )]
	[Title( "Get Challenge State" ), Category( "Save File" )]
	private static ChallengeState Action_GetChallengeState( ChallengeDefinition challenge)
	{
		if ( !Story.Active )
		{
			Log.Warning( "Tried to get challenge state outside of story mode, ignoring..." );
			return new();
		}

		return CurrentSave.GetChallengeState( challenge );
	}

	[ActionGraphNode( "savefile.challenge.get_state_all" )]
	[Title( "GEt all Challenge States" ), Category( "Save File" )]
	private static Dictionary<ChallengeDefinition, ChallengeState> Action_GetChallengeStateAll()
	{
		if ( !Story.Active )
		{
			Log.Warning( "Tried to get challenge state outside of story mode, ignoring..." );
			return new();
		}

		return CurrentSave.GetChallengeStateAll();
	}
	[ActionGraphNode("savefile.challenge.set_state")]
	[Title( "Set Challenge State" ), Category( "Save File")]
	private static void Action_SetChallengeState(ChallengeDefinition challenge, ChallengeState state)
	{
		if(!Story.Active)
		{
			Log.Warning( "Tried to set challenge state outside of story mode, ignoring..." );
			return;
		}

		CurrentSave.SetChallengeState( challenge, state );
	}
	[ActionGraphNode( "savefile.challenge.unlock" )]
	[Title("Unlock Challenge"), Category( "Save File" )]
	private static void Action_UnlockChallenge(ChallengeDefinition challenge)
	{
		if ( !Story.Active )
		{
			Log.Warning( "Tried to unlock challenge state outside of story mode, ignoring..." );
			return;
		}

		if(CurrentSave.GetChallengeState(challenge) > ChallengeState.Hidden)
		{
			Log.Info( "Challenge already unlocked, ignoring..." );
			return;
		}

		CurrentSave.SetChallengeState( challenge, ChallengeState.InProgress );
	}
}
