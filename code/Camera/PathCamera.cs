using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bydrive;

public class PathCamera : Component
{
	[Property] public List<CameraShot> Shots { get; set; }
	[Property] public bool MoveOnEnabled { get; set; }
	[Property] public bool Loop { get; set; }
	[Property] public float TimeScale { get; set; } = 1;
	[Property] public Action OnFinishMove { get; set; }
	public bool Finished { get; set; }
	private CameraComponent Camera => Game.ActiveScene.Camera;
	float time => timeSinceStart * TimeScale;
	float totalDuration;
	TimeSince timeSinceStart;
	Dictionary<float, CameraShot> startTimes;
	bool hasFinished;
	public float GetTotalDuration()
	{
		return Shots.Sum( s => s.Duration );
	}
	private Dictionary<float, CameraShot> CalculateStartTimes()
	{
		if(Shots == null || !Shots.Any()) return null;

		float time = 0;
		Dictionary<float, CameraShot> times = new();
		foreach(var shot in Shots)
		{
			times.Add(time, shot);
			time += shot.Duration;
		}

		return times;
	}
	public void Start()
	{
		timeSinceStart = 0;
		totalDuration = GetTotalDuration();
		startTimes = CalculateStartTimes();
		hasFinished = true;
	}
	public void Stop()
	{
		hasFinished = true;
	}

	protected override void OnEnabled()
	{
		if ( MoveOnEnabled )
			Start();
	}
	protected override void OnUpdate()
	{
		if( time >= totalDuration)
		{
			if(Loop)
			{
				timeSinceStart = 0;
			}
			else
			{
				Finished = true;
			}
		}

		if(!Finished)
		{
			(float shotStart, CameraShot currentShot) = startTimes.LastOrDefault( kv => kv.Key <= time );
			float shotFraction = (time - shotStart) / currentShot.Duration;
			UpdateCamera( currentShot, shotFraction );

		}
		else if(!hasFinished)
		{
			FinishMove();
		}
	}
	private void FinishMove()
	{
		var lastTransform = Shots.LastOrDefault()?.EndTransform ?? Transform.World;
		Camera.Transform.Position = lastTransform.Position;
		Camera.Transform.Rotation = lastTransform.Rotation;

		OnFinishMove?.Invoke();
	}
	private void UpdateCamera(CameraShot shot, float frac)
	{
		Transform start = shot.StartTransform;
		Transform end = shot.EndTransform;
		if(shot.EnablePosition)
		{
			float positionFraction = shot.PositionCurve.EvaluateDelta( frac );
			Camera.Transform.Position = start.Position.LerpTo( end.Position, positionFraction, false );
		}

		if(shot.EnableRotation)
		{
			float rotationFraction = shot.RotationCurve.EvaluateDelta( frac );
			Camera.Transform.Rotation = Rotation.Slerp( start.Rotation, end.Rotation, rotationFraction, false );
		}
	}

	protected override void DrawGizmos()
	{
		const float ROTATION_VERTICAL_OFFSET = 24f;
		const float ROTATION_FORWARD_DISTANCE = 16f;
		const float DURATION_TEXT_OFFSET = 16f;

		if ( !Gizmo.IsSelected ) return;

		int shotNum = 1;
		foreach(var shot in Shots)
		{
			Transform start = Transform.World.ToLocal( shot.StartTransform );
			Transform end = Transform.World.ToLocal( shot.EndTransform );

			// Draw movement
			Gizmo.Draw.Color = Color.Green;
			Gizmo.Draw.Arrow( start.Position, end.Position );
			Gizmo.Draw.WorldText( $"Shot {shotNum}: {shot.Duration}", new Transform(start.Position + Vector3.Down * DURATION_TEXT_OFFSET, Rotation.FromAxis(Vector3.Forward, 90f)) );

			// Draw rotations
			Gizmo.Draw.Color = Color.Yellow;
			Gizmo.Draw.Arrow( start.Position + Vector3.Up * ROTATION_VERTICAL_OFFSET, start.Position + Vector3.Up * ROTATION_VERTICAL_OFFSET + start.Rotation.Forward.Normal * ROTATION_FORWARD_DISTANCE, arrowWidth: 3f );
			Gizmo.Draw.Arrow( end.Position + Vector3.Up * ROTATION_VERTICAL_OFFSET, end.Position + Vector3.Up * ROTATION_VERTICAL_OFFSET + end.Rotation.Forward.Normal * ROTATION_FORWARD_DISTANCE, arrowWidth: 3f );

			shotNum++;
		}
	}
}

public class CameraShot
{
	public float Duration { get; set; } = 1f;
	public GameObject StartMarker { get; set; }
	public GameObject EndMarker { get; set; }
	[Hide, JsonIgnore] public Transform StartTransform => StartMarker?.Transform.World ?? Transform.Zero;
	[Hide, JsonIgnore] public Transform EndTransform => EndMarker?.Transform.World ?? Transform.Zero;
	[ToggleGroup("EnablePosition", Label = "Position" )]
	public bool EnablePosition { get; set; } = false;
	[ToggleGroup( "EnablePosition")]
	public Curve PositionCurve { get; set; }

	[ToggleGroup("EnableRotation", Label = "Rotation")]
	public bool EnableRotation { get; set; }
	[ToggleGroup("EnableRotation")]
	public Curve RotationCurve { get; set; }

	public override string ToString()
	{
		return $"{Duration:0.00}s";
	}
}
