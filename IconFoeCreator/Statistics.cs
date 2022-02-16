using System;
using System.Collections.Generic;
using System.Linq;

namespace IconFoeCreator
{
    public class Statistics
    {
        public string Name { get; set; } // Does not inherit
        public string Inherits { get; set; } // Does not inherit
        public bool Inherited { get; set; } // Does not inherit
        public string Type { get; set; } // Does not inherit
        public string Group { get; set; }
        public bool IsBaseTemplate { get; set; } // Does not inherit
        public bool? IsMob { get; set; }
        public bool? IsElite { get; set; }
        public bool? IsLegend { get; set; }
        public bool? IsUniqueJob { get; set; }
        public string UsesTemplate { get; set; } // Does not inherit
        public string UsesClass { get; set; } // Does not inherit
        public bool RestrictToBaseTemplates { get; set; } // Does not inherit
        public int Chapter { get; set; } // Does not inherit
        public int? Vitality { get; set; }
        public int? HP { get; set; }
        public double? AddHPPercent { get; set; }
        public int? HPMultiplier { get; set; }
        public bool? DoubleNormalFoeHP { get; set; }
        public bool? HPMultiplyByPlayers { get; set; }
        public int? Speed { get; set; }
        public int? Dash { get; set; }
        public int? Defense { get; set; }
        public int? Armor { get; set; }
        public int? FrayDamage { get; set; }
        public string DamageDie { get; set; }
        public string FactionBlight { get; set; }
        public List<Trait> Traits { get; set; } // Additive inheritance
        public List<string> RemoveTraits { get; set; }
        public List<Trait> SetupTraits { get; set; } // Additive inheritance
        public List<Interrupt> Interrupts { get; set; } // Additive inheritance
        public List<Action> Actions { get; set; } // Additive inheritance
        public List<BodyPart> BodyParts { get; set; } // Additive inheritance
        public string PhasesDescription { get; set; }
        public List<Phase> Phases { get; set; }
        public bool IsHomebrew { get; set; } // To be used and set internally

        public Statistics()
        {
            Name = String.Empty;
            Inherits = String.Empty;
            Inherited = false;
            Type = String.Empty;
            Traits = new List<Trait>();
            RemoveTraits = new List<string>();
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
            Statistics newStats = new Statistics
            {
                Inherited = true,
                Name = Name,
                Inherits = Inherits,
                Type = Type,
                IsBaseTemplate = IsBaseTemplate,
                UsesTemplate = UsesTemplate,
                UsesClass = UsesClass,
                RestrictToBaseTemplates = RestrictToBaseTemplates,
                Chapter = Chapter,
                IsHomebrew = IsHomebrew
            };

            // Inherited values
            if (!String.IsNullOrEmpty(Group)) { newStats.Group = Group; } else { newStats.Group = otherStats.Group; }
            if (IsMob.HasValue) { newStats.IsMob = IsMob; } else { newStats.IsMob = otherStats.IsMob; }
            if (IsElite.HasValue) { newStats.IsElite = IsElite; } else { newStats.IsElite = otherStats.IsElite; }
            if (IsLegend.HasValue) { newStats.IsLegend = IsLegend; } else { newStats.IsLegend = otherStats.IsLegend; }
            if (IsUniqueJob.HasValue) { newStats.IsUniqueJob = IsUniqueJob; } else { newStats.IsUniqueJob = otherStats.IsUniqueJob; }
            if (Vitality.HasValue) { newStats.Vitality = Vitality; } else { newStats.Vitality = otherStats.Vitality; }
            if (HP.HasValue) { newStats.HP = HP; } else { newStats.HP = otherStats.HP; }
            if (AddHPPercent.HasValue) { newStats.AddHPPercent = AddHPPercent; } else { newStats.AddHPPercent = otherStats.AddHPPercent; }
            if (HPMultiplier.HasValue) { newStats.HPMultiplier = HPMultiplier; } else { newStats.HPMultiplier = otherStats.HPMultiplier; }
            if (HPMultiplyByPlayers.HasValue) { newStats.HPMultiplyByPlayers = HPMultiplyByPlayers; } else { newStats.HPMultiplyByPlayers = otherStats.HPMultiplyByPlayers; }
            if (Speed.HasValue) { newStats.Speed = Speed; } else { newStats.Speed = otherStats.Speed; }
            if (Dash.HasValue) { newStats.Dash = Dash; } else { newStats.Dash = otherStats.Dash; }
            if (Defense.HasValue) { newStats.Defense = Defense; } else { newStats.Defense = otherStats.Defense; }
            if (Armor.HasValue) { newStats.Armor = Armor; } else { newStats.Armor = otherStats.Armor; }
            if (FrayDamage.HasValue) { newStats.FrayDamage = FrayDamage; } else { newStats.FrayDamage = otherStats.FrayDamage; }
            if (!String.IsNullOrEmpty(DamageDie)) { newStats.DamageDie = DamageDie; } else { newStats.DamageDie = otherStats.DamageDie; }
            if (!String.IsNullOrEmpty(FactionBlight)) { newStats.FactionBlight = FactionBlight; } else { newStats.FactionBlight = otherStats.FactionBlight; }
            if (!String.IsNullOrEmpty(PhasesDescription)) { newStats.PhasesDescription = PhasesDescription; } else { newStats.PhasesDescription = otherStats.PhasesDescription; }
            if (Phases.Count() > 0) { newStats.Phases = Phases; } else { newStats.Phases = otherStats.Phases; }

            // Additive statistics
            newStats.Traits.AddRange(otherStats.Traits);
            newStats.Traits.AddRange(Traits);
            newStats.RemoveTraits.AddRange(otherStats.RemoveTraits);
            newStats.RemoveTraits.AddRange(RemoveTraits);
            newStats.SetupTraits.AddRange(otherStats.SetupTraits);
            newStats.SetupTraits.AddRange(SetupTraits);
            newStats.Interrupts.AddRange(otherStats.Interrupts);
            newStats.Interrupts.AddRange(Interrupts);
            newStats.Actions.AddRange(otherStats.Actions);
            newStats.Actions.AddRange(Actions);
            newStats.BodyParts.AddRange(otherStats.BodyParts);
            newStats.BodyParts.AddRange(BodyParts);

            // Remove traits named to remove
            for (int i = 0; i < newStats.RemoveTraits.Count; ++i)
            {
                string statNameToRemove = newStats.RemoveTraits[i].ToLower();
                for (int j = 0; j < newStats.Traits.Count;)
                {
                    string statName = newStats.Traits[j].Name.ToLower();
                    if (statName == statNameToRemove)
                    {
                        newStats.Traits.RemoveAt(j);
                    }
                    else
                    {
                        ++j;
                    }
                }
            }

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
        public string DescriptionNonessential { get; set; }
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
        public string Trigger { get; set; }
        public string Effect { get; set; }
        public string Collide { get; set; }

        public Interrupt()
        {
            Tags = new List<string>();
        }
    }

    public class Action
    {
        public string Name { get; set; }
        public int ActionCost { get; set; }
        public int Recharge { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public string Hit { get; set; }
        public string AutoHit { get; set; }
        public string Critical { get; set; }
        public string Miss { get; set; }
        public string PostAttack { get; set; }
        public string AreaEffect { get; set; }
        public string Effect { get; set; }
        public string Mark { get; set; }
        public string Stance { get; set; }
        public string Collide { get; set; }
        public string Blightboost { get; set; }
        public string TerrainEffect { get; set; }
        public string Special { get; set; }
        public List<Action> Combos { get; set; }
        public string PostAction { get; set; }

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
        public bool HPMultiplyByPlayers { get; set; }
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
