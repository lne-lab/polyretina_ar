#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

using LNE.ArrayExts;
using LNE.IO;

public class LineLengthAnalysis : MonoBehaviour
{
	[MenuItem("Polyretina/.../Retrieve Line Lengths")]
	static void RetrieveLineLengths()
	{
		var directory = EditorUtility.OpenFolderPanel("Select Directory...", "", "");
		var savePath = EditorUtility.SaveFilePanel("Save file...", "", "", "csv");

		SaveAllLineDescs(savePath, directory, "cs", "shader", "cginc");
	}

	[MenuItem("Polyretina/.../Retrieve File Lengths")]
	static void RetrieveFileLengths()
	{
		var directory = EditorUtility.OpenFolderPanel("Select Directory...", "", "");
		var savePath = EditorUtility.SaveFilePanel("Save file...", "", "", "csv");

		SaveAllFileDescs(savePath, directory, "cs", "shader", "cginc");
	}

	public static void SaveAllLineDescs(string savePath, string directory, params string[] extensions)
	{
		var descs = GetAllLineDescs(directory, extensions);

		var csv = new CSV();
		csv.AppendColumn(descs.Convert((d) => d.file));
		csv.AppendColumn(descs.Convert((d) => d.line.ToString()));
		csv.AppendColumn(descs.Convert((d) => d.length.ToString()));
		csv.SaveWStream(savePath);
	}

	public static LineDesc[] GetAllLineDescs(string directory, params string[] extensions)
	{
		var descs = new List<LineDesc>();

		foreach (var file in GetAllFiles(directory, extensions))
		{
			descs.AddRange(
				GetFileLineDescs(file)
			);
		}

		return descs.ToArray();
	}

	public static string[] GetAllFiles(string directory, params string[] extensions)
	{
		var allFiles = new List<string>();

		foreach (var extension in extensions)
		{
			var files = Directory.GetFiles(directory, $"*.{extension}", SearchOption.AllDirectories);
			allFiles.AddRange(files);
		}

		return allFiles.ToArray();
	}

	public static LineDesc[] GetFileLineDescs(string path)
	{
		var descs = new List<LineDesc>();
		var lines = File.ReadAllLines(path);

		for (int i = 0; i < lines.Length; i++)
		{
			if (IsWhiteNoiseAndCurlyBracket(lines[i]))
			{
				continue;
			}

			var desc = new LineDesc {
				file = Path.GetFileName(path),
				line = i,
				length = lines[i].Replace("\t", "    ").Length
			};

			descs.Add(desc);
		}

		return descs.ToArray();
	}

	public static bool IsWhiteNoiseAndCurlyBracket(string line)
	{
		return Regex.IsMatch(line, @"\s?{|}");
	}

	public static void SaveAllFileDescs(string savePath, string directory, params string[] extensions)
	{
		var descs = GetAllFileDescs(directory, extensions);

		var csv = new CSV();
		csv.AppendColumn(descs.Convert((d) => d.name));
		csv.AppendColumn(descs.Convert((d) => d.length.ToString()));
		csv.SaveWStream(savePath);
	}

	public static FileDesc[] GetAllFileDescs(string directory, params string[] extensions)
	{
		var descs = new List<FileDesc>();

		foreach (var file in GetAllFiles(directory, extensions))
		{
			descs.Add(GetFileDesc(file));
		}

		return descs.ToArray();
	}

	public static FileDesc GetFileDesc(string path)
	{
		return new FileDesc() {
			name = Path.GetFileName(path),
			length = File.ReadAllLines(path).Length
		};
	}

	public class FileDesc
	{
		public string name;
		public int length;
	}

	public class LineDesc
	{
		public string file;
		public int line;
		public int length;
	}
}

#endif
