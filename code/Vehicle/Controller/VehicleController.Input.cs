
namespace Bydrive;

public partial class VehicleController
{
	public float ThrottleInput;
	public float TurnInput;
	public float BreakInput;

	public float TiltInput;
	public float RollInput;

	public bool WantsBoost;
	public bool WantsItem;
	public void VerifyInput()
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
		RollInput = 0.0f;

		WantsBoost = false;
		WantsItem = false;
	}

	public bool CanDrive()
	{
		return Race == null || Race.HasStarted;
	}
}
