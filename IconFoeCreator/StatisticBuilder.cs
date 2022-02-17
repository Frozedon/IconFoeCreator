using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IconFoeCreator
{
    public class StatisticBuilder
    {
        public List<string> Factions;
        public List<Statistics> Templates;
        public List<string> Classes;
        public List<Statistics> Jobs;

        private static readonly string DATA_FOLDER_PATH = "data/";
        private static readonly string BASE_FOLDER_PATH = "base/";
        private static readonly string HOMEBREW_FOLDER_PATH = "homebrew/";

        public StatisticBuilder()
        {
            Factions = new List<string>();
            Templates = new List<Statistics>();
            Classes = new List<string>();
            Jobs = new List<Statistics>();
        }

        public void BuildStatistics()
        {
            Factions.Clear();
            Templates.Clear();
            Classes.Clear();
            Jobs.Clear();

            List<Statistics> allStats = new List<Statistics>();

            // Collect stats from files
            if (Directory.Exists(DATA_FOLDER_PATH + BASE_FOLDER_PATH))
            {
                ReadJsonFilesInDirectory(DATA_FOLDER_PATH + BASE_FOLDER_PATH, allStats);
            }
            if (Directory.Exists(DATA_FOLDER_PATH + HOMEBREW_FOLDER_PATH))
            {
                ReadJsonFilesInDirectory(DATA_FOLDER_PATH + HOMEBREW_FOLDER_PATH, allStats, true);
            }

            // Handle inheritance
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 0; i < allStats.Count(); ++i)
                {
                    Statistics stat = allStats[i];
                    for (int j = 0; j < stat.Inherits.Count();)
                    {
                        string inherits = stat.Inherits[j];
                        Statistics match = allStats.FirstOrDefault(otherStat => otherStat.Name == inherits);
                        if (match != null)
                        {
                            if (match.Inherits.Count() == 0)
                            {
                                allStats[i] = stat = stat.InheritFrom(match);
                                stat.Inherits.Remove(inherits);
                                changed = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            ++j;
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

                    if (stat.Group != null && stat.Group != String.Empty)
                    {
                        var group = Classes.FirstOrDefault(otherGroup => otherGroup == stat.Group);
                        if (group == null)
                        {
                            Classes.Add(stat.Group);
                        }
                    }
                }
                else if (stat.Type == "Template")
                {
                    Templates.Add(stat);

                    if (stat.Group != null && stat.Group != String.Empty)
                    {
                        var group = Factions.FirstOrDefault(otherGroup => otherGroup == stat.Group);
                        if (group == null)
                        {
                            Factions.Add(stat.Group);
                        }
                    }
                }
            }

            // Sort lists
            Jobs.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            Templates.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            Classes.Sort(delegate (string x, string y)
            {
                return x.CompareTo(y);
            });

            Factions.Sort(delegate (string x, string y)
            {
                return x.CompareTo(y);
            });
        }

        private void ReadJsonFilesInDirectory(string dirPath, List<Statistics> statsOutput, bool isHomebrew = false)
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
                            Statistics readStat = null;
                            try
                            {
                                readStat = JsonConvert.DeserializeObject<Statistics>(json);
                            }
                            catch { }

                            if (readStat != null)
                            {
                                readStat.IsHomebrew = isHomebrew;
                                statsOutput.Add(readStat);
                            }
                        }
                    }
                }
            }

            foreach (string path in Directory.GetDirectories(dirPath))
            {
                ReadJsonFilesInDirectory(path, statsOutput, isHomebrew);
            }
        }
    }
}
