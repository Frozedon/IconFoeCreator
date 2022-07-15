using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IconFoeCreator
{
    public class StatisticBuilder
    {
        public List<Statistics> Foes;
        public List<Statistics> Templates;
        public List<Statistics> Jobs;
        public List<string> Factions;
        public List<string> Classes;
        public List<Trait> Traits;

        public static readonly string MOB = "mob";
        public static readonly string ELITE = "elite";
        public static readonly string LEGEND = "legend";
        public static readonly string CLASS_HEAVY = "heavy";
        public static readonly string CLASS_SKIRMISHER = "skirmisher";
        public static readonly string CLASS_LEADER = "leader";
        public static readonly string CLASS_ARTILLERY = "artillery";
        public static readonly string[] CORE_CLASSES = { CLASS_HEAVY, CLASS_SKIRMISHER, CLASS_LEADER, CLASS_ARTILLERY };

        private static readonly string DATA_FOLDER_PATH = "data/";
        private static readonly string BASE_FOLDER_PATH = DATA_FOLDER_PATH + "base/";
        private static readonly string HOMEBREW_FOLDER_PATH = DATA_FOLDER_PATH + "homebrew/";
        private static readonly string TRAITS_FILE_NAME = "traits.json";

        private static readonly string TYPE_FOE = "foe";
        private static readonly string TYPE_TEMPLATE = "template";
        private static readonly string TYPE_JOB = "job";

        public StatisticBuilder()
        {
            Foes = new List<Statistics>();
            Templates = new List<Statistics>();
            Jobs = new List<Statistics>();
            Factions = new List<string>();
            Classes = new List<string>();
            Traits = new List<Trait>();
        }

        public void BuildStatistics(bool useHomebrew)
        {
            Foes.Clear();
            Templates.Clear();
            Jobs.Clear();
            Factions.Clear();
            Classes.Clear();
            Traits.Clear();

            // Collect stats from files
            List<Statistics> allStats = new List<Statistics>();

            if (Directory.Exists(BASE_FOLDER_PATH))
            {
                ReadJsonFilesInDirectory(BASE_FOLDER_PATH, allStats);
                ReadTraitsJsonFile(BASE_FOLDER_PATH + TRAITS_FILE_NAME, Traits);
            }
            if (useHomebrew && Directory.Exists(HOMEBREW_FOLDER_PATH))
            {
                ReadJsonFilesInDirectory(HOMEBREW_FOLDER_PATH, allStats);
                ReadTraitsJsonFile(HOMEBREW_FOLDER_PATH + TRAITS_FILE_NAME, Traits);
            }

            // Handle inheritance
            HandleInheritance(allStats);

            // Organize into Lists
            foreach (Statistics stat in allStats)
            {
                if (!String.IsNullOrEmpty(stat.Faction) && !Factions.Contains(stat.Faction))
                {
                    Factions.Add(stat.Faction);
                }
                if (!String.IsNullOrEmpty(stat.Class) && !Classes.Contains(stat.Class))
                {
                    Classes.Add(stat.Class);
                }

                if (stat.Type.ToLower() == TYPE_FOE)
                {
                    Foes.Add(stat);
                }
                else if (stat.Type.ToLower() == TYPE_TEMPLATE)
                {
                    Templates.Add(stat);
                }
                else if (stat.Type.ToLower() == TYPE_JOB)
                {
                    Jobs.Add(stat);
                }
            }

            // Sort lists
            Foes.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            Templates.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            Jobs.Sort(delegate (Statistics x, Statistics y)
            {
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
                if (path.EndsWith(".json") && !path.EndsWith("/example.json") && !path.EndsWith("/" + TRAITS_FILE_NAME))
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
