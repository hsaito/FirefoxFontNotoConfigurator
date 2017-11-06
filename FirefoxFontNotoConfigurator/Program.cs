using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FirefoxFontNotoConfigurator
{
    internal static class Program
    {
        private static async Task Main()
        {
            try
            {
                var tableList = new List<FontTable>();
                using (var file = File.OpenRead("fonttable.csv"))
                {
                    var reader = new StreamReader(file);
                    await reader.ReadLineAsync();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        var splitLine = line.Split(",");
                        var entry = new FontTable
                        {
                            Region = splitLine[0],
                            Serif = splitLine[1],
                            Sans = splitLine[2],
                            Mono = splitLine[3]
                        };
                        tableList.Add(entry);
                    }
                }

                string config;
                using (var file = File.OpenRead("prefs.js"))
                {
                    var reader = new StreamReader(file);
                    config = await reader.ReadToEndAsync();
                }

                using (var file = File.Create("prefs.js"))
                {
                    config = Regex.Replace(config, "user_pref\\(\"font\\.name\\..*?\\);", "");
                    config = Regex.Replace(config, @"^\s*$\n|\r", "", RegexOptions.Multiline).TrimEnd();

                    foreach (var entry in tableList)
                    {
                        var item = $"user_pref(\"font.name.sans-serif.{entry.Region}\", \"{entry.Sans}\");\n";
                        item += $"user_pref(\"font.name.serif.{entry.Region}\", \"{entry.Serif}\");\n";
                        item += $"user_pref(\"font.name.monospace.{entry.Region}\", \"{entry.Mono}\");\n";
                        config += item;
                    }

                    var writer = new StreamWriter(file) {AutoFlush = true};
                    await writer.WriteAsync("\n");
                    await writer.WriteAsync(config);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private class FontTable
        {
            public string Mono;
            public string Region;
            public string Sans;
            public string Serif;
        }
    }
}