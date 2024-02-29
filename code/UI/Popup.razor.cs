using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class Popup : PanelComponent
{
	public struct Line
	{
		public string Message { get; set; }
		public Color Color { get; set; }
		public string Icon { get; set; }

		public Line( string message, Color color, string icon = "" )
		{
			Message = message;
			Color = color;
			Icon = icon;
		}
	}

	public static Popup Current { get; private set; }
	private List<Line> popupQueue { get; set; } = new();

	public static void Add( Line notification )
	{
		Current?.popupQueue.Add( notification );
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		Current = this;
	}
	protected override void OnUpdate()
	{
		
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Time.Now );
	}
}
