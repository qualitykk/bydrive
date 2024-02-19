using Sandbox.Citizen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class OverworldController : Component
{
	[Property] public CharacterController WalkMovement { get; set; }
	[Property] public CitizenAnimationHelper Animator { get; set; }
	[Property] public float Speed { get; set; } = 220f;
	[Property] public float RotationSpeed { get; set; } = 5f;
	[Property] public Vector3 CameraOffset { get; set; }
	public Vector3 CameraCenter => Transform.Position.WithZ( CameraOffset.z );
	Rotation cameraRotation;

	Vector3 wishVelocity;
	Rotation wishRotation;

	RealTimeSince lastGrounded;
	RealTimeSince lastUngrounded;
	protected override void OnStart()
	{
		cameraRotation = Transform.Rotation;
	}
	float GetFriction()
	{
		if ( WalkMovement.IsOnGround ) return 6.0f;

		// air friction
		return 0.2f;
	}
	protected override void OnUpdate()
	{
		Look();
		Move();
		UpdateAnimation();
	}
	private void Look()
	{
		const float CAMERA_MAX_PITCH = 15f;

		Angles cameraAngle = cameraRotation;
		cameraAngle += Input.AnalogLook;
		cameraAngle.pitch = cameraAngle.pitch.Clamp( -CAMERA_MAX_PITCH, CAMERA_MAX_PITCH );
		cameraAngle.roll = 0.0f;

		cameraRotation = cameraAngle;
	}

	private void Move()
	{
		if ( WalkMovement is null )
			return;

		CharacterController cc = WalkMovement;

		Vector3 halfGravity = Scene.PhysicsWorld.Gravity * Time.Delta * 0.5f;

		wishVelocity = Input.AnalogMove;


		if ( !wishVelocity.IsNearlyZero() )
		{
			wishVelocity = new Angles( 0, cameraRotation.Yaw(), 0 ).ToRotation() * wishVelocity;
			wishVelocity = wishVelocity.WithZ( 0 );
			wishVelocity = wishVelocity.ClampLength( 1 );
			wishVelocity *= Speed;

			if ( !cc.IsOnGround )
			{
				wishVelocity = wishVelocity.ClampLength( 50 );
			}
		}


		cc.ApplyFriction( GetFriction() );

		if ( cc.IsOnGround )
		{
			cc.Accelerate( wishVelocity );
			cc.Velocity = WalkMovement.Velocity.WithZ( 0 );
		}
		else
		{
			cc.Velocity += halfGravity;
			cc.Accelerate( wishVelocity );

		}

		cc.Move();

		if ( !cc.IsOnGround )
		{
			cc.Velocity += halfGravity;
		}
		else
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
		}

		if ( cc.IsOnGround )
		{
			lastGrounded = 0;
		}
		else
		{
			lastUngrounded = 0;
		}

		if(wishVelocity != default)
		{
			wishRotation = Rotation.LookAt( wishVelocity );
		}
		Transform.Rotation = Rotation.Slerp(Transform.Rotation, wishRotation, Time.Delta * RotationSpeed);
	}

	private void UpdateCamera()
	{
		CameraComponent camera = GameManager.ActiveScene.Camera;
		if ( camera == null ) return;

		Vector3 targetCameraPos = Transform.Position + CameraOffset * Rotation.LookAt(cameraRotation.Forward);

		// TODO Dont allow camera to go through walls
		var wallClipTrace = Scene.Trace.Ray( CameraCenter, targetCameraPos )
										.Size(WalkMovement.BoundingBox)
										.WithTag("world")
										.Run();

		if ( wallClipTrace.Hit )
		{
			Vector3 safePositionDirection = wallClipTrace.EndPosition - CameraCenter;
			targetCameraPos = CameraCenter + safePositionDirection * 0.90f;
		}

		// smooth view z, so when going up and down stairs or ducking, it's smooth af
		if ( lastUngrounded > 0.2f )
		{
			targetCameraPos.z = camera.Transform.Position.z.LerpTo( targetCameraPos.z, RealTime.Delta * 25.0f );
		}

		camera.Transform.Position = targetCameraPos;
		camera.Transform.Rotation = cameraRotation;
		camera.FieldOfView = 90f;
	}

	protected override void OnPreRender()
	{
		UpdateCamera();
	}

	private void UpdateAnimation()
	{
		if ( Animator is null ) return;

		var wv = wishVelocity.Length;

		Animator.WithWishVelocity( wishVelocity );
		Animator.WithVelocity( WalkMovement.Velocity );
		Animator.IsGrounded = WalkMovement.IsOnGround;

		/*
		var lookDir = eyeRotation.Forward * 1024;
		Animator.WithLook( lookDir, 1, 0.5f, 0.25f );
		*/
	}

	protected override void DrawGizmos()
	{
		if ( !Gizmo.IsSelected ) return;

		Gizmo.Draw.Color = Color.Blue;
		Gizmo.Draw.SolidSphere( CameraOffset, 4f );
	}
}
