using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redrome;

public partial class VehicleController
{
	/*
	
	private bool frontWheelsOnGround;
		private bool backWheelsOnGround;
		private float accelerateDirection;
		private float airRoll;
		private float airTilt;
		private float grip;
		private float wheelAngle = 0.0f;
		private float wheelRevolute = 0.0f;

		[Net] private float WheelSpeed { get; set; }

		#endregion
		public IReadOnlyList<VehicleWheel> Wheels => wheels;
		protected List<VehicleWheel> wheels = new();
		protected void InitWheels()
		{
			// TODO: Let the car define these.
			AddWheel( "cr_wheel_frontleft", WheelPosition.FrontLeft );

			AddWheel( "cr_wheel_frontright", WheelPosition.FrontRight );

			AddWheel( "cr_wheel_backleft", WheelPosition.BackLeft );

			AddWheel( "cr_wheel_backright", WheelPosition.BackRight );

			Log.Info($"AttachmentCount: {GetModel().AttachmentCount}");
			Log.Info( $"Wheels: {wheels.Count}" );
		}
		public void AddWheel(string name, WheelPosition pos)
		{
			VehicleWheel wheel = new()
			{
				AttachmentName = name,
				WheelPosition = pos
			};
			wheel.Init(this);
			wheels.Add(wheel );
		}
		[Event.Tick.Server]
		public void OnTick()
		{
			wheelAngle = wheelAngle.LerpTo( TurnDirection * 25, 1.0f - MathF.Pow( 0.001f, Time.Delta ) );
			wheelRevolute += (WheelSpeed / (14.0f * Scale)).RadianToDegree() * Time.Delta;

			var wheelRotFrontRight = Rotation.From( 0, 90 + wheelAngle , -wheelRevolute );
			var wheelRotFrontLeft = Rotation.From( 0, 90 + wheelAngle, wheelRevolute );
			var wheelRotBackRight = Rotation.From( 0, 90, +wheelRevolute );
			var wheelRotBackLeft = Rotation.From( 0, -90, -wheelRevolute );

			RaycastWheels( false, out _, out _, Time.Delta );

			foreach(var wheel in wheels)
			{
				switch(wheel.WheelPosition)
				{
					case WheelPosition.FrontLeft:
						wheel.LocalRotation = wheelRotFrontLeft;
						break;
					case WheelPosition.FrontRight:
						wheel.LocalRotation = wheelRotFrontRight;
						break;
					case WheelPosition.BackLeft:
						wheel.LocalRotation = wheelRotBackLeft;
						break;
					case WheelPosition.BackRight:
						wheel.LocalRotation = wheelRotBackRight;
						break;
				}
				//wheel.ResetPosition();
			}
		}

		private void RaycastWheels(bool doPhysics, out bool frontWheels, out bool backWheels, float dt )
		{
			var tiltAmount = AccelerationTilt * 2.5f;
			var leanAmount = TurnLean * 2.5f;

			float length = 20.0f;
			frontWheels = true;
			backWheels = false;

			foreach ( var wheel in wheels )
			{
				//Log.Info( $"Casting Wheel {wheel.AttachmentName}: {wheel.WheelPosition}" );
				if ( wheel.WheelPosition == WheelPosition.FrontLeft )
				{
					if ( wheel.Raycast( length + tiltAmount - leanAmount, doPhysics, dt ) )
						frontWheels = true;
				}
				else if ( wheel.WheelPosition == WheelPosition.FrontRight )
				{
					if ( wheel.Raycast( length + tiltAmount + leanAmount, doPhysics, dt ) )
						frontWheels = true;
				}
				else if ( wheel.WheelPosition == WheelPosition.BackLeft )
				{
					if ( wheel.Raycast( length + tiltAmount - leanAmount, doPhysics, dt ) )
						backWheels = true;
				}
				else
				{
					if ( wheel.Raycast( length + tiltAmount + leanAmount, doPhysics, dt ) )
						backWheels = true;
				}
			}
		}

	 */
}
