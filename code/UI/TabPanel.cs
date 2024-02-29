using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sandbox.UI.TabContainer;

namespace Bydrive;

[Alias("tab")]
public class TabPanel : Panel
{
	public const string ACTIVE_CLASS = "active";
	public const string INACTIVE_CLASS = "hidden";
	const int DEFAULT_INDEX = 0;
	public int Index { get; set; } = DEFAULT_INDEX;
	protected override void OnParametersSet()
	{
		SetClass( ACTIVE_CLASS, Index == DEFAULT_INDEX );
		SetClass( INACTIVE_CLASS, Index != DEFAULT_INDEX );
	}
}

public static class TabPanelExtensions
{
	public static void SelectTab(this Panel p, int index)
	{
		var tabs = p.Descendants.OfType<TabPanel>();
		foreach(TabPanel tab in tabs)
		{
			tab.SetClass( TabPanel.ACTIVE_CLASS, tab.Index == index );
			tab.SetClass( TabPanel.INACTIVE_CLASS, tab.Index != index );
		}
	}
}
