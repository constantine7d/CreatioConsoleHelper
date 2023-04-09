using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace CreatioConsoleHelper
{
	[Verb("renameschemas", HelpText = "Rename schemas")]
	public class RenameSchemasCommand : ICommand
	{
		[Option("onlyshow", Required = false, HelpText = "Only show schema list")]
		public bool OnlyShow { get; set; }

		[Option("pkgPath", Required = true, HelpText = "Pkg path")]
		public string PkgPath { get; set; }

		[Option("schemaPrefix", Required = true, HelpText = "Schema prefix")]
		public string SchemaPrefix { get; set; }
		public void Execute()
		{
			Console.WriteLine($"Executing Rename schemas: OnlyShow={OnlyShow}, PkgPath={PkgPath}, SchemaPrefix={SchemaPrefix}");
			Utils.OnlyShow = OnlyShow;
			//Todo log in cnfiguration directory

			List<RenameDetailAndPage> pages = new List<RenameDetailAndPage>();
			string doubleSchemaPrefix = SchemaPrefix + SchemaPrefix;
			//todo print list
			List<string> dirs = new List<string>(Directory.EnumerateDirectories(PkgPath));
			foreach (var dir in dirs)
			{
				string packageName = Utils.GetFolderNameFromPath(dir);
				//Console.WriteLine($"{packageName}");
				List<string> packagesDirs = new List<string>(Directory.EnumerateDirectories(dir));
				foreach (var packageDir in packagesDirs)
				{
					string packageDirName = Utils.GetFolderNameFromPath(packageDir);
					//Console.WriteLine($"	{packageDirName}");
					if (packageDirName == Utils.SchemasDirectoryName)
					{
						List<string> schemasDirs = new List<string>(Directory.EnumerateDirectories(packageDir));
						foreach (var schemaDir in schemasDirs)
						{
							string schemaDirName = Utils.GetFolderNameFromPath(schemaDir);
							if ((schemaDirName.StartsWith(doubleSchemaPrefix) || IsBadName(schemaDirName)) && schemaDirName.Contains("Page"))
							{
								Console.WriteLine($"		{schemaDirName}");
								pages.Add(new RenameDetailAndPage(PkgPath, packageName, schemaDir));
								continue;
							}
							/*if (IsBadName(schemaDirName))
							{
								Console.WriteLine($"		{schemaDirName}");
								pages.Add(schemaDir);
								continue;
							}*/
						}
						continue;
					}
				}
			}

			foreach (var page in pages)
			{
				page.NameNew = page.EntitySchemaName.StartsWith(SchemaPrefix) ? $"{page.EntitySchemaName}Page" : $"{SchemaPrefix}{page.EntitySchemaName}Page";
				if (page.Detail != null)
				{
					page.Detail.NameNew = page.EntitySchemaName.StartsWith(SchemaPrefix) ? $"{page.EntitySchemaName}Detail" : $"{SchemaPrefix}{page.EntitySchemaName}Detail";
				}
			}

			Console.WriteLine("----------------------------- Rename");

			var listForRename = pages.Select(it => it.Detail).Where(it => it != null);
			listForRename = listForRename.Concat(pages.Select(it => it as RenameSchema));
			int count = 0;
			foreach (var page in pages)
			{
				page.Rename(listForRename);
				count++;
				//if (count > 0) break; //for test
			}

			Console.WriteLine("----------------------------- Not Valid");
			Console.WriteLine("----------------------------- MoreThenTwoDetail");
			foreach (var page in pages)
			{
				if (page.MoreThenTwoDetail && !page.Valid)
				{
					Console.WriteLine($"  - new name {page.NameNew} from {page}");
				}
			}
			Console.WriteLine("----------------------------- Only page");
			foreach (var page in pages)
			{
				if (page.OnlyPage && !page.Valid)
				{
					Console.WriteLine($"  - new name {page.NameNew} from {page}");
				}
			}

			Console.WriteLine("----------------------------- Doubles");
			foreach (var schema in listForRename)
			{
				if (schema.DoubleExists)
				{
					Console.WriteLine($"  - new name {schema.NameNew} from {schema}");
				}
			}

			Console.WriteLine($"----------------------------- Renamed total {listForRename.Count(it => it.Renamed)} schemas");
			foreach (var schema in listForRename)
			{
				if (schema.Renamed)
				{
					Console.WriteLine($"  - {schema.NameNew} from {schema}");
				}
			}

			Console.WriteLine($"----------------------------- Renamed {pages.Count(it => it.Renamed)} from {count} pages");
			foreach (var page in pages)
			{
				if (page.Renamed)
				{
					Console.WriteLine($" - {page.NameNew} from {page}");
					if (page.Detail != null)
					{
						if (page.Detail.NeedRename)
						{
							Console.WriteLine($"   - Detail {page.Detail.NameNew} from {page.Detail}");
						}
					}
				}
			}
		}

		private bool IsBadName(string name)
		{
			int digitCount = 0;

			for (int i = 0; i < name.Length; i++)
			{
				if (Char.IsDigit(name[i]))
				{
					digitCount++;
				}
			}
			return digitCount > 2;
		}


	}

}
