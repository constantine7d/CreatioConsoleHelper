using System;
using System.Collections.Generic;
using System.IO;

namespace CreatioConsoleHelper
{
	public class RenameDetailAndPage : RenameSchema
	{
		public bool Valid { get; set; } = true;
		public bool MoreThenTwoDetail { get; set; }
		public bool OnlyPage { get; set; }

		public RenameSchema Detail { get; set; }

		public string EntitySchemaName { get; set; }

		public RenameDetailAndPage(string pkgPath, string packageName, string pagePath) : base(pkgPath, packageName, pagePath)
		{
			SetEntitySchemaName();
			SetDetail();
		}

		private void SetEntitySchemaName()
		{
			EntitySchemaName = Utils.GetEntitySchemaName(Path.Combine(this.DirectoryPath, NameCurrent + ".js"));
		}

		private void SetDetail()
		{
			int count = 0;
			List<string> schemasDirs = new List<string>(Directory.EnumerateDirectories(Path.Combine(PkgPath, PackageName, Utils.SchemasDirectoryName)));
			string filePath;
			foreach (var schemaDir in schemasDirs)
			{
				string schemaName = Utils.GetFolderNameFromPath(schemaDir);
				if (schemaName.Contains("Detail"))
				{
					filePath = Path.Combine(schemaDir, schemaName + ".js");
					if (!File.Exists(filePath))
					{
						continue;
					}
					if (Utils.GetEntitySchemaName(filePath, true) == this.EntitySchemaName)
					{
						this.Detail = new RenameSchema(PkgPath, PackageName, schemaDir);
						Console.WriteLine($"Found detail {this.Detail}");
						count++;
					}
				}
			}
			if (count > 1)
			{
				Console.WriteLine("Found more then one detail in " + this.ToString());
				Valid = false;
				MoreThenTwoDetail = true;
				this.Detail = null;
			}
			if (this.Detail == null)
			{
				Console.WriteLine("Detail not found in " + this.ToString());
				Valid = false;
				OnlyPage = true;
				this.Detail = null;
			}
		}

		public override void Rename(IEnumerable<RenameSchema> listForRename)
		{
			if (Valid)
			{
				base.Rename(listForRename);
				Detail?.Rename(listForRename);
			}
			else
			{
				Console.WriteLine($" - Not valid {this}");
			}
		}

		public override string ToString()
		{
			return $"Page={PackageName}.{NameCurrent}, EntitySchema={EntitySchemaName}";
		}
	}

}
