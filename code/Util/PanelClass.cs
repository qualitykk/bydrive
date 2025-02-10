using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZCom;

internal static class PanelClass
{
	public static string If( bool value, string c)
	{
		return value ? c : "";
	}

	public static string If( Func<bool> action, string c)
	{
		if ( action == null ) return "";

		return action() ? c : "";
	}
	public static string IfEqual( object expected, object value, string c  )
	{
		if(value.Equals(expected))
		{
			return c;
		}

		return "";
	}
}
