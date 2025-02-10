using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtuality;
public class DontDestroyOnLoad : Component
{
	protected override void OnEnabled()
	{
		GameObject.Flags |= GameObjectFlags.DontDestroyOnLoad;
	}

	protected override void OnDisabled()
	{
		GameObject.Flags = GameObject.Flags & ~GameObjectFlags.DontDestroyOnLoad;
	}
}
