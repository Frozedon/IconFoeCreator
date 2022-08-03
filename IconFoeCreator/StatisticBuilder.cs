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
        public List<Statistics> SpecialTemplates;
        public List<string> Factions;
        public List<string> Classes;
        public List<string> SpecialClasses;
        public List<TraitData> Traits;

        public static readonly string FACTION_BASIC = "basic";
        public static readonly string FACTION_BASIC_READABLE = "Basic Foes";

        public static readonly string CLASS_HEAVY = "heavy";
        public static readonly string CLASS_SKIRMISHER = "skirmisher";
        public static readonly string CLASS_LEADER = "leader";
        public static readonly string CLASS_ARTILLERY = "artillery";
        public static readonly string CLASS_UNIQUE = "unique";
        public static readonly string[] CLASSES_CORE = { CLASS_HEAVY, CLASS_SKIRMISHER, CLASS_LEADER, CLASS_ARTILLERY };
        public static readonly string[] CLASSES_CORE_READABLE = { "Heavy", "Skirmisher", "Leader", "Artillery" };

        public static readonly string SPECIAL_CLASS_NORMAL = "normal";
        public static readonly string SPECIAL_CLASS_MOB = "mob";
        public static readonly string SPECIAL_CLASS_ELITE = "elite";
        public static readonly string SPECIAL_CLASS_LEGEND = "legend";
        public static readonly string[] SPECIAL_CLASSES_CORE = { SPECIAL_CLASS_NORMAL, SPECIAL_CLASS_MOB, SPECIAL_CLASS_ELITE, SPECIAL_CLASS_LEGEND };
        public static readonly string[] SPECIAL_CLASSES_CORE_READABLE = { "Normal", "Mob", "Elite", "Legend" };

        private static readonly string DATA_FOLDER_PATH = "data/";
        private static readonly string BASE_FOLDER_PATH = DATA_FOLDER_PATH + "base/";
        private static readonly string HOMEBREW_FOLDER_PATH = DATA_FOLDER_PATH + "homebrew/";
        private static readonly string TRAITS_FILE_NAME = "traits.json";

        private static readonly string TYPE_FOE = "foe";
        private static readonly string TYPE_TEMPLATE = "template";
        private static readonly string TYPE_SPECIAL_TEMPLATE = "specialtemplate";

        public StatisticBuilder()
        {
            Foes = new List<Statistics>();
            Templates = new List<Statistics>();
            SpecialTemplates = new List<Statistics>();
            Factions = new List<string>();
            Classes = new List<string>();
            SpecialClasses = new List<string>();
            Traits = new List<TraitData>();
        }

        public void BuildStatistics(bool useHomebrew)
        {
            Foes.Clear();
            Templates.Clear();
            Factions.Clear();
            Classes.Clear();
            SpecialClasses.Clear();
            Traits.Clear();

            // Collect stats from files
            List<Statistics> allStats = new List<Statistics>();

            if (Directory.Exists(BASE_FOLDER_PATH))
            {
                ReadJsonFilesInDirectory(BASE_FOLDER_PATH, allStats);
            }
            if (useHomebrew && Directory.Exists(HOMEBREW_FOLDER_PATH))
            {
                ReadJsonFilesInDirectory(HOMEBREW_FOLDER_PATH, allStats);
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
                if (!String.IsNullOrEmpty(stat.SpecialClass) && !SpecialClasses.Contains(stat.SpecialClass))
                {
                    SpecialClasses.Add(stat.SpecialClass);
                }

                string typeLower = stat.Type.ToLower();
                if (typeLower == TYPE_FOE)
                {
                    Foes.Add(stat);
                }
                else if (typeLower == TYPE_TEMPLATE)
                {
                    Templates.Add(stat);
                }
                else if (typeLower == TYPE_SPECIAL_TEMPLATE)
                {
                    SpecialTemplates.Add(stat);
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

            SpecialTemplates.Sort(delegate (Statistics x, Statistics y)
            {
                return x.Name.CompareTo(y.Name);
            });

            Factions.RemoveAll(x => x.ToLower() == FACTION_BASIC);
            Factions.Sort(delegate (string x, string y)
            {
                return x.CompareTo(y);
            });
            Factions.Insert(0, FACTION_BASIC_READABLE);

            Classes.RemoveAll(x => CLASSES_CORE.Contains(x.ToLower()));
            Classes.Sort(delegate (string x, string y)
            {
                return x.CompareTo(y);
            });
            Classes.InsertRange(0, CLASSES_CORE_READABLE);

            SpecialClasses.RemoveAll(x => SPECIAL_CLASSES_CORE.Contains(x.ToLower()));
            SpecialClasses.Sort(delegate (string x, string y)
            {
                return x.CompareTo(y);
            });
            SpecialClasses.InsertRange(0, SPECIAL_CLASSES_CORE_READABLE);
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
                    if (stat.Inherits.Count > 0)
                    {
                        string inherits = stat.Inherits.Last();
                        if (!String.IsNullOrEmpty(inherits))
                        {
                            Statistics match = stats.FirstOrDefault(otherStat => otherStat.Name == inherits);
                            if (match != null && match.Inherits.Count() == 0)
                            {
                                stats[i] = stat = stat.InheritFrom(match, stat.Inherits.Count > 1);
                                stat.Inherits.Remove(inherits);
                                changed = true;
                            }
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
                if (path.EndsWith("/" + TRAITS_FILE_NAME) || path.EndsWith("\\" + TRAITS_FILE_NAME))
                {
                    ReadTraitsJsonFile(path, Traits);
                }
                else if (path.EndsWith(".json") && !path.EndsWith("/example.json") && !path.EndsWith("\\example.json"))
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

        private void ReadTraitsJsonFile(string path, List<TraitData> traitsOutput)
        {
            if (File.Exists(path) && path.EndsWith(".json"))
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    if (json != null && json.Length > 0)
                    {
                        List<TraitData> readTraits = null;
                        try
                        {
                            readTraits = JsonConvert.DeserializeObject<List<TraitData>>(json);
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

        public List<string> Debug_CheckForTraitsExisting()
        {
            List<string> missingTraits = new List<string>();

            List<Statistics> allStats = new List<Statistics>();
            allStats.AddRange(Foes);
            allStats.AddRange(Templates);
            allStats.AddRange(SpecialTemplates);

            foreach (Statistics stat in allStats)
            {
                List<string> traitNames = new List<string>();
                traitNames.AddRange(stat.Traits);
                foreach (AbilitySetData abilityData in stat.ExtraAbilitySets) { traitNames.AddRange(abilityData.Traits); }
                foreach (ConditionalAbilityData abilityData in stat.ConditionalAbilities) { traitNames.AddRange(abilityData.Traits); }
                foreach (PhaseData phaseData in stat.Phases) { traitNames.AddRange(phaseData.Traits); }

                foreach (string traitName in traitNames)
                {
                    bool found = false;
                    foreach (TraitData trait in Traits)
                    {
                        if (trait.Matches(traitName))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        missingTraits.Add(traitName);
                    }
                }
            }

            return missingTraits.Distinct().ToList();
        }

        public List<string> Debug_CheckForDoubleTraits()
        {
            List<string> doubleTraits = new List<string>();

            foreach (TraitData trait in Traits)
            {
                var traitsFound = Traits.FindAll(x => x.Name.ToLower() == trait.Name.ToLower());

                if (traitsFound.Count > 1)
                {
                    doubleTraits.Add(trait.Name);
                }
            }

            return doubleTraits.Distinct().ToList();
        }
    }
}
