namespace Bydrive;

public partial class StoryMenu
{
	internal bool active { get; set; } = false;
	protected override void OnUpdate()
	{
		SetClass( "active", active );

		if(Input.EscapePressed)
		{
			active = !active;
		}
	}

	void OnClickChallenge(ChallengeDefinition definition)
	{
		StartRace.Challenge( definition, VehicleBuilder.ForDefinition(VehicleDefinition.GetDefault()) );
	}
}
