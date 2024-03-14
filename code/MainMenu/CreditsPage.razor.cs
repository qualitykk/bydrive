
namespace Bydrive;

public partial class CreditsPage
{
	private static readonly List<CreditsEntry> developmentEntries = new()
	{
		new(76561198835895479, "qualityk", "kilianq", "Main Developer"),
		new(76561198282489899, "Kanalratte", "kanalratte", "Models, Textures"),
		new(76561198162417562, "SlimesMcDingle", "slimesmcdingle", "Models, Textures"),
	};

	private static readonly List<CreditsEntry> attributionEntries = new()
	{ 
		new(0, "SkyernAklea", "", "Car Engine Sounds"),
		new(0, "inchadney", "", "City Soundscape")
	};
}

internal struct CreditsEntry
{
	public ulong SteamID { get; set; }
	public string Name { get; set; }
	public string Handle { get; set; }
	public string Credit { get; set; }

	public CreditsEntry( ulong steamID, string name, string handle, string credit )
	{
		SteamID = steamID;
		Name = name;
		Handle = handle;
		Credit = credit;
	}
}
