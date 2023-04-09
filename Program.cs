using CommandLine;
using System;
using System.IO;

namespace CreatioConsoleHelper
{
	class Program
	{
		static void Main(string[] args)
		{
			FileStream ostrm;
			StreamWriter writer;
			TextWriter oldOut = Console.Out;
			string logFileName = Path.Combine(Directory.GetCurrentDirectory(), $"RenameSchemas{DateTime.Now.ToString("yyMMdd_HHmmss")}.log");
			Console.WriteLine($"Log path {logFileName}");
			try
			{
				ostrm = new FileStream(logFileName, FileMode.OpenOrCreate, FileAccess.Write);
				writer = new StreamWriter(ostrm);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Cannot open {logFileName} for writing");
				Console.WriteLine(e.Message);
				return;
			}
			Console.SetOut(writer);

			var result = Parser.Default.ParseArguments<TestCommand, RenameSchemasCommand>(args)
				.WithParsed<ICommand>(t => t.Execute());

			if (result.Tag == ParserResultType.NotParsed)
			{
				Console.WriteLine("Help text requested, or parsing failed. Exit");
			}
			Console.SetOut(oldOut);
			writer.Close();
			ostrm.Close();
			Console.WriteLine("Done");
		}
	}

	[Verb("test", HelpText = "Test command help text")]
	public class TestCommand : ICommand
	{
		public void Execute()
		{
			Console.WriteLine("Test Command");
		}
	}
}
