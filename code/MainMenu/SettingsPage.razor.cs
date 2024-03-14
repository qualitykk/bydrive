
namespace Bydrive;

public partial class SettingsPage
{
	int page;
	void ClickPage(int num)
	{
		page = num;
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( page, Settings );
	}
}
