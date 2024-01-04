using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon( "settings_input_antenna" )]
public class VehiclePlayerInput : VehicleInputComponent
{
	protected override void BuildInput()
	{
		// Vehicle Controller Inputs
		VehicleController.ThrottleInput = (Input.Down( InputActions.FORWARD ) ? 1 : 0) + (Input.Down( InputActions.BACK ) ? -1 : 0);
		VehicleController.TurnInput = (Input.Down( InputActions.LEFT ) ? 1 : 0) + (Input.Down( InputActions.RIGHT ) ? -1 : 0);
		VehicleController.BreakInput = (Input.Down( InputActions.BREAK ) ? 1 : 0);

		VehicleController.TiltInput = (Input.Down( InputActions.BOOST ) ? 1 : 0) + (Input.Down( InputActions.PITCH_DOWN ) ? -1 : 0);
		VehicleController.RollInput = (Input.Down( InputActions.LEFT ) ? 1 : 0) + (Input.Down( InputActions.RIGHT ) ? -1 : 0);

		// Participant Inputs
		if(Input.Down(InputActions.RESPAWN))
		{
			ParticipantInstance.Respawn();
			ResetVehicleInputs();
		}
	}

	public bool IsLocalInput()
	{
		return !Network.Active || Network.IsOwner;
	}
}
