using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CreatioConsoleHelper
{
	public class RenameSchema
	{
		public string PkgPath { get; set; }
		public string PackageName { get; set; }

		public string DirectoryPath { get; set; }
		public string NameCurrent { get; set; }
		public string NameNew { get; set; }

		public bool NeedRename { get; private set; }

		public bool DoubleExists { get; private set; }
		public bool Renamed { get; private set; }

		public RenameSchema(string pkgPath, string packageName, string path)
		{
			this.PkgPath = pkgPath;
			this.PackageName = packageName;
			this.DirectoryPath = path;
			this.NameCurrent = Utils.GetFolderNameFromPath(path);
		}

		public override string ToString()
		{
			return $"{PackageName}.{NameCurrent}";
		}

		public virtual void Rename(IEnumerable<RenameSchema> listForRename)
		{
			CheckNeedRename();
			if (!NeedRename)
			{
				Console.WriteLine($"Rename not needed ({this})");
				return;
			}
			if (ExistsDouble(listForRename))
			{
				Console.WriteLine($"Double {NameNew} exists ({this})");
				return;
			}

			Console.WriteLine($"Rename {this} to {NameNew}");
			if (string.IsNullOrWhiteSpace(NameNew))
			{
				Console.WriteLine($" - NameNew is empty");
				return;
			}

			string[] schemaFiles = Directory.GetFiles(DirectoryPath, "*.*");

			Console.WriteLine($" - Replace text in schema");
			foreach (string filePath in schemaFiles)
			{
				Utils.ReplaceTextInFile(filePath, NameCurrent, NameNew);
				if (Path.GetFileNameWithoutExtension(Path.GetFileName(filePath)) == NameCurrent)
				{
					Utils.RenameFile(filePath, NameNew);
				}
			}
			Utils.RenameDirectory(DirectoryPath, NameNew);
			Utils.RenameDirectory(Path.Combine(PkgPath, PackageName, Utils.ResourcesDirectoryName, $"{NameCurrent}.ClientUnit"), $"{NameNew}.ClientUnit");
			Utils.ReplaceInAllFiles(PkgPath, "*.js", NameCurrent, NameNew);
			Renamed = true;
		}

		private void CheckNeedRename()
		{
			NeedRename = NameCurrent != NameNew;
		}

		private bool ExistsDouble(IEnumerable<RenameSchema> listForRename)
		{
			if (Utils.SchemaExists(PkgPath, NameNew) || listForRename.Any(it => it.NameNew == NameNew && !it.Equals(this)))
			{
				DoubleExists = true;
				return true;
			}
			return false;
		}
	}

}
