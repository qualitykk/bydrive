using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[AttributeUsage(AttributeTargets.Property)]
public class AutoReferenceAttribute : Attribute
{
	/// <summary>
	/// Only set resource reference if property is unset
	/// </summary>
	public bool UnsetOnly { get; set; } 

	public AutoReferenceAttribute( bool unsetOnly = true )
	{
		UnsetOnly = unsetOnly;
	}
}
