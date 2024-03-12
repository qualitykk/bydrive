using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Alias("VehicleCameraView")]
public class VehicleCamera : Component, ICameraMode
{
	[Property] public float BaseFieldOfView { get; set; } = 90f;
	[Property] public float MaxFieldOfView { get; set; } = 120f;
	private VehicleController Vehicle => GetLocalVehicle();
	private Vector3 CameraOffset => Vehicle?.GetCameraPositionOffset() ?? 0f;
	private VehicleController lastVehicle;
	public void UpdateCamera( CameraComponent camera )
	{
		if ( Vehicle != lastVehicle )
		{
			GameObject.Parent = Vehicle?.GameObject;
			lastVehicle = Vehicle;
		}

		if ( Vehicle == null ) return;

		Vector3 position = GetCameraPosition(camera);
		Rotation rotation = GetCameraRotation();

		camera.Transform.LocalPosition = position;
		camera.Transform.LocalRotation = rotation;

		float dt = Time.Delta;

		float maxSpeed = Vehicle.GetStats().MaxSpeed;
		float speedFraction = Vehicle.Speed / maxSpeed;
		float currentFov = camera.FieldOfView;
		float targetFov = speedFraction.Remap( 0, 1, BaseFieldOfView, MaxFieldOfView, false );

		if ( currentFov < targetFov )
		{
			camera.FieldOfView = currentFov.LerpTo( targetFov, dt );
		}
		else
		{
			const float BREAK_FOV_DECREASE_MULTIPLIER = 4f;
			camera.FieldOfView = currentFov.LerpTo( targetFov, dt * BREAK_FOV_DECREASE_MULTIPLIER );
		}
	}

	const float TURN_OFFSET_DURATION = 0.3f;
	const float TURN_OFFSET_MAX_DISTANCE = 48f;
	const float TURN_OFFSET_MAX_YAW = -10f;
	float currentTurnOffset;
	private Vector3 GetCameraPosition(CameraComponent camera)
	{
		Vector3 position = CameraOffset;

		float turn = Vehicle.TurnDirection;
		currentTurnOffset = currentTurnOffset.LerpTo( turn, Time.Delta / TURN_OFFSET_DURATION );

		position.y = currentTurnOffset * TURN_OFFSET_MAX_DISTANCE;
		// Dont let camera go through walls
		var tr = Scene.Trace.Ray( camera.Transform.World.PointToWorld( CameraOffset ), camera.Transform.World.PointToWorld( position ) )
							.IgnoreGameObjectHierarchy(Vehicle.GameObject)
							.WithTag( TraceTags.WORLD )
							.Run();
		return camera.Transform.World.PointToLocal(tr.EndPosition);
	}

	private Rotation GetCameraRotation()
	{
		const float PITCH_OFFSET = 15f;

		float pitch = PITCH_OFFSET;
		float yaw = currentTurnOffset * TURN_OFFSET_MAX_YAW;

		return Rotation.From( pitch, yaw, 0f );
	}
}
