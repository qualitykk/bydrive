
namespace Bydrive;

public partial class VehicleController
{
	[ActionGraphIgnore] public float ThrottleInput;
	[ActionGraphIgnore] public float TurnInput;
	[ActionGraphIgnore] public float BreakInput;
	[ActionGraphIgnore] public float TiltInput;

	[ActionGraphIgnore] public bool WantsBoost;
	[ActionGraphIgnore] public bool WantsItem;
	private void VerifyInput()
	{
		if ( !CanDrive() )
		{
			ResetInput();
		}
	}

	public void ResetInput()
	{
		ThrottleInput = 0.0f;
		TurnInput = 0.0f;
		BreakInput = 0.0f;
		TiltInput = 0.0f;

		WantsBoost = false;
		WantsItem = false;
	}

	public bool CanDrive()
	{
		return Race == null || Race.HasStarted;
	}
}
