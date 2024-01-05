using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

internal static class TransformExtensions
{
	public static Vector3 VelocityToLocal( this Transform transform, Vector3 velocity )
	{
		return transform.PointToLocal( transform.Position + velocity );
	}
}
