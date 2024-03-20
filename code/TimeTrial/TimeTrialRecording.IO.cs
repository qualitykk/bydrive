using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class TimeTrialRecording
{
	const string RACE_DATA_OLD = "time_trials.json";
	const string RACE_DATA_DIRECTORY = "/time_trials";
	const string RACE_DATA_PATTERN = "*.trial";
	private static string GetPath( string track, long player, Guid id )
	{
		Assert.NotNull( player );
		Assert.NotNull( id );

		return $"{GetTrackPath( track )}/{player}_{id}.trial";
	}
	private static string GetPath( TimeTrialRecording recording ) => GetPath( recording.Track, recording.SteamId, recording.Id );
	private static string GetTrackPath( string track )
	{
		Assert.False( string.IsNullOrEmpty( track ) );
		string trackName = track.Split( '/', '\\' ).Last().Split( '.' ).First();
		return $"{RACE_DATA_DIRECTORY}/{trackName}";
	}

	public static List<TimeTrialRecording> Read( string track, Dictionary<string, string> trackVariables )
	{
		string trackPath = GetTrackPath( track );
		if ( !FileSystem.Data.DirectoryExists( trackPath ) )
		{
			FileSystem.Data.CreateDirectory( trackPath );
		}

		List<TimeTrialRecording> data = new();
		var timeTrialFiles = FileSystem.Data.FindFile( trackPath, RACE_DATA_PATTERN );
		foreach ( var file in timeTrialFiles )
		{
			TimeTrialRecording fileData = Read( $"{trackPath}/{file}" );
			data.Add( fileData );
		}

		var oldData = FileSystem.Data.ReadJson<List<TimeTrialRecording>>( RACE_DATA_OLD );
		if ( oldData != null && oldData.Any() )
		{
			data.AddRange( oldData.Where( d => d.Track == track ) );
			FileSystem.Data.DeleteFile( RACE_DATA_OLD );
			Write( oldData );
		}

		return data.Where( d => d.TrackVariables?.Any() == true && d.TrackVariables.SequenceEqual( trackVariables ) )?.ToList();
	}
	public static TimeTrialRecording Read( string path )
	{
		const int GUID_BYTEARRAY_SIZE = 16;
		TimeTrialRecording recording = new();

		if ( !FileSystem.Data.FileExists( path ) )
			return recording;

		var fileStream = FileSystem.Data.OpenRead( path );
		using var reader = new BinaryReader( fileStream, Encoding.UTF8, false );
		recording.Id = new Guid( reader.ReadBytes( GUID_BYTEARRAY_SIZE ) );
		recording.SteamId = reader.ReadInt64();
		recording.Track = reader.ReadString();
		recording.Vehicle = reader.ReadString();

		Dictionary<string, string> trackVariables = new();
		int variableCount = reader.ReadInt32();

		for ( int i = 0; i < variableCount; i++ )
		{
			string key = reader.ReadString();
			string value = reader.ReadString();

			trackVariables.Add( key, value );
		}

		recording.TrackVariables = trackVariables;

		List<TimestampedVehicleInput> inputs = new();
		int inputCount = reader.ReadInt32();

		for ( int i = 0; i < inputCount; i++ )
		{
			float time = reader.ReadSingle();

			float throttleInput = reader.ReadSingle();
			float turnInput = reader.ReadSingle();
			float breakInput = reader.ReadSingle();
			float tiltInput = reader.ReadSingle();
			bool itemInput = reader.ReadBoolean();
			bool boostInput = reader.ReadBoolean();

			VehicleInputState state = new( throttleInput, turnInput, breakInput, tiltInput, itemInput, boostInput );
			inputs.Add( new() { Time = time, Input = state } );
		}

		recording.Inputs = inputs;

		List<float> lapTimes = new();
		int lapCount = reader.ReadInt32();

		for ( int i = 0; i < lapCount; i++ )
			lapTimes.Add( reader.ReadSingle() );

		recording.LapTimes = lapTimes;

		return recording;
	}
	public static void Write( TimeTrialRecording data )
	{
		if ( !FileSystem.Data.DirectoryExists( GetTrackPath(data.Track) ) )
		{
			FileSystem.Data.CreateDirectory( GetTrackPath( data.Track ) );
		}

		var fileStream = FileSystem.Data.OpenWrite( GetPath(data) );
		using var writer = new BinaryWriter( fileStream, Encoding.UTF8, false );
		writer.Write( data.Id.ToByteArray() );
		writer.Write( data.SteamId );
		writer.Write( data.Track );
		writer.Write( data.Vehicle );

		writer.Write( data.TrackVariables.Count );
		foreach ( (string key, string value) in data.TrackVariables )
		{
			writer.Write( key );
			writer.Write( value );
		}

		writer.Write( data.Inputs.Count );
		foreach ( var timestamp in data.Inputs )
		{
			writer.Write( timestamp.Time );
			writer.Write( timestamp.Input.ThrottleInput );
			writer.Write( timestamp.Input.TurnInput );
			writer.Write( timestamp.Input.BreakInput );
			writer.Write( timestamp.Input.TiltInput );
			writer.Write( timestamp.Input.WantsItem );
			writer.Write( timestamp.Input.WantsBoost );
		}

		writer.Write( data.LapTimes.Count );
		foreach ( var lapTime in data.LapTimes )
			writer.Write( lapTime );
	}

	public static void Write( List<TimeTrialRecording> allData )
	{
		foreach ( TimeTrialRecording data in allData )
		{
			Write( data );
		}
	}
}
