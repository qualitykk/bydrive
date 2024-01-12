using Editor.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive.Tools;

[Dock("Editor", "Icon Lookup", "search")]
public class IconLookupWidget : Widget
{
	const string ICON_OVERVIEW_WEBSITE = "https://materialui.co/icons";
	WebWidget webpage;
	public IconLookupWidget(Widget parent) : base(parent)
	{
		webpage = new( this );
		webpage.Size = new( 1920, 1080 );
		webpage.HorizontalSizeMode = SizeMode.Flexible;
		webpage.VerticalSizeMode = SizeMode.Flexible;
		webpage.Surface.Url = ICON_OVERVIEW_WEBSITE;
	}
}
