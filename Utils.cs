using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CreatioConsoleHelper
{
	public static class Utils
	{

		public static bool OnlyShow { get; set; } = false;

		public static string SchemasDirectoryName = "Schemas";
		public static string ResourcesDirectoryName = "Resources";

		public static Regex BetweenQuotes = new Regex(@"""(.*?)""");

		public static string GetEntitySchemaName(string filePath, bool silent = false)
		{
			string[] lines = File.ReadAllLines(filePath);
			string firstOccurrence = lines.FirstOrDefault(l => l.Contains("entitySchemaName"));
			if (firstOccurrence != null)
			{
				var match = BetweenQuotes.Match(firstOccurrence);
				if (match.Length > 1)
				{
					return match.Groups[1].Value;
				}
			}
			if (!silent)
			{
				throw new Exception($"EntitySchemaName not found in {filePath}");
			}
			return String.Empty;
		}

		public static string GetFolderNameFromPath(string path)
		{
			return path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
		}

		public static void RenameFile(string filePath, string newName)
		{
			string newPath = Path.Combine(Path.GetDirectoryName(filePath), newName + Path.GetExtension(filePath));
			if (!OnlyShow)
			{
				File.Move(filePath, newPath);
			}
			Console.WriteLine($" - renamed {Path.GetFileName(filePath)} to {Path.GetFileName(newPath)}");
		}

		public static void RenameDirectory(string dirPath, string newName)
		{
			if (!Directory.Exists(dirPath))
			{
				Console.WriteLine($" - dir not found {dirPath}");
				return;
			}
			string newPath = Path.Combine(Path.GetDirectoryName(dirPath), newName);
			if (!OnlyShow)
			{
				Directory.Move(dirPath, newPath);
			}
			Console.WriteLine($" - renamed dir {dirPath} to {newPath}");
		}

		public static void ReplaceInAllFiles(string pkgPath, string fileMask, string oldValue, string newValue)
		{
			Console.WriteLine($" - Replace text in all files");
			List<string> dirs = new List<string>(Directory.EnumerateDirectories(pkgPath));
			foreach (var dir in dirs)
			{
				string packageName = Utils.GetFolderNameFromPath(dir);
				List<string> packagesDirs = new List<string>(Directory.EnumerateDirectories(dir));
				foreach (var packageDir in packagesDirs)
				{
					string packageDirName = Utils.GetFolderNameFromPath(packageDir);
					if (packageDirName == Utils.SchemasDirectoryName)
					{
						List<string> schemasDirs = new List<string>(Directory.EnumerateDirectories(packageDir));
						foreach (var schemaDir in schemasDirs)
						{
							string[] schemaFiles = Directory.GetFiles(schemaDir, fileMask);
							foreach (var filePath in schemaFiles)
							{
								ReplaceTextInFile(filePath, oldValue, newValue);
							}
						}
						continue;
					}
				}
			}
		}

		public static bool SchemaExists(string pkgPath, string schemaName)
		{
			List<string> dirs = new List<string>(Directory.EnumerateDirectories(pkgPath));
			foreach (var dir in dirs)
			{
				string packageName = Utils.GetFolderNameFromPath(dir);
				List<string> packagesDirs = new List<string>(Directory.EnumerateDirectories(dir));
				foreach (var packageDir in packagesDirs)
				{
					string packageDirName = Utils.GetFolderNameFromPath(packageDir);
					if (packageDirName == Utils.SchemasDirectoryName)
					{
						if (Directory.EnumerateDirectories(packageDir, schemaName).Any())
						{
							return true;
						}
						continue;
					}
				}
			}
			return false;
		}

		public static void ReplaceTextInFile(string filePath, string oldValue, string newValue)
		{
			string fileName = Path.GetFileName(filePath);
			string fileText = File.ReadAllText(filePath);
			if (fileText.Contains(oldValue))
			{
				if (!OnlyShow)
				{
					File.WriteAllText(filePath, fileText.Replace(oldValue, newValue));
				}
				Console.WriteLine($"  -- replaced {fileName}");
			}
		}
	}
}
