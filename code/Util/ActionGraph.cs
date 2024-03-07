using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

internal static class ActionGraph
{
	#region Save File
	[ActionGraphNode( "savefile.challenge.get_state" )]
	[Title( "Get Challenge State" ), Category( "Save File" )]
	private static ChallengeState Action_GetChallengeState( ChallengeDefinition challenge )
	{
		if ( !Story.Active )
		{
			Log.Warning( "Tried to get challenge state outside of story mode, ignoring..." );
			return new();
		}

		return CurrentSave.GetChallengeState( challenge );
	}

	[ActionGraphNode( "savefile.challenge.get_state_all" )]
	[Title( "Get all Challenge States" ), Category( "Save File" )]
	private static Dictionary<ChallengeDefinition, ChallengeState> Action_GetChallengeStateAll()
	{
		if ( !Story.Active )
		{
			Log.Warning( "Tried to get challenge state outside of story mode, ignoring..." );
			return new();
		}

		return CurrentSave.GetChallengeStateAll();
	}
	[ActionGraphNode( "savefile.challenge.set_state" )]
	[Title( "Set Challenge State" ), Category( "Save File" )]
	private static void Action_SetChallengeState( ChallengeDefinition challenge, ChallengeState state )
	{
		if ( !Story.Active )
		{
			Log.Warning( "Tried to set challenge state outside of story mode, ignoring..." );
			return;
		}

		CurrentSave.SetChallengeState( challenge, state );
	}
	[ActionGraphNode( "savefile.challenge.unlock" )]
	[Title( "Unlock Challenge" ), Category( "Save File" )]
	private static void Action_UnlockChallenge( ChallengeDefinition challenge )
	{
		if ( !Story.Active )
		{
			Log.Warning( "Tried to unlock challenge state outside of story mode, ignoring..." );
			return;
		}

		if ( CurrentSave.GetChallengeState( challenge ) > ChallengeState.Hidden )
		{
			Log.Info( "Challenge already unlocked, ignoring..." );
			return;
		}

		CurrentSave.SetChallengeState( challenge, ChallengeState.InProgress );
	}
	#endregion

	[ActionGraphNode("util.construct"), Pure]
	[Title("New Object")]
	private static T Action_CreateObject<T>()
	{
		return TypeLibrary.Create<T>();
	}
}
