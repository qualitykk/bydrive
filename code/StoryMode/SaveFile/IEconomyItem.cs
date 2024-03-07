using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public interface IEconomyItem
{
	string Title { get; }
	string Description { get; }
	float MoneyCost { get; }
	int TokenCost { get; }
	Story.UnlockCheck CanPurchase { get;}
}
