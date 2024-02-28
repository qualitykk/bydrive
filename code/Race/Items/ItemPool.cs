using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;


[GameResource("Item Pool", "itempool", "Defines items with chances to be granted by pickups.")]
public class ItemPool : GameResource 
{
	public class Entry
	{
		public ItemDefinition Item { get; set; }
		/// <summary>
		/// How many slots this takes up in the item roulette.
		/// </summary>
		[MinMax(1f, 25f)] public int Weight { get; set; } = 1;
	}

	public List<Entry> Entries { get; set; } = new();
	public List<ItemDefinition> GetItemsWeighted()
	{
		List<ItemDefinition> items = new();
		foreach ( var entry in Entries )
		{
			for ( int i = 0; i < entry.Weight; i++ )
			{
				items.Add( entry.Item );
			}
		}

		return items;
	}
	public ItemDefinition GetRandom()
	{
		var roulette = GetItemsWeighted();

		return Game.Random.FromList( roulette );
	}
}
