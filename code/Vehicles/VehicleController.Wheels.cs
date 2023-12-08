using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redrome;

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

	private void UpdateWheels()
	{
		wheelAngle = wheelAngle.LerpTo( turnDirection * 25, 1.0f - MathF.Pow( 0.001f, Time.Delta ) );
		wheelRevolute += (WheelSpeed / 14.0f).RadianToDegree() * Time.Delta;

		RaycastWheels( false, Time.Delta );
	}

	private void RaycastWheels(bool doPhysics, float dt )
	{
		var tiltAmount = AccelerationTilt * 2.5f;
		var leanAmount = turnLean * 2.5f;

		float length = 20.0f;

		foreach ( var wheel in GameObject.Components.GetAll<VehicleWheel>() )
		{
			if(wheel.Raycast( length + tiltAmount + leanAmount, doPhysics, dt ))
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
