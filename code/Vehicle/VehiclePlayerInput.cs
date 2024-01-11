using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Category( "Vehicle" )]
[Icon( "settings_input_antenna" )]
public class VehiclePlayerInput : VehicleInputComponent
{
	[Property] public bool DebugLocal { get; set; } = false;
	protected override void BuildInput()
	{
		// Vehicle Controller Inputs
		VehicleController.ThrottleInput = (Input.Down( InputActions.FORWARD ) ? 1 : 0) + (Input.Down( InputActions.BACK ) ? -1 : 0);
		VehicleController.TurnInput = (Input.Down( InputActions.LEFT ) ? 1 : 0) + (Input.Down( InputActions.RIGHT ) ? -1 : 0);
		VehicleController.BreakInput = (Input.Down( InputActions.BREAK ) ? 1 : 0);

		VehicleController.TiltInput = (Input.Down( InputActions.BOOST ) ? 1 : 0) + (Input.Down( InputActions.PITCH_DOWN ) ? -1 : 0);
		VehicleController.RollInput = (Input.Down( InputActions.LEFT ) ? 1 : 0) + (Input.Down( InputActions.RIGHT ) ? -1 : 0);

		VehicleController.WantsBoost = Input.Down( InputActions.BOOST );
		VehicleController.WantsItem = Input.Down( InputActions.ITEM );

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

	protected override void DrawGizmos()
	{
		if ( !DebugLocal || !IsLocalInput() ) return;

		const float TEXT_SCREEN_POS_VERTICAL = 200f;
		const float TEXT_VERTICAL_GAP = 30f;
		PhysicsBody body = VehicleController?.Body;
		if ( body == null ) return;

		Vector2 screenPos = new( 200, TEXT_SCREEN_POS_VERTICAL );
		Vector3 localVelocity = VehicleController.Transform.Local.VelocityToLocal( body.Velocity );
		Gizmo.Draw.ScreenText( $"Velocity: {body.Velocity}", screenPos );
		Gizmo.Draw.ScreenText( $"Local Velocity: {localVelocity}", screenPos + Vector2.Up * TEXT_VERTICAL_GAP );
		Gizmo.Draw.ScreenText( $"Speed: {VehicleController.Speed}", screenPos + Vector2.Up * TEXT_VERTICAL_GAP * 2 );
	}
}
