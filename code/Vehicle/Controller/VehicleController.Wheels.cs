using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	private bool wheelsOnGround;
	private bool drivingWheelsOnGround;
	private bool turningWheelsOnGround;
	private float accelerateDirection;
	
	private float grip;
	private float wheelAngle = 0.0f;
	private float wheelRevolute = 0.0f;
	private float AccelerationTilt { get; set; }
	private float WheelSpeed { get; set; }
	private IEnumerable<VehicleWheel> GetWheels() => GameObject.Components.GetAll<VehicleWheel>();
	private void UpdateWheels()
	{
		wheelAngle = wheelAngle.LerpTo( TurnDirection * 25, 1.0f - MathF.Pow( 0.001f, Time.Delta ) );
		wheelRevolute += (WheelSpeed / 14.0f).RadianToDegree() * Time.Delta;

		RaycastWheels( false, Time.Delta );

		foreach(var wheel in GetWheels())
		{
			Rotation wheelRotation;
			if(wheel.IsTurning)
			{
				wheelRotation = Rotation.From( wheelRevolute, wheelAngle, 0 );
			}
			else
			{
				wheelRotation = Rotation.From( wheelRevolute, 0, 0 );
			}

			wheel.GameObject.Transform.LocalRotation = wheelRotation;
		}
	}

	private void RaycastWheels(bool doPhysics, float dt )
	{
		var tiltAmount = AccelerationTilt * 2.5f;
		var leanAmount = turnLean * 2.5f;

		foreach ( var wheel in GetWheels() )
		{
			if(wheel.Raycast( tiltAmount + leanAmount, doPhysics, dt ))
			{
				wheelsOnGround = true;
				if ( wheel.IsDriving )
					drivingWheelsOnGround = true;

				if ( wheel.IsTurning )
					turningWheelsOnGround = true;
			}
		}
	}
}
