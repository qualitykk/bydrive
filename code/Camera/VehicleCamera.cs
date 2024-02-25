﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Alias("VehicleCameraView")]
public class VehicleCamera : Component
{
	[Property] public float BaseFieldOfView { get; set; } = 90f;
	[Property] public float MaxFieldOfView { get; set; } = 120f;
	private CameraComponent Camera => GameManager.ActiveScene.Camera;
	private VehicleController Vehicle => GetLocalVehicle();
	private Vector3 CameraOffset => Vehicle?.GetCameraPositionOffset() ?? 0f;
	private VehicleController lastVehicle;
	protected override void OnUpdate()
	{
		if ( Vehicle != lastVehicle )
		{
			GameObject.Parent = Vehicle?.GameObject;
			lastVehicle = Vehicle;
		}

		if ( Camera == null || Vehicle == null ) return;

		Vector3 position = GetCameraPosition();
		Rotation rotation = GetCameraRotation();

		Camera.Transform.LocalPosition = position;
		Camera.Transform.LocalRotation = rotation;

		float dt = Time.Delta;

		float maxSpeed = Vehicle.Stats.MaxSpeed;
		float speedFraction = Vehicle.Speed / maxSpeed;
		float currentFov = Camera.FieldOfView;
		float targetFov = speedFraction.Remap( 0, 1, BaseFieldOfView, MaxFieldOfView, false );

		if ( currentFov < targetFov )
		{
			Camera.FieldOfView = currentFov.LerpTo( targetFov, dt );
		}
		else
		{
			const float BREAK_FOV_DECREASE_MULTIPLIER = 4f;
			Camera.FieldOfView = currentFov.LerpTo( targetFov, dt * BREAK_FOV_DECREASE_MULTIPLIER );
		}
	}

	const float TURN_OFFSET_DURATION = 0.3f;
	const float TURN_OFFSET_MAX_DISTANCE = 48f;
	const float TURN_OFFSET_MAX_YAW = -10f;
	float currentTurnOffset;
	private Vector3 GetCameraPosition()
	{
		Vector3 position = CameraOffset;

		float turn = Vehicle.TurnDirection;
		currentTurnOffset = currentTurnOffset.LerpTo( turn, Time.Delta / TURN_OFFSET_DURATION );

		position.y = currentTurnOffset * TURN_OFFSET_MAX_DISTANCE;
		// Dont let camera go through walls
		var tr = Scene.Trace.Ray( Camera.Transform.World.PointToWorld( CameraOffset ), Camera.Transform.World.PointToWorld( position ) )
							.IgnoreGameObjectHierarchy(Vehicle.GameObject)
							.WithTag( TraceTags.WORLD )
							.Run();
		return Camera.Transform.World.PointToLocal(tr.EndPosition);
	}

	private Rotation GetCameraRotation()
	{
		const float PITCH_OFFSET = 15f;

		float pitch = PITCH_OFFSET;
		float yaw = currentTurnOffset * TURN_OFFSET_MAX_YAW;

		return Rotation.From( pitch, yaw, 0f );
	}
}
