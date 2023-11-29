
namespace Redrome;

public partial class VehicleController
{
	private float throttleInput;
	private float turnInput;
	private float breakInput;

	private float tiltInput;
	private float rollInput;
	public void BuildInput()
	{
		throttleInput = (Input.Down( InputActions.FORWARD ) ? 1 : 0) + (Input.Down( InputActions.BACK ) ? -1 : 0);
		turnInput = (Input.Down( InputActions.LEFT ) ? 1 : 0) + (Input.Down( InputActions.RIGHT ) ? -1 : 0);
		breakInput = (Input.Down( InputActions.BREAK ) ? 1 : 0);

		tiltInput = (Input.Down( InputActions.BOOST ) ? 1 : 0) + (Input.Down( InputActions.PITCH_DOWN ) ? -1 : 0);
		rollInput = (Input.Down( InputActions.LEFT ) ? 1 : 0) + (Input.Down( InputActions.RIGHT ) ? -1 : 0);
	}
}
