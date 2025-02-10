using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class VehicleCamera : Component, ICameraMode
{
	[Property] public float BaseFieldOfView { get; set; } = 90f;
	[Property] public float MaxFieldOfView { get; set; } = 120f;
	private VehicleController Vehicle => GetLocalVehicle();
	private Vector3 CameraOffset => Vehicle?.GetCameraPositionOffset() ?? 0f;
	VehicleController lastVehicle;
	public void UpdateCamera( CameraComponent camera )
	{
		if ( Vehicle != lastVehicle )
		{
			//camera.GameObject.Parent = Vehicle?.GameObject;
			lastVehicle = Vehicle;
		}

		if ( Vehicle == null ) return;

		Transform baseTransform = Vehicle.Transform.World;
		baseTransform.Position = baseTransform.Position.SnapToGrid( 8f, false, false, true);
		baseTransform.Rotation = Rotation.FromYaw( baseTransform.Rotation.Yaw() );
		float delta = 1.0f - MathF.Pow( 0.00001f, Time.Delta);

		Vector3 position = GetCameraPosition(camera, baseTransform, delta );
		Rotation rotation = GetCameraRotation(camera, baseTransform, delta );

		camera.WorldPosition = position;
		camera.WorldRotation = rotation;

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
	private Vector3 GetCameraPosition(CameraComponent camera, Transform transform, float delta)
	{
		Vector3 position = CameraOffset;

		float turn = Vehicle.TurnDirection;
		currentTurnOffset = currentTurnOffset.LerpTo( turn, Time.Delta / TURN_OFFSET_DURATION );

		position.y = currentTurnOffset * TURN_OFFSET_MAX_DISTANCE;
		// Dont let camera go through walls
		var tr = Scene.Trace.Ray( transform.PointToWorld( CameraOffset ), transform.PointToWorld( position ) )
							.IgnoreGameObjectHierarchy(Vehicle.GameObject)
							.WithTag( TraceTags.WORLD )
							.Run();
		//Log.Info( tr );

		return camera.WorldPosition.LerpTo( tr.EndPosition, delta );
	}

	private Rotation GetCameraRotation( CameraComponent camera, Transform transform, float delta )
	{
		const float PITCH_OFFSET = 15f;

		float pitch = PITCH_OFFSET;
		float yaw = currentTurnOffset * TURN_OFFSET_MAX_YAW;
		yaw += transform.Rotation.Yaw();

		return Rotation.Slerp(camera.WorldRotation, Rotation.From( pitch, yaw, 0f ), delta );
	}
}
