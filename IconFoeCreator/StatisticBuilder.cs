using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IconFoeCreator
{
    public class Group
    {
        public string Name;
        public bool OnlyHomebrew;

        public override string ToString()
        {
            return Name;
        }
    }

    public class StatisticBuilder
    {
        public List<Group> Factions;
        public List<Statistics> Templates;
        public List<Group> Classes;
        public List<Statistics> Jobs;
        public List<Statistics> UniqueFoes;
        public List<Statistics> Specials;

        public static readonly string HEAVY_CLASS = "heavy";
        public static readonly string SKIRMISHER_CLASS = "skirmisher";
        public static readonly string LEADER_CLASS = "leader";
        public static readonly string ARTILLERY_CLASS = "artillery";
        public static readonly string[] CORE_CLASSES = { HEAVY_CLASS, SKIRMISHER_CLASS, LEADER_CLASS, ARTILLERY_CLASS };

        private static readonly string DATA_FOLDER_PATH = "data/";
        private static readonly string BASE_FOLDER_PATH = "base/";
        private static readonly string HOMEBREW_FOLDER_PATH = "homebrew/";

        private static readonly string TYPE_TEMPLATE = "template";
        private static readonly string TYPE_JOB = "job";
        private static readonly string TYPE_UNIQUEFOE = "uniquefoe";
        private static readonly string TYPE_SPECIAL = "special";

        public StatisticBuilder()
        {
            Factions = new List<Group>();
            Templates = new List<Statistics>();
            Classes = new List<Group>();
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
                        var group = Classes.FirstOrDefault(otherGroup => otherGroup.Name == stat.Group);
                        if (group == null)
                        {
                            Classes.Add(new Group { Name = stat.Group, OnlyHomebrew = stat.IsHomebrew });
                        }
                        else if (!stat.IsHomebrew)
                        {
                            group.OnlyHomebrew = false;
                        }
                    }
                }
                else if (typeLowercase == TYPE_TEMPLATE)
                {
                    Templates.Add(stat);

                    if (!String.IsNullOrEmpty(stat.Group))
                    {
                        var group = Factions.FirstOrDefault(otherGroup => otherGroup.Name == stat.Group);
                        if (group == null)
                        {
                            Factions.Add(new Group { Name = stat.Group, OnlyHomebrew = stat.IsHomebrew });
                        }
                        else if (!stat.IsHomebrew)
                        {
                            group.OnlyHomebrew = false;
                        }
                    }
                }
                else if (typeLowercase == TYPE_UNIQUEFOE)
                {
                    UniqueFoes.Add(stat);

                    if (!String.IsNullOrEmpty(stat.Group))
                    {
                        var group = Factions.FirstOrDefault(otherGroup => otherGroup.Name == stat.Group);
                        if (group == null)
                        {
                            Factions.Add(new Group { Name = stat.Group, OnlyHomebrew = stat.IsHomebrew });
                        }
                        else if (!stat.IsHomebrew)
                        {
                            group.OnlyHomebrew = false;
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

            Classes.Sort(delegate (Group x, Group y)
            {
                string xResult = Array.Find(CORE_CLASSES, element => element == x.Name.ToLower());
                string yResult = Array.Find(CORE_CLASSES, element => element == y.Name.ToLower());

                if (xResult == null && yResult != null)
                    return 1;
                if (xResult != null && yResult == null)
                    return -1;

                return x.Name.CompareTo(y.Name);
            });

            Factions.Sort(delegate (Group x, Group y)
            {
                return x.Name.CompareTo(y.Name);
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
