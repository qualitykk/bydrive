using Sandbox.UI;

namespace Bydrive;

public partial class RaceNotifications : Panel
{
	const float DEFAULT_NOTIFICATION_TIME = 5f;
	const float FADE_TIME = 1.5f;
	public struct Line
	{
		public string Message { get; set; }
		public Color Color { get; set; }
		public TimeUntil TimeUntilDeletion { get; set; }
		public string Icon { get; set; }

		public Line( string message, Color color, float time = DEFAULT_NOTIFICATION_TIME, string icon = "" )
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

	public static RaceNotifications Current { get; private set; }
	private List<Line> activeNotifications { get; set; } = new();

	private static void AddLine( Line notification )
	{
		Current?.activeNotifications.Add( notification );
	}
	public static void Broadcast( Line instance )
	{
		AddLine( instance );
	}
	public static void AddObject( VehicleController vehicle, Line notification )
	{
		if ( GetLocalVehicle() != vehicle )
			return;

		AddLine( notification );
	}
	public static void AddObject( RaceParticipant participant, Line notification )
	{
		if ( GetLocalParticipant() != participant )
			return;

		AddLine( notification );
	}
	public static void AddLineLocal( Line notification )
	{
		AddObject( GetLocalParticipant(), notification );
	}

	public RaceNotifications()
	{
		Current = this;
	}
	public override void Tick()
	{
		foreach ( Line line in activeNotifications.ToArray() )
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
