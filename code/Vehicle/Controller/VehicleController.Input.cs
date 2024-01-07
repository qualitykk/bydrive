
namespace Bydrive;

public partial class VehicleController
{
	public float ThrottleInput;
	public float TurnInput;
	public float BreakInput;

	public float TiltInput;
	public float RollInput;
	public void VerifyInput()
	{
		if ( !CanDrive() )
		{
			
		}
	}

	public void ResetInput()
	{
		ThrottleInput = 0.0f;
		TurnInput = 0.0f;
		BreakInput = 0.0f;
		TiltInput = 0.0f;
		RollInput = 0.0f;
	}

	public bool CanDrive()
	{
		return Race == null || Race.HasStarted;
	}
}
