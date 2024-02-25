using Bydrive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class Notifications : PanelComponent
{
	const float DEFAULT_NOTIFICATION_TIME = 5f;
	const float FADE_TIME = 1.5f;
	public struct NotificationInstance
	{
		public string Message { get; set; }
		public Color Color { get; set; }
		public TimeUntil TimeUntilDeletion { get; set; }
		public string Icon { get; set; }

		public NotificationInstance( string message, Color color, float time = DEFAULT_NOTIFICATION_TIME, string icon = "" )
		{
			Message = message;
			Color = color;
			TimeUntilDeletion = time;
			Icon = icon;
		}

		public float GetOpacity()
		{
			if ( TimeUntilDeletion <= FADE_TIME )
			{
				return TimeUntilDeletion.Relative / FADE_TIME;
			}

			return 1f;
		}
	}

	public static Notifications Current { get; private set; }
	private List<NotificationInstance> activeNotifications { get; set; } = new();

	private static void AddLine( NotificationInstance notification )
	{
		Current?.activeNotifications.Add( notification );
	}
	public static void Broadcast( NotificationInstance instance )
	{
		AddLine( instance );
	}
	public static void Add( VehicleController vehicle, NotificationInstance notification )
	{
		if ( GetLocalVehicle() != vehicle )
			return;

		AddLine( notification );
	}
	public static void Add( RaceParticipant participant, NotificationInstance notification )
	{
		if ( GetLocalParticipantInstance() != participant )
			return;

		AddLine( notification );
	}
	public static void Add( NotificationInstance notification )
	{
		Add( GetLocalParticipantInstance(), notification );
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		Current = this;
	}
	protected override void OnUpdate()
	{
		base.OnUpdate();
		foreach ( NotificationInstance line in activeNotifications.ToArray() )
		{
			if ( line.TimeUntilDeletion )
			{
				activeNotifications.Remove( line );
			}
		}
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Time.Now );
	}
}
