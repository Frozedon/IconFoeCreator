using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace IconFoeCreator
{
    public class StatisticBuilder
    {
        public List<Statistics> Jobs;
        public List<Statistics> Factions;

        private static readonly string DATA_FOLDER_PATH = "data/";

        public StatisticBuilder()
        {
            Jobs = new List<Statistics>();
            Factions = new List<Statistics>();
        }

        public void BuildStatistics()
        {
            Jobs.Clear();
            Factions.Clear();

            Factions.Add(new Statistics() { Name = "..." });

            List<Statistics> allStats = new List<Statistics>();

            // Collect stats from files
            if (Directory.Exists(DATA_FOLDER_PATH))
            {
                ReadJsonFilesInDirectory(DATA_FOLDER_PATH, allStats);
            }

            // Handle inheritance
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 0; i < allStats.Count(); ++i)
                {
                    Statistics stat = allStats[i];
                    if (stat.Inherits != null && stat.Inherits != String.Empty && !stat.Inherited)
                    {
                        Statistics match = allStats.First(otherStat => otherStat.Name == stat.Inherits);
                        if (match != null && (match.Inherits == null || match.Inherits == String.Empty || match.Inherited))
                        {
                            Statistics newStat = match.InheritFrom(stat);
                            allStats[i] = newStat;
                            changed = true;
                        }
                    }
                }
            }

            // Sort by name
            allStats.Sort(delegate(Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            // Organize into Lists
            foreach (Statistics stat in allStats)
            {
                if (stat.Type == "Job")
                {
                    Jobs.Add(stat);
                }
                else if (stat.Type == "Faction")
                {
                    Factions.Add(stat);
                }
            }
        }

        private void ReadJsonFilesInDirectory(string dirPath, List<Statistics> statsOutput)
        {
            // Iterate through folder looking for json files
            foreach (string path in Directory.GetFiles(dirPath))
            {
                if (path.EndsWith(".json") && !path.EndsWith("/example.json"))
                {
                    using (StreamReader r = new StreamReader(path))
                    {
                        string json = r.ReadToEnd();
                        if (json != null && json.Length > 0)
                        {
                            //Statistics readStat = null;
                            //try
                            //{
                            Statistics readStat = JsonConvert.DeserializeObject<Statistics>(json);
                            //}
                            //catch { }

                            if (readStat != null)
                            {
                                statsOutput.Add(readStat);
                            }
                        }
                    }
                }
            }

            foreach (string path in Directory.GetDirectories(dirPath))
            {
                ReadJsonFilesInDirectory(path, statsOutput);
            }
        }
    }
}
