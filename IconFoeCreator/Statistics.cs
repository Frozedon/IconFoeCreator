using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IconFoeCreator
{
    public class Statistics
    {
        // Does Not Inherit
        public string Name { get; set; }
        public string Type { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Inherits { get; set; } 

        public int Chapter { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveInterrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveActions { get; set; }

        // Normal Inheritance
        public string Faction { get; set; }
        public string Class { get; set; }
        public bool? IsMob { get; set; }
        public int? Vitality { get; set; }
        public int? HP { get; set; }
        public int? HPMultiplier { get; set; }
        public bool? HPMultiplyByPlayers { get; set; }
        public int? Speed { get; set; }
        public float? DashMultiplier { get; set; }
        public int? Defense { get; set; }
        public int? FrayDamage { get; set; }
        public int? DamageDie { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<BodyPart>))]
        public List<BodyPart> BodyParts { get; set; }

        public string PhasesDescription { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Phase>))]
        public List<Phase> Phases { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<AbilitySet>))]
        public List<AbilitySet> ExtraAbilitySets { get; set; }

        public ChapterData Chapter2 { get; set; }
        public ChapterData Chapter3 { get; set; }


        // Additive Inheritance
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Trait>))]
        public List<Trait> SetupTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Interrupt>))]
        public List<Interrupt> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }


        // Used by code
        private List<Trait> mActualTraits;


        public Statistics()
        {
            Name = String.Empty;
            Type = String.Empty;
            Inherits = new List<string>();
            Chapter = 0;
            Faction = String.Empty;
            Class = String.Empty;
            BodyParts = new List<BodyPart>();
            PhasesDescription = String.Empty;
            Phases = new List<Phase>();
            ExtraAbilitySets = new List<AbilitySet>();
            Traits = new List<string>();
            RemoveTraits = new List<string>();
            RemoveInterrupts = new List<string>();
            RemoveActions = new List<string>();
            SetupTraits = new List<Trait>();
            Interrupts = new List<Interrupt>();
            Actions = new List<Action>();
            mActualTraits = new List<Trait>();
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
                Type = Type,
                Inherits = Inherits,
                Chapter = Chapter
            };

            // Inherited values
            if (!String.IsNullOrEmpty(Faction)) { newStats.Faction = Faction; } else { newStats.Faction = otherStats.Faction; }
            if (!String.IsNullOrEmpty(Class)) { newStats.Class = Class; } else { newStats.Class = otherStats.Class; }
            if (IsMob.HasValue) { newStats.IsMob = IsMob; } else { newStats.IsMob = otherStats.IsMob; }
            if (Vitality.HasValue) { newStats.Vitality = Vitality; } else { newStats.Vitality = otherStats.Vitality; }
            if (HP.HasValue) { newStats.HP = HP; } else { newStats.HP = otherStats.HP; }
            if (HPMultiplier.HasValue) { newStats.HPMultiplier = HPMultiplier; } else { newStats.HPMultiplier = otherStats.HPMultiplier; }
            if (HPMultiplyByPlayers.HasValue) { newStats.HPMultiplyByPlayers = HPMultiplyByPlayers; } else { newStats.HPMultiplyByPlayers = otherStats.HPMultiplyByPlayers; }
            if (Speed.HasValue) { newStats.Speed = Speed; } else { newStats.Speed = otherStats.Speed; }
            if (Defense.HasValue) { newStats.Defense = Defense; } else { newStats.Defense = otherStats.Defense; }
            if (FrayDamage.HasValue) { newStats.FrayDamage = FrayDamage; } else { newStats.FrayDamage = otherStats.FrayDamage; }
            if (DamageDie.HasValue) { newStats.DamageDie = DamageDie; } else { newStats.DamageDie = otherStats.DamageDie; }
            if (BodyParts.Count > 0) { newStats.BodyParts = BodyParts; } else { newStats.BodyParts = otherStats.BodyParts; }
            if (!String.IsNullOrEmpty(PhasesDescription)) { newStats.PhasesDescription = PhasesDescription; } else { newStats.PhasesDescription = otherStats.PhasesDescription; }
            if (Phases.Count() > 0) { newStats.Phases = Phases; } else { newStats.Phases = otherStats.Phases; }
            if (ExtraAbilitySets.Count() > 0) { newStats.ExtraAbilitySets = ExtraAbilitySets; } else { newStats.ExtraAbilitySets = otherStats.ExtraAbilitySets; }

            // Additive statistics
            newStats.SetupTraits.AddRange(SetupTraits);
            newStats.SetupTraits.AddRange(otherStats.SetupTraits);

            // Only add traits, interrupts, and actions from the inherited that are not on the current removal lists
            newStats.Traits.AddRange(Traits);
            foreach (string trait in otherStats.Traits)
            {
                if (!RemoveTraits.Exists(x => x == trait)) {
                    newStats.Traits.Add(trait);
                }
            }
            newStats.Traits = newStats.Traits.Distinct().ToList();

            newStats.Interrupts.AddRange(Interrupts);
            foreach (Interrupt interrupt in otherStats.Interrupts)
            {
                if (!RemoveInterrupts.Exists(x => x == interrupt.Name))
                {
                    newStats.Interrupts.Add(interrupt);
                }
            }

            newStats.Actions.AddRange(Actions);
            foreach (Action action in otherStats.Actions)
            {
                if (!RemoveActions.Exists(x => x == action.Name))
                {
                    newStats.Actions.Add(action);
                }
            }


            return newStats;
        }

        public static bool IsValid(Statistics stats)
        {
            return stats != null && stats.Name != null && stats.Name != "...";
        }

        public static List<Trait> GetActualTraits(List<string> traits, List<Trait> traitLib)
        {
            List<Trait> updatedTraits = new List<Trait>();

            foreach (string traitName in traits)
            {
                Trait traitFound = traitLib.Find(x => x.Matches(traitName));
                if (traitFound != null)
                {
                    updatedTraits.Add(traitFound.MakeExpressedTrait(traitName));
                }
                else
                {
                    updatedTraits.Add(new Trait()
                    {
                        Name = traitName
                    });
                }
            }

            return updatedTraits;
        }

        public List<Trait> LoadActualTraits(List<Trait> traitLib)
        {
            mActualTraits = GetActualTraits(Traits, traitLib);
            return mActualTraits;
        }

        public int GetDash()
        {
            List<Trait> traitWithDash = mActualTraits.FindAll(x => x.DashMultiplier.HasValue);

            float dashMultiplier = float.MaxValue;
            foreach (Trait trait in traitWithDash)
            {
                float value = trait.DashMultiplier.Value;
                if (value < dashMultiplier)
                {
                    dashMultiplier = value;
                }
            }

            if (dashMultiplier == float.MaxValue)
            {
                dashMultiplier = 0.5f;
            }

            return (int)Math.Ceiling((float)Speed.GetValueOrDefault(0) * dashMultiplier);
        }

        public int GetDefense()
        {
            List<Trait> traitWithDefense = mActualTraits.FindAll(x => x.Defense.HasValue);

            int defense = int.MaxValue;
            foreach (Trait trait in traitWithDefense)
            {
                int value = trait.Defense.Value;
                if (value < defense)
                {
                    defense = value;
                }
            }

            if (defense != int.MaxValue)
            {
                return defense;
            }
            else
            {
                return Defense.GetValueOrDefault(0);
            }
        }

        public double GetEncounterBudget()
        {
            List<Trait> traitWithEB = mActualTraits.FindAll(x => x.EncounterBudget.HasValue);

            double encounterBudget = double.MaxValue;
            foreach (Trait trait in traitWithEB)
            {
                double value = trait.EncounterBudget.Value;
                if (value < encounterBudget)
                {
                    encounterBudget = value;
                }
            }

            if (encounterBudget != double.MaxValue)
            {
                return encounterBudget;
            }
            else
            {
                return 1.0;
            }
        }
    }

    public class Trait
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public float? DashMultiplier { get; set; }
        public int? Defense { get; set; }
        public double? EncounterBudget { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }

        public Trait()
        {
            Actions = new List<Action>();
        }

        private static readonly string VALUE_TOKEN = "[X]";

        public bool Matches(string traitName)
        {
            if (Name.Contains(VALUE_TOKEN))
            {
                string regexName = "^" + Name.Replace(VALUE_TOKEN, "\\d+") + "$";
                return Regex.IsMatch(traitName, regexName, RegexOptions.IgnoreCase);
            }
            else
            {
                return Name == traitName;
            }
        }

        public Trait MakeExpressedTrait(string traitName)
        {
            if (Name.Contains(VALUE_TOKEN))
            {
                Trait newTrait = new Trait()
                {
                    Name = Name,
                    Description = Description,
                    DashMultiplier = DashMultiplier,
                    Defense = Defense,
                    EncounterBudget = EncounterBudget,
                    Actions = Actions
                };

                string strippedTraitName = (string)traitName.Clone();
                string[] substrings = Name.Split(VALUE_TOKEN.ToCharArray());
                if (substrings.Length > 0)
                {
                    strippedTraitName = strippedTraitName.Replace(substrings[0], "");
                }
                string valueStr = Regex.Match(strippedTraitName, @"\d+", RegexOptions.IgnoreCase).Value;

                newTrait.Name = Name.Replace(VALUE_TOKEN, valueStr);
                newTrait.Description = Description.Replace(VALUE_TOKEN, valueStr);

                return newTrait;
            }
            else
            {
                return this;
            }
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
        public string SpecialInterrupt { get; set; }
        public string SpecialRecharge { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Combos { get; set; }

        public string PostAction { get; set; }

        public Action()
        {
            Tags = new List<string>();
            Combos = new List<Action>();
        }
    }

    public class ChapterData
    {
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }
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

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }

        public Phase()
        {
            Traits = new List<string>();
            Actions = new List<Action>();
        }
    }

    public class AbilitySet
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }

        public AbilitySet()
        {
            Traits = new List<string>();
            Actions = new List<Action>();
        }
    }
}
