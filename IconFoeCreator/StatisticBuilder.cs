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
        public List<string> JobGroups;
        public List<string> FactionGroups;

        private static readonly string DATA_FOLDER_PATH = "data/";
        public static readonly string EMPTY_GROUP = "...";
        public static readonly string ANY_GROUP = "Any";

        public StatisticBuilder()
        {
            Jobs = new List<Statistics>();
            Factions = new List<Statistics>();
            JobGroups = new List<string>();
            FactionGroups = new List<string>();
        }

        public void BuildStatistics()
        {
            Jobs.Clear();
            Factions.Clear();
            JobGroups.Clear();
            FactionGroups.Clear();

            // Add defaults
            Factions.Add(new Statistics() { Name = EMPTY_GROUP });
            JobGroups.Add(ANY_GROUP);
            FactionGroups.Add(ANY_GROUP);

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
                        Statistics match = allStats.FirstOrDefault(otherStat => otherStat.Name == stat.Inherits);
                        if (match != null && (match.Inherits == null || match.Inherits == String.Empty || match.Inherited))
                        {
                            Statistics newStat = stat.InheritFrom(match);
                            allStats[i] = newStat;
                            changed = true;
                        }
                    }
                }
            }

            // Organize into Lists
            foreach (Statistics stat in allStats)
            {
                if (stat.Type == "Job")
                {
                    Jobs.Add(stat);

                    if (stat.Group != null && stat.Group != String.Empty && !JobGroups.Contains(stat.Group))
                    {
                        JobGroups.Add(stat.Group);
                    }
                }
                else if (stat.Type == "Faction")
                {
                    Factions.Add(stat);

                    if (stat.Group != null && stat.Group != String.Empty && !FactionGroups.Contains(stat.Group))
                    {
                        FactionGroups.Add(stat.Group);
                    }
                }
            }

            // Sort lists
            Jobs.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            JobGroups.Sort(delegate (string x, string y)
            {
                if (x == ANY_GROUP) { return -1; }
                if (y == ANY_GROUP) { return 1; }
                return x.CompareTo(y);
            });

            Factions.Sort(delegate (Statistics x, Statistics y)
            {
                if (x.Name == EMPTY_GROUP) { return -1; }
                if (y.Name == EMPTY_GROUP) { return 1; }
                return x.Name.CompareTo(y.Name);
            });

            FactionGroups.Sort(delegate (string x, string y)
            {
                if (x == ANY_GROUP) { return -1; }
                if (y == ANY_GROUP) { return 1; }
                return x.CompareTo(y);
            });
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
