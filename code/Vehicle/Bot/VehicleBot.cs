using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class VehicleBot : VehicleInputComponent
{
	const float TURNAROUND_SLOW_SPEED = 0.7f;
	const float MAX_DRIVABLE_ANGLE = 45f;
	// Wall Avoidance
	const float SAFE_WALL_MIN_FRONT_DISTANCE = 256f;
	const float SAFE_WALL_MIN_REVERSE_TIME = 1.5f;
	const float SAFE_WALL_SLOW_LOW_SPEED = 0.65f;
	const float SAFE_WALL_SLOW_DISTANCE = 768f;


	internal VehicleBotPath currentBotPath;
	VehicleBotPath lastBotPath;
	VehicleBotPath nextBotPath;
	protected override void BuildInput()
	{
		Vector3 goal = GetNextPosition();
		if(goal == Vector3.Zero)
		{
			VehicleController.TurnInput = 0f;
			VehicleController.ThrottleInput = 0f;
			return;
		}

		UpdateTurning( goal );
		UpdateThrottle( goal );
	}

	float currentTurnaroundDirection;
	private void UpdateTurning(Vector3 goal)
	{
		Vector3 goalDirection = goal - Transform.Position;
		Vector3 localGoal = Transform.World.PointToLocal( goal );
		// Calculate if the direction to the goal is left or right form our forward vector. (https://discussions.unity.com/t/steer-towards-target/186660)
		float goalSteerDirection = Vector3.Dot( Vector3.Cross( Transform.Rotation.Forward, goalDirection.Normal ), Vector3.Up );
		float turnDirection = goalSteerDirection;

		if( localGoal.x < 0f )
		{
			if(currentTurnaroundDirection == 0f)
			{
				currentTurnaroundDirection = Game.Random.Int( -1, 1 );
			}

			turnDirection = currentTurnaroundDirection;
		}
		else
		{
			currentTurnaroundDirection = 0f;
		}

		const float MIN_TURN_INPUT = 0.75f;
		turnDirection = MathF.Max( MathF.Abs( turnDirection ), MIN_TURN_INPUT ) * MathF.Sign( turnDirection );

		/*
		var trLeft = SafeCheckTrace( Transform.Rotation.Left, SAFE_WALL_MIN_SIDE_DISTANCE );
		var trRight = SafeCheckTrace( Transform.Rotation.Right, SAFE_WALL_MIN_SIDE_DISTANCE );

		if(trLeft.Hit && trRight.Hit)
		{
			turnDirection = 0f;
		}
		else if(trLeft.Hit)
		{
			turnDirection = SAFE_WALL_SLIGHT_STEER;
		}
		else if(trRight.Hit)
		{
			turnDirection = -SAFE_WALL_SLIGHT_STEER;
		}
		*/

		VehicleController.TurnInput = turnDirection * MathF.Sign( VehicleController.ThrottleInput );
	}

	TimeSince timeSinceGoBack;
	private void UpdateThrottle(Vector3 goal)
	{
		Vector3 localGoal = Transform.World.PointToLocal( goal );

		float throttle = 1f;
		if(localGoal.x < 0f)
		{
			throttle = TURNAROUND_SLOW_SPEED;
		}

		if ( timeSinceGoBack < SAFE_WALL_MIN_REVERSE_TIME )
		{
			throttle = -1f;
		}
		else 
		{
			var tr = SafeCheckTrace( Transform.Rotation.Forward, SAFE_WALL_SLOW_DISTANCE );
			if ( tr.Hit && !IsDrivableNormal(tr.Normal) )
			{
				if ( tr.Distance < SAFE_WALL_MIN_FRONT_DISTANCE )
				{
					throttle = -1f;
					timeSinceGoBack = 0;
				}
				else
				{
					throttle = tr.Distance.Remap(SAFE_WALL_SLOW_DISTANCE, SAFE_WALL_SLOW_DISTANCE, SAFE_WALL_SLOW_LOW_SPEED);
				}
			}
		}

		VehicleController.ThrottleInput = throttle;
	}

	private bool IsDrivableNormal(Vector3 normal)
	{
		return normal.Angle( Vector3.Up ) <= MAX_DRIVABLE_ANGLE;
	}

	private Vector3 GetNextPosition()
	{
		if(currentBotPath != lastBotPath)
		{
			nextBotPath = null;
			lastBotPath = currentBotPath;
		}

		if(currentBotPath != null)
		{
			if(nextBotPath == null)
			{
				nextBotPath = Game.Random.FromList( currentBotPath.NextPaths );
			}

			return nextBotPath.GetTargetPosition(Transform.Position);
		}

		return ParticipantInstance.NextKeyCheckpoints.ElementAtOrDefault( 0 )?.Transform.Position ?? Transform.Position;
	}

	private Vector3 LocalTracePosition => VehicleController.Body.LocalMassCenter;
	private Vector3 TracePosition => VehicleController.Body.MassCenter;
	private SceneTraceResult SafeCheckTrace( Vector3 direction, float distance)
	{
		return Scene.Trace.Ray(new Ray( TracePosition, direction), distance)
							.IgnoreGameObject(GameObject)
							.WithTag( TraceTags.SOLID )
							//.WithoutTags(TraceTags.VEHICLE)
							.Run();
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Vector3 nextLocalPosition = Transform.World.PointToLocal( GetNextPosition() );
		Gizmo.Draw.Color = Color.Green;
		Gizmo.Draw.Line( LocalTracePosition, nextLocalPosition );
		Gizmo.Draw.WorldText( $"currentBotPath: {currentBotPath}", new( LocalTracePosition, Rotation.Identity ) );

		Gizmo.Draw.Color = Color.Red;
		Gizmo.Draw.Line( LocalTracePosition, Transform.LocalRotation.Forward * SAFE_WALL_SLOW_DISTANCE );
	}
}
