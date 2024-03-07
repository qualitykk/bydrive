namespace Bydrive;

public partial class StoryMenu
{
	bool wantsDraw = false;
	protected override void OnUpdate()
	{
		SetClass( "active", wantsDraw );

		if(Input.EscapePressed)
		{
			wantsDraw = !wantsDraw;
		}
	}

	private string GetTeleportLabel()
	{
		if(Story.InOverworld)
		{
			return "Enter Garage";
		}
		return "Return to City";
	}

	void OnClickResume()
	{
		wantsDraw = false;
	}

	void OnClickTeleport()
	{
		wantsDraw = false;

		if ( Story.InOverworld )
		{
			Story.EnterRaceSetup();
		}
		else
		{
			Story.EnterOverworld();
		}
	}

	void OnClickExit()
	{
		Story.Save();
		Story.Exit();
	}

	void OnClickChallenge(ChallengeDefinition definition)
	{
		StartRace.Challenge( definition, VehicleBuilder.ForDefinition(VehicleDefinition.GetDefault()) );
	}
}
