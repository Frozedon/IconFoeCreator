using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IconFoeCreator
{
    public class Statistics
    {
        public string Name { get; set; } // Does not inherit
        public string TitleName { get; set; } // Does not inherit

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Inherits { get; set; } // Does not inherit

        public string Type { get; set; } // Does not inherit
        public string Group { get; set; }
        public bool DoNotDisplay { get; set; } // Does not inherit
        public double? EncounterBudget { get; set; }
        public bool IsBasicTemplate { get; set; } // Does not inherit
        public bool? IsMob { get; set; }
        public bool? IsElite { get; set; }
        public bool? IsLegend { get; set; }
        public string UsesClass { get; set; }
        public string UsesFaction { get; set; }
        public bool? RestrictToBasicTemplates { get; set; }
        public int Chapter { get; set; } // Does not inherit
        public int? Vitality { get; set; }
        public int? HP { get; set; }
        public double? AddHPPercent { get; set; } // Additive inheritance
        public int? HPMultiplier { get; set; }
        public bool? DoubleNormalFoeHP { get; set; }
        public bool? HPMultiplyByPlayers { get; set; }
        public int? Speed { get; set; }
        public int? Dash { get; set; }
        public int? Defense { get; set; }
        public int? Armor { get; set; }
        public int? FrayDamage { get; set; }
        public int? DamageDie { get; set; }
        public string FactionBlight { get; set; }
        public int? MinimumRecharge { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Trait>))]
        public List<Trait> Traits { get; set; } // Additive inheritance

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveTraits { get; set; } // Additive inheritance

        [JsonConverter(typeof(SingleOrArrayConverter<Trait>))]
        public List<Trait> SetupTraits { get; set; } // Additive inheritance

        [JsonConverter(typeof(SingleOrArrayConverter<Interrupt>))]
        public List<Interrupt> Interrupts { get; set; } // Additive inheritance

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; } // Additive inheritance

        [JsonConverter(typeof(SingleOrArrayConverter<BodyPart>))]
        public List<BodyPart> BodyParts { get; set; } // Additive inheritance

        public string PhasesDescription { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Phase>))]
        public List<Phase> Phases { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<AbilitySet>))]
        public List<AbilitySet> ExtraAbilitySets { get; set; }

        public bool IsHomebrew { get; set; } // To be used and set internally

        public Statistics()
        {
            Name = String.Empty;
            TitleName = String.Empty;
            Inherits = new List<string>();
            Type = String.Empty;
            Traits = new List<Trait>();
            RemoveTraits = new List<string>();
            SetupTraits = new List<Trait>();
            Interrupts = new List<Interrupt>();
            Actions = new List<Action>();
            BodyParts = new List<BodyPart>();
            Phases = new List<Phase>();
            ExtraAbilitySets = new List<AbilitySet>();
        }

        public override string ToString()
        {
            return Name;
        }

        public Statistics InheritFrom(Statistics otherStats)
        {
            Statistics newStats = new Statistics
            {
                Name = Name,
                TitleName = TitleName,
                Inherits = Inherits,
                Type = Type,
                IsBasicTemplate = IsBasicTemplate,
                DoNotDisplay = DoNotDisplay,
                Chapter = Chapter,
                IsHomebrew = IsHomebrew
            };

            // Inherited values
            if (!String.IsNullOrEmpty(Group)) { newStats.Group = Group; } else { newStats.Group = otherStats.Group; }
            if (EncounterBudget.HasValue) { newStats.EncounterBudget = EncounterBudget; } else { newStats.EncounterBudget = otherStats.EncounterBudget; }
            if (IsMob.HasValue) { newStats.IsMob = IsMob; } else { newStats.IsMob = otherStats.IsMob; }
            if (IsElite.HasValue) { newStats.IsElite = IsElite; } else { newStats.IsElite = otherStats.IsElite; }
            if (IsLegend.HasValue) { newStats.IsLegend = IsLegend; } else { newStats.IsLegend = otherStats.IsLegend; }
            if (!String.IsNullOrEmpty(UsesClass)) { newStats.UsesClass = UsesClass; } else { newStats.UsesClass = otherStats.UsesClass; }
            if (!String.IsNullOrEmpty(UsesFaction)) { newStats.UsesFaction = UsesFaction; } else { newStats.UsesFaction = otherStats.UsesFaction; }
            if (RestrictToBasicTemplates.HasValue) { newStats.RestrictToBasicTemplates = RestrictToBasicTemplates; } else { newStats.RestrictToBasicTemplates = otherStats.RestrictToBasicTemplates; }
            if (Vitality.HasValue) { newStats.Vitality = Vitality; } else { newStats.Vitality = otherStats.Vitality; }
            if (HP.HasValue) { newStats.HP = HP; } else { newStats.HP = otherStats.HP; }
            if (HPMultiplier.HasValue) { newStats.HPMultiplier = HPMultiplier; } else { newStats.HPMultiplier = otherStats.HPMultiplier; }
            if (DoubleNormalFoeHP.HasValue) { newStats.DoubleNormalFoeHP = DoubleNormalFoeHP; } else { newStats.DoubleNormalFoeHP = otherStats.DoubleNormalFoeHP; }
            if (HPMultiplyByPlayers.HasValue) { newStats.HPMultiplyByPlayers = HPMultiplyByPlayers; } else { newStats.HPMultiplyByPlayers = otherStats.HPMultiplyByPlayers; }
            if (Speed.HasValue) { newStats.Speed = Speed; } else { newStats.Speed = otherStats.Speed; }
            if (Dash.HasValue) { newStats.Dash = Dash; } else { newStats.Dash = otherStats.Dash; }
            if (Defense.HasValue) { newStats.Defense = Defense; } else { newStats.Defense = otherStats.Defense; }
            if (Armor.HasValue) { newStats.Armor = Armor; } else { newStats.Armor = otherStats.Armor; }
            if (FrayDamage.HasValue) { newStats.FrayDamage = FrayDamage; } else { newStats.FrayDamage = otherStats.FrayDamage; }
            if (DamageDie.HasValue) { newStats.DamageDie = DamageDie; } else { newStats.DamageDie = otherStats.DamageDie; }
            if (!String.IsNullOrEmpty(FactionBlight)) { newStats.FactionBlight = FactionBlight; } else { newStats.FactionBlight = otherStats.FactionBlight; }
            if (!String.IsNullOrEmpty(PhasesDescription)) { newStats.PhasesDescription = PhasesDescription; } else { newStats.PhasesDescription = otherStats.PhasesDescription; }
            if (Phases.Count() > 0) { newStats.Phases = Phases; } else { newStats.Phases = otherStats.Phases; }
            if (MinimumRecharge.HasValue) { newStats.MinimumRecharge = MinimumRecharge; } else { newStats.MinimumRecharge = otherStats.MinimumRecharge; }

            // Additive statistics
            newStats.AddHPPercent = AddHPPercent.GetValueOrDefault(0.0) + otherStats.AddHPPercent.GetValueOrDefault(0.0);
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
            newStats.ExtraAbilitySets.AddRange(otherStats.ExtraAbilitySets);
            newStats.ExtraAbilitySets.AddRange(ExtraAbilitySets);

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

                for (int j = 0; j < newStats.SetupTraits.Count;)
                {
                    string statName = newStats.SetupTraits[j].Name.ToLower();
                    if (statName == statNameToRemove)
                    {
                        newStats.SetupTraits.RemoveAt(j);
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

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
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

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
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

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
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

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
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

        [JsonConverter(typeof(SingleOrArrayConverter<Trait>))]
        public List<Trait> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }

        public Phase()
        {
            Traits = new List<Trait>();
            Actions = new List<Action>();
        }
    }

    public class AbilitySet
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Trait>))]
        public List<Trait> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }

        public AbilitySet()
        {
            Traits = new List<Trait>();
            Actions = new List<Action>();
        }
    }
}
