using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public abstract class SingletonComponent<TSelf> : Component, IHotloadManaged where TSelf : SingletonComponent<TSelf>
{
	public static TSelf Instance { get; private set; }

	protected override void OnAwake()
	{
		if ( Active )
		{
			Instance = (TSelf)this;
		}
	}

	void IHotloadManaged.Destroyed( Dictionary<string, object> state )
	{
		state["IsActive"] = Instance == this;
	}

	void IHotloadManaged.Created( IReadOnlyDictionary<string, object> state )
	{
		if ( state.GetValueOrDefault( "IsActive" ) is true )
		{
			Instance = (TSelf)this;
		}
	}

	protected override void OnDestroy()
	{
		if ( Instance == this )
		{
			Instance = null;
		}
	}
}
