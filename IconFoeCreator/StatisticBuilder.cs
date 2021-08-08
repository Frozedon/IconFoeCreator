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

        public void BuildTestStatistics()
        {
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

            /*
            Statistics heavyStats = new Statistics();
            heavyStats.Name = "Heavy";
            heavyStats.Health[0].Value = 8;
            heavyStats.Health[1].Value = 9;
            heavyStats.Health[2].Value = 10;
            heavyStats.Speed.Value = 3;
            heavyStats.Run.Value = 2;
            heavyStats.Dash.Value = 3;
            heavyStats.Defense.Value = 6;
            heavyStats.Armor[0].Value = 2;
            heavyStats.Armor[1].Value = 2;
            heavyStats.Armor[2].Value = 3;
            heavyStats.AttackBonus[0].Value = 0;
            heavyStats.AttackBonus[1].Value = 2;
            heavyStats.AttackBonus[2].Value = 4;
            heavyStats.FrayDamage[0].Value = 2;
            heavyStats.FrayDamage[1].Value = 2;
            heavyStats.FrayDamage[2].Value = 3;
            heavyStats.LightDamage.Value = 3;
            heavyStats.HeavyDamage.Value = 5;
            heavyStats.CriticalDamage.Value = 7;
            heavyStats.DamageSaveType.Value = "Physical";

            Statistics soldierStats = new Statistics();
            soldierStats.Name = "Soldier";
            soldierStats.Traits.Add(new Trait() { Name = "Guardian", Description = "Special interrupt. When an ally you can see in range 3 is targeted by an attack, you can dash up to 2 spaces towards that ally.Then, if you’re adjacent, you can change the target of the attack to you." });
            soldierStats.Traits.Add(new Trait() { Name = "Vigilance", Description = "Hostile characters can’t dash in your engagement and can’t move through your space for any reason." });
            soldierStats.Traits.Add(new Trait() { Name = "Rank and File", Description = "Gain true strike if adjacent to an ally." });
            soldierStats.Actions.Add(new Action() { Name = "Slash", ActionCost = 1, Tags = { "melee attack" }, Hit = "Deal light damage.", Miss = "Deal fray damage." });
            soldierStats.Actions.Add(new Action() { Name = "Bash", ActionCost = 1, Description = "An adjacent character must physical save or take away fray damage and become dazed." });
            soldierStats.Actions.Add(new Action() { Name = "Fortify", ActionCost = 1, Tags = { "stance" }, Description = "Count as in cover and grant cover to adjacent allied characters, but cannot dash or run. End if shoved, dazed staggered, or stunned." });

            Statistics impalerStats = new Statistics();
            impalerStats.Name = "Impaler";
            impalerStats.Traits.Add(new Trait() { Name = "Guardian", Description = "Special interrupt. When an ally you can see in range 3 is targeted by an attack, you can dash up to 2 spaces towards that ally.Then, if you’re adjacent, you can change the target of the attack to you." });
            impalerStats.Traits.Add(new Trait() { Name = "Vigilance", Description = "Hostile characters can’t dash in your engagement and can’t move through your space for any reason." });
            impalerStats.Actions.Add(new Action() { Name = "Pike", ActionCost = 2, Tags = { "melee attack" }, Hit = "Deal heavy damage and shove 1.", Miss = "Deal fray damage." });
            impalerStats.Actions.Add(new Action() { Name = "Impale", ActionCost = 2, Tags = { "melee attack", "true strike", "recahrge 5+" }, Hit = "Deal light damage and shove 3. The impaler chases after the target, following it.", Miss = "Deal light damage.", Collide = "Foe is also dazed and immobilized." });

            Statistics relictStats = new Statistics();
            relictStats.Name = "Relict";
            relictStats.Traits.Add(new Trait() { Name = "Defiant Spark", Description = "All Relict have Defiance." });
            relictStats.Traits.Add(new Trait() { Name = "Monsters", Description = "Relict do not flee or negotiate." });
            relictStats.Traits.Add(new Trait() { Name = "Faction Blight", Description = "Electrified." });

            Statistics wightStats = new Statistics();
            wightStats.Name = "Wight";
            wightStats.Traits.Add(new Trait() { Name = "Networked", Description = "+1 boon on all attacks if another Relict is adjacent to their target." });
            wightStats.Traits.Add(new Trait() { Name = "Shuffle", Description = "A wight can move 1 space forward at the start of their turn. When they do this, they can also move any wights that are contiguously connected to them via adjacency." });

            Statistics idolStats = new Statistics();
            idolStats.Name = "Idol";
            idolStats.PercentAddHitPoints.Value = 0.25f;
            idolStats.NoDash.Value = true;
            idolStats.NoRun.Value = true;
            idolStats.Traits.Add(new Trait() { Name = "Large", Description = "Increase size to 2." });
            idolStats.Traits.Add(new Trait() { Name = "Combat Subroutine", Description = "Has hatred of the closest character." });
            idolStats.Traits.Add(new Trait() { Name = "Imbued Strength", Description = "All melee damage is boosted and gains shove 1." });
            idolStats.Traits.Add(new Trait() { Name = "Stone March", Description = "Slow and cannot dash, run, or teleport." });
            idolStats.Traits.Add(new Trait() { Name = "Heavy", Description = "Immune to shove." });

            Statistics fusedStats = new Statistics();
            fusedStats.Name = "Fused";
            fusedStats.PercentAddHitPoints.Value = 0.25f;
            fusedStats.Traits.Add(new Trait() { Name = "Large", Description = "Increase size to 2." });
            fusedStats.Traits.Add(new Trait() { Name = "Deadsoul", Description = "Has evasion and dodge vs magic." });
            fusedStats.Traits.Add(new Trait() { Name = "Shambling", Description = "Permanently staggered and dazed." });
            fusedStats.Traits.Add(new Trait() { Name = "Release Passengers", Description = "When defeated, summon 4 Relict Chaft Mobs in free adjacent spaces." });
            

            // Add to lists
            Jobs.Clear();
            Jobs.Add(heavyStats.MakeCombined(soldierStats));
            Jobs.Add(heavyStats.MakeCombined(impalerStats));

            Factions.Clear();
            Factions.Add(relictStats.MakeCombined(wightStats.MakeCombined(idolStats)));
            Factions.Add(relictStats.MakeCombined(wightStats.MakeCombined(fusedStats)));
            */
        }

        private void ReadJsonFilesInDirectory(string dirPath, List<Statistics> statsOutput)
        {
            // Iterate through folder looking for json files
            foreach (string path in Directory.GetFiles(dirPath))
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

            foreach (string path in Directory.GetDirectories(dirPath))
            {
                ReadJsonFilesInDirectory(path, statsOutput);
            }
        }
    }
}
