using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IconFoeCreator
{
    public class StatisticBuilder
    {
        public List<Statistics> Stats;
        public List<string> Factions;
        public List<string> Classes;
        public List<Trait> Traits;

        public static readonly string MOB = "mob";
        public static readonly string ELITE = "elite";
        public static readonly string LEGEND = "legend";
        public static readonly string HEAVY_CLASS = "heavy";
        public static readonly string SKIRMISHER_CLASS = "skirmisher";
        public static readonly string LEADER_CLASS = "leader";
        public static readonly string ARTILLERY_CLASS = "artillery";
        public static readonly string[] CORE_CLASSES = { HEAVY_CLASS, SKIRMISHER_CLASS, LEADER_CLASS, ARTILLERY_CLASS };

        private static readonly string DATA_FOLDER_PATH = "data/";
        private static readonly string BASE_FOLDER_PATH = DATA_FOLDER_PATH + "base/";
        private static readonly string HOMEBREW_FOLDER_PATH = DATA_FOLDER_PATH + "homebrew/";
        private static readonly string TRAITS_FILE_PATH = DATA_FOLDER_PATH + "traits.json";

        public StatisticBuilder()
        {
            Stats = new List<Statistics>();
            Factions = new List<string>();
            Classes = new List<string>();
            Traits = new List<Trait>();
        }

        public void BuildStatistics(bool useHomebrew)
        {
            Stats.Clear();
            Factions.Clear();
            Classes.Clear();
            Traits.Clear();

            // Collect stats from files
            if (Directory.Exists(BASE_FOLDER_PATH))
            {
                ReadJsonFilesInDirectory(BASE_FOLDER_PATH, Stats);
            }
            if (useHomebrew && Directory.Exists(HOMEBREW_FOLDER_PATH))
            {
                ReadJsonFilesInDirectory(HOMEBREW_FOLDER_PATH, Stats);
            }

            // Handle inheritance
            HandleInheritance(Stats);

            // Organize into Lists
            foreach (Statistics stat in Stats)
            {
                if (!String.IsNullOrEmpty(stat.Faction))
                {
                    Factions.Add(stat.Faction);
                }
                if (!String.IsNullOrEmpty(stat.Class))
                {
                    Classes.Add(stat.Class);
                }
            }

            // Read traits
            ReadTraitsJsonFile(TRAITS_FILE_PATH, Traits);

            // Sort lists
            Stats.Sort(delegate (Statistics x, Statistics y)
            {
                if (x.IsBasicTemplate && !y.IsBasicTemplate)
                    return -1;
                if (!x.IsBasicTemplate && y.IsBasicTemplate)
                    return 1;

                return x.Name.CompareTo(y.Name);
            });

            Factions.Sort(delegate (string x, string y)
            {
                return x.CompareTo(y);
            });

            Classes.Sort(delegate (string x, string y)
            {
                return x.CompareTo(y);
            });
        }

        private void HandleInheritance(List<Statistics> stats)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 0; i < stats.Count(); ++i)
                {
                    Statistics stat = stats[i];
                    for (int j = 0; j < stat.Inherits.Count();)
                    {
                        string inherits = stat.Inherits[j];
                        Statistics match = stats.FirstOrDefault(otherStat => otherStat.Name == inherits);
                        if (match != null)
                        {
                            if (match.Inherits.Count() == 0)
                            {
                                stats[i] = stat = stat.InheritFrom(match);
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
                            Statistics readStat = null;
                            try
                            {
                                readStat = JsonConvert.DeserializeObject<Statistics>(json);
                            }
                            catch { }

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

        private void ReadTraitsJsonFile(string path, List<Trait> traitsOutput)
        {
            if (File.Exists(path) && path.EndsWith(".json"))
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    if (json != null && json.Length > 0)
                    {
                        List<Trait> readTraits = null;
                        try
                        {
                            readTraits = JsonConvert.DeserializeObject<List<Trait>>(json);
                        }
                        catch { }

                        if (readTraits != null)
                        {
                            traitsOutput.AddRange(readTraits);
                        }
                    }
                }
            }
        }
    }
}
