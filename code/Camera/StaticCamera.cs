using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class StaticCamera : Component, ICameraMode
{
	[Property] public GameObject Anchor { get; set; }
	protected override void OnEnabled()
	{
		if ( Anchor == null )
			Anchor = GameObject;
	}
	public void UpdateCamera( CameraComponent camera )
	{
		camera.WorldPosition = Anchor.WorldPosition;
		camera.WorldRotation = Anchor.WorldRotation;
	}
}
