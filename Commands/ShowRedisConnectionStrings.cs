using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CreatioConsoleHelper
{
    [Verb("ShowRedisConnectionStrings", HelpText = "Show redis connection strings")]
    public class ShowRedisConnectionStrings : ICommand
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
                    foreach (var connectionString in connectionStrings.Items)
                    {
                        if (connectionString.Name == "redis")
                        {
                            connectionString.Parse();
                            connectionString.SiteName = Path.GetFileNameWithoutExtension(dir);
                            redisConnectionStrings.Add(connectionString);
                        }
                    }
                }
            }

            foreach (var connectionString in redisConnectionStrings.OrderBy(it => it.RedisDb))
            {
                Console.WriteLine($"- {connectionString.SiteName}:");
                Console.WriteLine($"  - {connectionString.Name}: {connectionString.ConnectionString}");
            }
        }
    }



    [Serializable()]
    [XmlRoot("connectionStrings")]
    public class ConnectionStrings
    {

        [XmlElement("add")]
        public ConnectionStringItem[] Items { get; set; }
    }

    [Serializable()]
    public class ConnectionStringItem
    {

        [XmlAttributeAttribute("name")]
        public string Name { get; set; }

        [XmlAttributeAttribute("connectionString")]
        public string ConnectionString { get; set; }

        [NonSerialized]
        public int RedisDb = -1;
        [NonSerialized]
        public string SiteName = String.Empty;

        public void Parse()
        {
            foreach (var item in ConnectionString.Split(";").Select(it => it.Split("=")))
            {
                if (item[0].Trim() == "db" && int.TryParse(item[1], out var parsed))
                {
                    RedisDb = parsed;
                }
            }
        }
    }
}
