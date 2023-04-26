using CommandLine;
using System;

namespace CreatioConsoleHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<TestCommand, RenameSchemasCommand, ShowRedisConnectionStrings>(args)
                .WithParsed<ICommand>(t => t.Execute());
            if (result.Tag == ParserResultType.NotParsed)
            {
                Console.WriteLine("Help text requested, or parsing failed. Exit");
            }
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
