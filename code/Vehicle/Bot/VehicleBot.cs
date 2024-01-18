using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class VehicleBot : VehicleInputComponent
{
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

		Vector3 goalDirection = goal - Transform.Position;
		Vector3 localGoal = Transform.World.PointToLocal( goal );

		// Calculate if the direction to the goal is left or right form our forward vector. (https://discussions.unity.com/t/steer-towards-target/186660)
		float turnDirection = Vector3.Dot( Vector3.Cross( Transform.Rotation.Forward, goalDirection.Normal ), Vector3.Up);
		float throttle = MathF.Sign( localGoal.x );

		const float MIN_TURN_INPUT = 0.2f;

		turnDirection = MathF.Max(MathF.Abs(turnDirection), MIN_TURN_INPUT) * MathF.Sign(turnDirection);

		VehicleController.TurnInput = turnDirection;
		VehicleController.ThrottleInput = throttle;
		//VehicleController.ThrottleInput = 0.8f;
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

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Vector3 nextLocalPosition = Transform.World.PointToLocal( GetNextPosition() );
		Gizmo.Draw.Color = Color.Green;
		Gizmo.Draw.Line( Vector3.Zero, nextLocalPosition );
		Gizmo.Draw.WorldText( $"currentBotPath: {currentBotPath}", new() );

		Gizmo.Draw.Color = Color.Orange;
		Gizmo.Draw.Line( Vector3.Zero, Transform.LocalRotation.Forward * 256f );
	}
}
