using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bydrive;

public class SaveFile
{
	public const string SAVE_DIRECTORY = "/story";
	public const string SAVE_PATTERN = "*.save";
	public static SaveFile[] GetAll()
	{
		var files = FileSystem.Data.FindFile(SAVE_DIRECTORY, SAVE_PATTERN);

		return files.Select( Load ).ToArray();
	}
	public static SaveFile Load(string path)
	{
		return JsonSerializer.Deserialize<SaveFile>(path);
	}
	public string CharacterName { get; set; }
	public float Playtime { get; set; }
}
