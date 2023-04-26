using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CreatioConsoleHelper.Commands
{
    [Verb("ShowConnectionStrings", HelpText = "Show redis connection strings")]
    public class ShowConnectionStrings : ICommand
    {

        [Option("sitesFolder", Required = true, HelpText = "Sites folders path")]
        public string SitesFolder { get; set; }

        public void Execute()
        {
            Console.WriteLine($"Site folder: {SitesFolder}");
            XmlSerializer serializer = new XmlSerializer(typeof(ConnectionStrings));
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(SitesFolder));
            var redisConnectionStrings = new List<ConnectionStringItem>();
            foreach (var dir in dirs)
            {

                var connectionStringPath = Path.Combine(dir, "ConnectionStrings.config");
                if (!File.Exists(connectionStringPath))
                {
                    continue;
                }
                using (var reader = new StreamReader(connectionStringPath))
                {
                    var connectionStrings = (ConnectionStrings)serializer.Deserialize(reader);
                    Console.WriteLine($"- {Path.GetFileNameWithoutExtension(dir)}:");
                    foreach (var connectionString in connectionStrings.Items.OrderBy(it => it.Name))
                    {
                        if (connectionString.Name == "redis" || connectionString.Name == "db")
                        {
                            Console.WriteLine($"  - {connectionString.Name}: {connectionString.ConnectionString}");
                        }
                    }
                }
            }
        }
    }
}
