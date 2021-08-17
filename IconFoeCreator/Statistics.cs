using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconFoeCreator
{
    public class Statistics
    {
        public string Name { get; set; } // Does not inherit
        public string Inherits { get; set; } // Does not inherit
        public bool Inherited { get; set; } // Does not inherit
        public string Group { get; set; }
        public string Type { get; set; } // Does not inherit
        public int?[] Health { get; set; }
        public int? HPMultiplier { get; set; }
        public bool? HPMultiplyByPlayers { get; set; }
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
        public List<Trait> Traits { get; set; } // Additive inheritance
        public List<Trait> SetupTraits { get; set; } // Additive inheritance
        public List<Interrupt> Interrupts { get; set; } // Additive inheritance
        public List<Action> Actions { get; set; } // Additive inheritance
        public string FactionBlight { get; set; }
        public List<BodyPart> BodyParts { get; set; } // Additive inheritance
        public string PhasesDescription { get; set; }
        public List<Phase> Phases { get; set; }

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
            SetupTraits = new List<Trait>();
            Interrupts = new List<Interrupt>();
            Actions = new List<Action>();
            BodyParts = new List<BodyPart>();
            Phases = new List<Phase>();
        }

        public override string ToString()
        {
            return Name;
        }

        public Statistics InheritFrom(Statistics otherStats)
        {
            Statistics newStats = new Statistics();
            newStats.Inherited = true;

            newStats.Name = Name;
            newStats.Inherits = Inherits;
            newStats.Type = Type;

            // Inheritted values
            if (Group != null && Group != String.Empty) { newStats.Group = Group; } else { newStats.Group = otherStats.Group; }
            if (HPMultiplier.HasValue) { newStats.HPMultiplier = HPMultiplier; } else { newStats.HPMultiplier = otherStats.HPMultiplier; }
            if (HPMultiplyByPlayers.HasValue) { newStats.HPMultiplyByPlayers = HPMultiplyByPlayers; } else { newStats.HPMultiplyByPlayers = otherStats.HPMultiplyByPlayers; }
            if (Speed.HasValue) { newStats.Speed = Speed; } else { newStats.Speed = otherStats.Speed; }
            if (Run.HasValue) { newStats.Run = Run; } else { newStats.Run = otherStats.Run; }
            if (Dash.HasValue) { newStats.Dash = Dash; } else { newStats.Dash = otherStats.Dash; }
            if (Defense.HasValue) { newStats.Defense = Defense; } else { newStats.Defense = otherStats.Defense; }
            if (LightDamage.HasValue) { newStats.LightDamage = LightDamage; } else { newStats.LightDamage = otherStats.LightDamage; }
            if (HeavyDamage.HasValue) { newStats.HeavyDamage = HeavyDamage; } else { newStats.HeavyDamage = otherStats.HeavyDamage; }
            if (CriticalDamage.HasValue) { newStats.CriticalDamage = CriticalDamage; } else { newStats.CriticalDamage = otherStats.CriticalDamage; }
            if (LightDamageDie != null && LightDamageDie != String.Empty) { newStats.LightDamageDie = LightDamageDie; } else { newStats.LightDamageDie = otherStats.LightDamageDie; }
            if (HeavyDamageDie != null && HeavyDamageDie != String.Empty) { newStats.HeavyDamageDie = HeavyDamageDie; } else { newStats.HeavyDamageDie = otherStats.HeavyDamageDie; }
            if (CriticalDamageDie != null && CriticalDamageDie != String.Empty) { newStats.CriticalDamageDie = CriticalDamageDie; } else { newStats.CriticalDamageDie = otherStats.CriticalDamageDie; }
            if (DamageType != null && DamageType != String.Empty) { newStats.DamageType = DamageType; } else { newStats.DamageType = otherStats.DamageType; }
            if (FactionBlight != null && FactionBlight != String.Empty) { newStats.FactionBlight = FactionBlight; } else { newStats.FactionBlight = otherStats.FactionBlight; }
            if (PhasesDescription != null && PhasesDescription != String.Empty) { newStats.PhasesDescription = PhasesDescription; } else { newStats.PhasesDescription = otherStats.PhasesDescription; }
            if (Phases.Count() > 0) { newStats.Phases = Phases; } else { newStats.Phases = otherStats.Phases; }

            // These replace those of a specific size
            for (int i = 0; i < Constants.ChapterCount; ++i)
            {
                if (Health[i].HasValue) { newStats.Health[i] = Health[i]; } else { newStats.Health[i] = otherStats.Health[i]; }
                if (Armor[i].HasValue) { newStats.Armor[i] = Armor[i]; } else { newStats.Armor[i] = otherStats.Armor[i]; }
                if (Attack[i].HasValue) { newStats.Attack[i] = Attack[i]; } else { newStats.Attack[i] = otherStats.Attack[i]; }
                if (FrayDamage[i].HasValue) { newStats.FrayDamage[i] = FrayDamage[i]; } else { newStats.FrayDamage[i] = otherStats.FrayDamage[i]; }
            }

            // Additive statistics
            newStats.Traits.AddRange(otherStats.Traits);
            newStats.Traits.AddRange(Traits);
            newStats.SetupTraits.AddRange(otherStats.SetupTraits);
            newStats.SetupTraits.AddRange(SetupTraits);
            newStats.Interrupts.AddRange(otherStats.Interrupts);
            newStats.Interrupts.AddRange(Interrupts);
            newStats.Actions.AddRange(otherStats.Actions);
            newStats.Actions.AddRange(Actions);
            newStats.BodyParts.AddRange(otherStats.BodyParts);
            newStats.BodyParts.AddRange(BodyParts);

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

            return newStats;
        }

        public static bool IsValid(Statistics stats)
        {
            return stats != null && stats.Name != null && stats.Name != "...";
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
        public bool Nonessential { get; set; }

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
        public List<Action> Combos { get; set; }

        public Action()
        {
            Tags = new List<string>();
            Combos = new List<Action>();
        }
    }

    public class BodyPart
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public string Description { get; set; }
    }

    public class Phase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Trait> Traits { get; set; }
        public List<Action> Actions { get; set; }

        public Phase()
        {
            Traits = new List<Trait>();
            Actions = new List<Action>();
        }
    }
}
