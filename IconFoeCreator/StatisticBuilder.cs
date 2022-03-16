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
        public List<Statistics> UniqueFoes;
        public List<Statistics> Specials;

        public static readonly string[] CoreClasses = { "Heavy", "Skirmisher", "Leader", "Artillery" };

        private static readonly string DATA_FOLDER_PATH = "data/";
        private static readonly string BASE_FOLDER_PATH = "base/";
        private static readonly string HOMEBREW_FOLDER_PATH = "homebrew/";

        private static readonly string TYPE_TEMPLATE = "template";
        private static readonly string TYPE_JOB = "job";
        private static readonly string TYPE_UNIQUEFOE = "uniquefoe";
        private static readonly string TYPE_SPECIAL = "special";

        public StatisticBuilder()
        {
            Factions = new List<string>();
            Templates = new List<Statistics>();
            Classes = new List<string>();
            Jobs = new List<Statistics>();
            UniqueFoes = new List<Statistics>();
            Specials = new List<Statistics>();
        }

        public void BuildStatistics()
        {
            Factions.Clear();
            Templates.Clear();
            Classes.Clear();
            Jobs.Clear();
            UniqueFoes.Clear();
            Specials.Clear();

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
                string typeLowercase = stat.Type.ToLower();
                if (typeLowercase == TYPE_JOB)
                {
                    Jobs.Add(stat);

                    if (!String.IsNullOrEmpty(stat.Group))
                    {
                        var group = Classes.FirstOrDefault(otherGroup => otherGroup == stat.Group);
                        if (group == null)
                        {
                            Classes.Add(stat.Group);
                        }
                    }
                }
                else if (typeLowercase == TYPE_TEMPLATE)
                {
                    Templates.Add(stat);

                    if (!String.IsNullOrEmpty(stat.Group))
                    {
                        var group = Factions.FirstOrDefault(otherGroup => otherGroup == stat.Group);
                        if (group == null)
                        {
                            Factions.Add(stat.Group);
                        }
                    }
                }
                else if (typeLowercase == TYPE_UNIQUEFOE)
                {
                    UniqueFoes.Add(stat);

                    if (!String.IsNullOrEmpty(stat.Group))
                    {
                        var group = Factions.FirstOrDefault(otherGroup => otherGroup == stat.Group);
                        if (group == null)
                        {
                            Factions.Add(stat.Group);
                        }
                    }
                }
                else if (typeLowercase == TYPE_SPECIAL)
                {
                    Specials.Add(stat);
                }
            }

            // Sort lists
            Jobs.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            Templates.Sort(delegate (Statistics x, Statistics y)
            {
                if (x.IsBasicTemplate && !y.IsBasicTemplate)
                    return -1;
                if (!x.IsBasicTemplate && y.IsBasicTemplate)
                    return 1;

                return x.Name.CompareTo(y.Name);
            });

            UniqueFoes.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            Specials.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            Classes.Sort(delegate (string x, string y)
            {
                string xResult = Array.Find(CoreClasses, element => element == x);
                string yResult = Array.Find(CoreClasses, element => element == y);

                if (xResult == null && yResult != null)
                    return 1;
                if (xResult != null && yResult == null)
                    return -1;

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
