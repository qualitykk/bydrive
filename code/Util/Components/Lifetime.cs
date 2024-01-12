using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

/// <summary>
/// Deletes the attached gameobject after a certain amount of time
/// </summary>
[Icon( "auto_delete" )]
public class Lifetime : Component
{
	const float DEFAULT_DELETE_TIME = 5f;
	[Property] public float Time { get; set; } = DEFAULT_DELETE_TIME;
	[ActionGraphInclude] public TimeUntil TimeUntilDeletion { get; private set; }
	protected override void OnEnabled()
	{
		TimeUntilDeletion = Time;
	}

	protected override void OnUpdate()
	{
		if(TimeUntilDeletion)
		{
			GameObject.Destroy();
		}
	}
}
