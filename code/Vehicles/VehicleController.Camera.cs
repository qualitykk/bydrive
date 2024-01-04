using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	[Property, Category("Camera")] public CameraComponent Camera { get; set; }
	[Property, Category( "Camera" )] public float BaseFieldOfView { get; set; } = 90f;
	[Property, Category( "Camera" )] public float MaxFieldOfView { get; set; } = 120f;
	private void UpdateCamera()
	{
		if ( Camera == null || Body == null ) return;

		float dt = Time.Delta;
		Vector3 localVelocity = Transform.World.PointToLocal(Body.Velocity);

		float speedFraction = localVelocity.Length / MaxSpeed;
		float currentFov = Camera.FieldOfView;
		float targetFov = speedFraction.Remap( 0, 1, BaseFieldOfView, MaxFieldOfView );

		if(currentFov < targetFov)
		{
			Camera.FieldOfView = currentFov.LerpTo( targetFov, dt );
		}
		else
		{
			const float BREAK_FOV_DECREASE_MULTIPLIER = 4f;
			Camera.FieldOfView = currentFov.LerpTo( targetFov, dt * BREAK_FOV_DECREASE_MULTIPLIER );
		}
	}
}
