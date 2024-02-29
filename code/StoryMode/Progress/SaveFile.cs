using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bydrive;

public partial class SaveFile
{
	public const string SAVE_DIRECTORY = "/story";
	public const string SAVE_PATTERN = "*.save";
	private static BaseFileSystem fs => FileSystem.Data;
	public static SaveFile[] GetAll()
	{
		IEnumerable<string> fileNames = fs.FindFile(SAVE_DIRECTORY, SAVE_PATTERN).Select(file => $"{SAVE_DIRECTORY}/{file}");

		SaveFile[] files = fileNames.Select( Load ).ToArray();
		return files;
	}
	public static SaveFile Load(string path)
	{
		return fs.ReadJsonOrDefault<SaveFile>( path );
	}
	public static void Save(string path, SaveFile file) 
	{
		if(!fs.DirectoryExists(SAVE_DIRECTORY))
		{
			fs.CreateDirectory( SAVE_DIRECTORY );
		}

		fs.WriteJson( path, file );
	}
	public static SaveFile Create()
	{
		SaveFile file = new();
		file.Id = Guid.NewGuid();
		file.Save();

		return file;
	}
	public Guid Id { get; set; }
	public string CharacterName { get; set; } = "MissingNo";
	public float Playtime { get; set; }
	public Transform LastTransform { get; set; }
	public void Save()
	{
		string path = GetFilePath();
		Save( path, this );
	}
	public void AutoSave()
	{
		string path = GetFilePath();
		path += $"_autosave_{DateTime.UtcNow}";

		Save( path, this );
	}
	private string GetFilePath()
	{
		return $"{SAVE_DIRECTORY}/{Id}.save";
	}

	public override string ToString()
	{
		return $"{Id}/{CharacterName}";
	}
}
