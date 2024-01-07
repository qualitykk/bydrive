using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public abstract class VehicleStatsProvider : Component
{
	public abstract VehicleStats GetStats();
}
