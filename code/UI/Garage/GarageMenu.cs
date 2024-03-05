using Sandbox.UI;

namespace Bydrive;
[StyleSheet]
public class GarageMenu : PanelComponent
{
	private NavHostPanel content;
	protected override void OnStart()
	{
		base.OnStart();
	
		content = Panel.AddChild<NavHostPanel>();
		content.DefaultUrl = "/";
	}
}
