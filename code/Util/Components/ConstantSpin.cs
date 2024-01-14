using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon("refresh")]
public class ConstantSpin : Component
{
	public const float DEGREES_PER_SECOND = 45f;
	[Property] public float Speed { get; set; } = 1;

	protected override void OnUpdate()
	{
		float rotateDegrees = Speed * DEGREES_PER_SECOND * Time.Delta;
		Transform.Rotation = Transform.Rotation.RotateAroundAxis( Vector3.Up, rotateDegrees );
	}
}
