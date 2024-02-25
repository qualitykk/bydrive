using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class SaveFile
{
	public float Money { get; set; }
	[ActionGraphIgnore] public Dictionary<string, ChallengeStatus> ChallengeStates { get; set; }
	public ChallengeStatus GetChallengeStatus(string challenge)
	{
		if(ChallengeStates.TryGetValue(challenge, out ChallengeStatus status))
		{
			return status;
		}

		return ChallengeStatus.Hidden;
	}
	public ChallengeStatus GetChallengeStatus( ChallengeDefinition challenge ) => GetChallengeStatus( challenge.Id );
	public void SetChallengeStatus(string challenge, ChallengeStatus status)
	{
		if(ChallengeStates.ContainsKey(challenge))
		{
			ChallengeStates[challenge] = status;
		}
		else
		{
			ChallengeStates.Add(challenge, status);
		}
	}
	public void SetChallengeStatus( ChallengeDefinition challenge, ChallengeStatus status ) => SetChallengeStatus( challenge.Id, status );
}
