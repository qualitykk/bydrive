using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class VehicleItemEvents : Component
{
	[Property] public Action<VehicleController> OnItemUsed { get; set; }
}
