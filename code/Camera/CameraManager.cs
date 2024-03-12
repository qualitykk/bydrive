using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon( "switch_video" )]
public class CameraManager : Component
{
	public static CameraManager Instance { get; private set; }
	public static void MakeActive(ICameraMode mode, int priority = 0)
	{
		Instance?.SetCameraMode(mode, priority);
	}
	public static void MakeInactive(ICameraMode mode)
	{
		Instance?.StopCameraMode( mode );
	}
	[Property] public CameraComponent Camera { get; set; }
	public ICameraMode CurrentCameraMode { get; private set; }
	private int currentPriority;
	public CameraManager()
	{
		Instance = this;
	}
	public void SetCameraMode(ICameraMode mode, int priority = 0)
	{
		if ( priority < currentPriority ) return;

		CurrentCameraMode = mode;
		currentPriority = priority;
	}
	public void StopCameraMode(ICameraMode mode)
	{
		if(CurrentCameraMode == mode)
		{
			CurrentCameraMode = null;
			currentPriority = 0;
		}
	}
	protected override void OnUpdate()
	{
		if(CurrentCameraMode != null)
		{
			CurrentCameraMode.UpdateCamera( Camera ?? Game.ActiveScene.Camera );
		}
	}
}
