using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class Popup : PanelComponent
{
	private class QueueEntry
	{
		public PopupPage[] Pages;

		public QueueEntry( PopupPage[] pages )
		{
			Pages = pages;
		}

		public QueueEntry(PopupPage page)
		{
			Pages = new PopupPage[1] { page };		
		}
	}
	public static Popup Instance { get; private set; }
	private List<QueueEntry> popupQueue { get; set; } = new();
	private QueueEntry currentPopup;
	private int currentPageNumber;
	private int currentPageMax => currentPopup.Pages.Length - 1;
	private PopupPage currentPage => currentPopup?.Pages?.ElementAtOrDefault( currentPageNumber ) ?? default;
	public static void Add( PopupPage notification )
	{
		Instance?.popupQueue.Add( new(notification) );
	}
	public static void Add(IEnumerable<PopupPage> pages)
	{
		Instance?.popupQueue.Add( new(pages.ToArray()));
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		Instance = this;
	}
	protected override void OnUpdate()
	{
		if(popupQueue.Any() && currentPopup == null)
		{
			currentPopup = popupQueue.First();
			popupQueue.RemoveAt( 0 );
			UI.MakeMenu( Panel );
		}

		if(currentPopup != null)
		{
			if(Input.Pressed(InputActions.DIALOG_SKIP) || Input.Pressed(InputActions.USE)) 
			{
				if(currentPageNumber == currentPageMax ) 
				{
					currentPopup = null;
					currentPageNumber = 0;
					UI.MakeMenuInactive( Panel );
				}
				else
				{
					currentPageNumber++;
				}
			}
		}
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( currentPopup, popupQueue, currentPage );
	}

	[ConCmd("ui_show_popup")]
	private static void Command_ShowPopup(string title, string message, string color = "")
	{
		if(string.IsNullOrEmpty(color))
		{
			color = UI.Colors.Popup.Info.ToString(true, true);
		}

		Add( new PopupPage( title, message, Color.Parse( color ) ?? UI.Colors.Popup.Info ) );
	}
}

public struct PopupPage
{
	public string Title { get; set; }
	public string Message { get; set; }
	public string Image { get; set; }
	public Color Color { get; set; }
	public PopupPage( string title, string message, Color color )
	{
		Title = title;
		Message = message;
		Color = color;
	}
}
