using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconFoeCreator
{
    public class Statistics
    {
        public string Name { get; set; }
        public string Inherits { get; set; }
        public bool Inherited { get; set; }
        public string Type { get; set; }
        public int?[] Health { get; set; }
        public int? HPMultiplier { get; set; }
        public int? Speed { get; set; }
        public int? Run { get; set; }
        public int? Dash { get; set; }
        public int? Defense { get; set; }
        public int?[] Armor { get; set; }
        public int?[] Attack { get; set; }
        public int?[] FrayDamage { get; set; }
        public int? LightDamage { get; set; }
        public int? HeavyDamage { get; set; }
        public int? CriticalDamage { get; set; }
        public string LightDamageDie { get; set; }
        public string HeavyDamageDie { get; set; }
        public string CriticalDamageDie { get; set; }
        public string DamageType { get; set; }
        public List<Trait> Traits { get; set; }
        public List<Interrupt> Interrupts { get; set; }
        public List<Action> Actions { get; set; }
        public string FactionBlight { get; set; }

        public Statistics()
        {
            Name = String.Empty;
            Inherits = String.Empty;
            Inherited = false;
            Type = String.Empty;
            Health = new int?[Constants.ChapterCount];
            Armor = new int?[Constants.ChapterCount];
            Attack = new int?[Constants.ChapterCount];
            FrayDamage = new int?[Constants.ChapterCount];
            Traits = new List<Trait>();
            Interrupts = new List<Interrupt>();
            Actions = new List<Action>();
        }

        public override string ToString()
        {
            return Name;
        }

        public Statistics InheritFrom(Statistics otherStats)
        {
            Statistics newStats = new Statistics();

            if (otherStats.Name != null && otherStats.Name != String.Empty) { newStats.Name = otherStats.Name; } else { newStats.Name = Name; }
            if (otherStats.Inherits != null && otherStats.Inherits != String.Empty) { newStats.Inherits = otherStats.Inherits; } else { newStats.Inherits = Inherits; }
            if (otherStats.Type != null && otherStats.Type != String.Empty) { newStats.Type = otherStats.Type; } else { newStats.Type = Type; }
            if (otherStats.HPMultiplier.HasValue) { newStats.HPMultiplier = otherStats.HPMultiplier; } else { newStats.HPMultiplier = HPMultiplier; }
            if (otherStats.Speed.HasValue) { newStats.Speed = otherStats.Speed; } else { newStats.Speed = Speed; }
            if (otherStats.Run.HasValue) { newStats.Run = otherStats.Run; } else { newStats.Run = Run; }
            if (otherStats.Dash.HasValue) { newStats.Dash = otherStats.Dash; } else { newStats.Dash = Dash; }
            if (otherStats.Defense.HasValue) { newStats.Defense = otherStats.Defense; } else { newStats.Defense = Defense; }
            if (otherStats.LightDamage.HasValue) { newStats.LightDamage = otherStats.LightDamage; } else { newStats.LightDamage = LightDamage; }
            if (otherStats.HeavyDamage.HasValue) { newStats.HeavyDamage = otherStats.HeavyDamage; } else { newStats.HeavyDamage = HeavyDamage; }
            if (otherStats.CriticalDamage.HasValue) { newStats.CriticalDamage = otherStats.CriticalDamage; } else { newStats.CriticalDamage = CriticalDamage; }
            if (otherStats.LightDamageDie != null && otherStats.LightDamageDie != String.Empty) { newStats.LightDamageDie = otherStats.LightDamageDie; } else { newStats.LightDamageDie = LightDamageDie; }
            if (otherStats.HeavyDamageDie != null && otherStats.HeavyDamageDie != String.Empty) { newStats.HeavyDamageDie = otherStats.HeavyDamageDie; } else { newStats.HeavyDamageDie = HeavyDamageDie; }
            if (otherStats.CriticalDamageDie != null && otherStats.CriticalDamageDie != String.Empty) { newStats.CriticalDamageDie = otherStats.CriticalDamageDie; } else { newStats.CriticalDamageDie = CriticalDamageDie; }
            if (otherStats.DamageType != null && otherStats.DamageType != String.Empty) { newStats.DamageType = otherStats.DamageType; } else { newStats.DamageType = DamageType; }
            if (otherStats.FactionBlight != null && otherStats.FactionBlight != String.Empty) { newStats.FactionBlight = otherStats.FactionBlight; } else { newStats.FactionBlight = FactionBlight; }

            // These replace those of a specific size
            for (int i = 0; i < Constants.ChapterCount; ++i)
            {
                if (otherStats.Health[i].HasValue) { newStats.Health[i] = otherStats.Health[i]; } else { newStats.Health[i] = Health[i]; }
                if (otherStats.Armor[i].HasValue) { newStats.Armor[i] = otherStats.Armor[i]; } else { newStats.Armor[i] = Armor[i]; }
                if (otherStats.Attack[i].HasValue) { newStats.Attack[i] = otherStats.Attack[i]; } else { newStats.Attack[i] = Attack[i]; }
                if (otherStats.FrayDamage[i].HasValue) { newStats.FrayDamage[i] = otherStats.FrayDamage[i]; } else { newStats.FrayDamage[i] = FrayDamage[i]; }
            }

            // Additive statistics
            newStats.Traits.AddRange(Traits);
            newStats.Traits.AddRange(otherStats.Traits);
            newStats.Interrupts.AddRange(Interrupts);
            newStats.Interrupts.AddRange(otherStats.Interrupts);
            newStats.Actions.AddRange(Actions);
            newStats.Actions.AddRange(otherStats.Actions);

            // Remove traits with duplicate names
            for (int i = 0; i < newStats.Traits.Count; ++i)
            {
                string statName = newStats.Traits[i].Name.ToLower();
                for (int j = i + 1; j < newStats.Traits.Count; ++j)
                {
                    string otherStatName = newStats.Traits[j].Name.ToLower();
                    if (statName == otherStatName)
                    {
                        newStats.Traits.RemoveAt(j);
                    }
                }
            }

            newStats.Inherited = true;

            return newStats;
        }
    }

    public class Trait
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public double? AddHP { get; set; }
        public int? AddArmor { get; set; }
        public int? MaxArmor { get; set; }
        public bool? NoRun { get; set; }
        public bool? NoDash { get; set; }

        public Trait()
        {
            Tags = new List<string>();
        }
    }

    public class Interrupt
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }

        public Interrupt()
        {
            Tags = new List<string>();
        }
    }

    public class Action
    {
        public string Name { get; set; }
        public int ActionCost { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public string Hit { get; set; }
        public string CriticalHit { get; set; }
        public string Miss { get; set; }
        public string AreaEffect { get; set; }
        public string Effect { get; set; }
        public string Collide { get; set; }
        public string Blightboost { get; set; }

        public Action()
        {
            Tags = new List<string>();
        }
    }
}
