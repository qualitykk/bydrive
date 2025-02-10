using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

/// <summary>
/// GameObject is enabled if race variable of key matches value and disabled otherwise.
/// </summary>
public class TrackVariableToggle : Component
{
	[Property] public string Key { get;set; }
	[Property] public string Value { get;set; }
	[Property] public bool FullMatch { get;set; }
	[Property] public bool Invert { get;set; }
}
