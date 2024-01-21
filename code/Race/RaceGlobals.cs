using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon( "tune" )]
public class RaceGlobals : Component
{
	public static RaceGlobals Current { get; private set; }
	[Property] public MapInstance Level { get; set; }
	[Property] public GameObject Track { get; set; }
	protected override void OnAwake()
	{
		Current = this;
	}
}
