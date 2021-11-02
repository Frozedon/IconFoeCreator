using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IconFoeCreator
{
    public class StatisticGroup
    {
        public string Name;
        public bool HasBase = false;
    }

    public class StatisticBuilder
    {
        public List<StatisticGroup> Factions;
        public List<Statistics> Templates;
        public List<StatisticGroup> Classes;
        public List<Statistics> Jobs;

        private static readonly string DATA_FOLDER_PATH = "data/";
        private static readonly string BASE_FOLDER_PATH = "base/";
        private static readonly string HOMEBREW_FOLDER_PATH = "homebrew/";
        public static readonly string EMPTY_GROUP = "...";
        public static readonly string ANY_GROUP = "Any";

        public StatisticBuilder()
        {
            Factions = new List<StatisticGroup>();
            Templates = new List<Statistics>();
            Classes = new List<StatisticGroup>();
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

                    if (stat.Group != null && stat.Group != String.Empty)
                    {
                        var group = Classes.FirstOrDefault(otherGroup => otherGroup.Name == stat.Group);
                        if (group == null)
                        {
                            Classes.Add(new StatisticGroup() { Name = stat.Group, HasBase = !stat.IsHomebrew });
                        }
                        else if (!stat.IsHomebrew)
                        {
                            group.HasBase = true;
                        }
                    }
                }
                else if (stat.Type == "Faction")
                {
                    Templates.Add(stat);

                    if (stat.Group != null && stat.Group != String.Empty)
                    {
                        var group = Factions.FirstOrDefault(otherGroup => otherGroup.Name == stat.Group);
                        if (group == null)
                        {
                            Factions.Add(new StatisticGroup() { Name = stat.Group, HasBase = !stat.IsHomebrew });
                        }
                        else if (!stat.IsHomebrew)
                        {
                            group.HasBase = true;
                        }
                    }
                }
            }

            // Add defaults
            Templates.Add(new Statistics() { Name = EMPTY_GROUP });
            Classes.Add(new StatisticGroup() { Name = ANY_GROUP, HasBase = true });
            Factions.Add(new StatisticGroup() { Name = ANY_GROUP, HasBase = true });

            // Sort lists
            Jobs.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            Templates.Sort(delegate (Statistics x, Statistics y)
            {
                if (x.Name == EMPTY_GROUP) { return -1; }
                if (y.Name == EMPTY_GROUP) { return 1; }
                return x.Name.CompareTo(y.Name);
            });

            Classes.Sort(delegate (StatisticGroup x, StatisticGroup y)
            {
                if (x.Name == ANY_GROUP) { return -1; }
                if (y.Name == ANY_GROUP) { return 1; }
                return x.Name.CompareTo(y.Name);
            });

            Factions.Sort(delegate (StatisticGroup x, StatisticGroup y)
            {
                if (x.Name == ANY_GROUP) { return -1; }
                if (y.Name == ANY_GROUP) { return 1; }
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
