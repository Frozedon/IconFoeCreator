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
        public bool UsesTemplate { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveInterrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveActions { get; set; }


        // Normal Inheritance
        public string Faction { get; set; }
        public string Class { get; set; }
        public string SpecialClass { get; set; }
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
        private float mDashMultiplier;
        private double mEncounterBudget;


        public Statistics()
        {
            Name = String.Empty;
            Type = String.Empty;
            Inherits = new List<string>();
            Chapter = 0;
            UsesTemplate = false;
            Faction = String.Empty;
            Class = String.Empty;
            SpecialClass = String.Empty;
            BodyParts = new List<BodyPart>();
            PhasesDescription = String.Empty;
            Phases = new List<Phase>();
            ExtraAbilitySets = new List<AbilitySet>();
            Chapter2 = new ChapterData();
            Chapter3 = new ChapterData();
            Traits = new List<string>();
            RemoveTraits = new List<string>();
            RemoveInterrupts = new List<string>();
            RemoveActions = new List<string>();
            SetupTraits = new List<Trait>();
            Interrupts = new List<Interrupt>();
            Actions = new List<Action>();
            mActualTraits = new List<Trait>();
            mDashMultiplier = 0.5f;
            mEncounterBudget = 1.0;
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
                Chapter = Chapter,
                UsesTemplate = UsesTemplate
            };

            // Inherited values
            if (!String.IsNullOrEmpty(Faction)) { newStats.Faction = Faction; } else { newStats.Faction = otherStats.Faction; }
            if (!String.IsNullOrEmpty(Class)) { newStats.Class = Class; } else { newStats.Class = otherStats.Class; }
            if (!String.IsNullOrEmpty(SpecialClass)) { newStats.SpecialClass = SpecialClass; } else { newStats.SpecialClass = otherStats.SpecialClass; }
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
            InheritTraits(newStats.Traits, otherStats.Traits, RemoveTraits);
            newStats.Traits = newStats.Traits.Distinct().ToList();

            newStats.Interrupts.AddRange(Interrupts);
            InheritInterrupts(newStats.Interrupts, otherStats.Interrupts, RemoveInterrupts);

            newStats.Actions.AddRange(Actions);
            InheritActions(newStats.Actions, otherStats.Actions, RemoveActions);

            // Keep all the data for chapter 2 and 3
            newStats.Chapter2.Traits.AddRange(Chapter2.Traits);
            newStats.Chapter2.Traits.AddRange(otherStats.Chapter2.Traits);
            newStats.Chapter3.Traits.AddRange(Chapter3.Traits);
            newStats.Chapter3.Traits.AddRange(otherStats.Chapter3.Traits);

            newStats.Chapter2.Interrupts.AddRange(Chapter2.Interrupts);
            newStats.Chapter2.Interrupts.AddRange(otherStats.Chapter2.Interrupts);
            newStats.Chapter3.Interrupts.AddRange(Chapter3.Interrupts);
            newStats.Chapter3.Interrupts.AddRange(otherStats.Chapter3.Interrupts);

            newStats.Chapter2.Actions.AddRange(Chapter2.Actions);
            newStats.Chapter2.Actions.AddRange(otherStats.Chapter2.Actions);
            newStats.Chapter3.Actions.AddRange(Chapter3.Actions);
            newStats.Chapter3.Actions.AddRange(otherStats.Chapter3.Actions);

            newStats.Chapter2.RemoveTraits.AddRange(Chapter2.RemoveTraits);
            newStats.Chapter2.RemoveTraits.AddRange(otherStats.Chapter2.RemoveTraits);
            newStats.Chapter3.RemoveTraits.AddRange(Chapter3.RemoveTraits);
            newStats.Chapter3.RemoveTraits.AddRange(otherStats.Chapter3.RemoveTraits);

            newStats.Chapter2.RemoveInterrupts.AddRange(Chapter2.RemoveInterrupts);
            newStats.Chapter2.RemoveInterrupts.AddRange(otherStats.Chapter2.RemoveInterrupts);
            newStats.Chapter3.RemoveInterrupts.AddRange(Chapter3.RemoveInterrupts);
            newStats.Chapter3.RemoveInterrupts.AddRange(otherStats.Chapter3.RemoveInterrupts);

            newStats.Chapter2.RemoveActions.AddRange(Chapter2.RemoveActions);
            newStats.Chapter2.RemoveActions.AddRange(otherStats.Chapter2.RemoveActions);
            newStats.Chapter3.RemoveActions.AddRange(Chapter3.RemoveActions);
            newStats.Chapter3.RemoveActions.AddRange(otherStats.Chapter3.RemoveActions);


            return newStats;
        }

        public static void InheritTraits(List<string> outputTraits, List<string> inheritedTraits, List<string> traitsToNotInherit)
        {
            foreach (string trait in inheritedTraits)
            {
                if (!traitsToNotInherit.Exists(x => x == trait))
                {
                    outputTraits.Add(trait);
                }
            }
        }

        public static void InheritInterrupts(List<Interrupt> outputInterrupts, List<Interrupt> inheritedInterrupts, List<string> interruptsToNotInherit)
        {
            foreach (Interrupt interrupt in inheritedInterrupts)
            {
                if (!interruptsToNotInherit.Exists(x => x == interrupt.Name))
                {
                    outputInterrupts.Add(interrupt);
                }
            }
        }

        public static void InheritActions(List<Action> outputActions, List<Action> inheritedActions, List<string> actionsToNotInherit)
        {
            foreach (Action action in inheritedActions)
            {
                if (!actionsToNotInherit.Exists(x => x == action.Name))
                {
                    outputActions.Add(action);
                }
            }
        }

        public static bool IsValid(Statistics stats)
        {
            return stats != null && stats.Name != null && stats.Name != "...";
        }

        public void ProcessChapter(int chapter)
        {
            if (chapter >= 2)
            {
                List<string> newTraits = new List<string>();
                newTraits.AddRange(Chapter2.Traits); 
                InheritTraits(newTraits, Traits, Chapter2.RemoveTraits);
                newTraits = newTraits.Distinct().ToList();
                Traits = newTraits;

                List<Interrupt> newInterrupts = new List<Interrupt>();
                newInterrupts.AddRange(Chapter2.Interrupts);
                InheritInterrupts(newInterrupts, Interrupts, Chapter2.RemoveInterrupts);
                Interrupts = newInterrupts;

                List<Action> newActions = new List<Action>();
                newActions.AddRange(Chapter2.Actions);
                InheritActions(newActions, Actions, Chapter2.RemoveActions);
                Actions = newActions;
            }
            if (chapter >= 3)
            {
                List<string> newTraits = new List<string>();
                newTraits.AddRange(Chapter3.Traits);
                InheritTraits(newTraits, Traits, Chapter3.RemoveTraits);
                newTraits = newTraits.Distinct().ToList();
                Traits = newTraits;

                List<Interrupt> newInterrupts = new List<Interrupt>();
                newInterrupts.AddRange(Chapter3.Interrupts);
                InheritInterrupts(newInterrupts, Interrupts, Chapter3.RemoveInterrupts);
                Interrupts = newInterrupts;

                List<Action> newActions = new List<Action>();
                newActions.AddRange(Chapter3.Actions);
                InheritActions(newActions, Actions, Chapter3.RemoveActions);
                Actions = newActions;
            }
        }

        public static List<Trait> BuildTraitList(List<string> traits, List<Trait> traitLib)
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

            // Remove duplicates
            for (int i = 0; i < updatedTraits.Count; ++i)
            {
                for (int j = i + 1; j < updatedTraits.Count;)
                {
                    if (updatedTraits[i].GetOriginalName() == updatedTraits[j].GetOriginalName())
                    {
                        updatedTraits.RemoveAt(j);
                    }
                    else
                    {
                        ++j;
                    }
                }
            }

            return updatedTraits;
        }

        public void ProcessTraits(List<Trait> traitLib)
        {
            mActualTraits = BuildTraitList(Traits, traitLib);

            // Get traits with specific values
            Trait defenseTrait = mActualTraits.Find(x => x.Defense.HasValue);
            if (defenseTrait != null)
            {
                Defense = defenseTrait.Defense.Value;
            }

            Trait dashMultiplierTrait = mActualTraits.Find(x => x.DashMultiplier.HasValue);
            if (dashMultiplierTrait != null)
            {
                mDashMultiplier = dashMultiplierTrait.DashMultiplier.Value;
            }

            Trait encounterBudgetTrait = mActualTraits.Find(x => x.EncounterBudget.HasValue);
            if (encounterBudgetTrait != null)
            {
                mEncounterBudget = encounterBudgetTrait.EncounterBudget.Value;
            }

            // Collect Actions
            foreach (Trait trait in mActualTraits.FindAll(x => x.Actions.Count > 0))
            {
                foreach (Action action in trait.Actions)
                {
                    Actions.Add(action);
                }

                if (String.IsNullOrEmpty(trait.Description))
                {
                    mActualTraits.Remove(trait);
                }
            }

            // Process phases and extra ability set traits
            foreach (Phase phase in Phases)
            {
                phase.ProcessTraits(traitLib);
            }
            foreach (AbilitySet abilitySet in ExtraAbilitySets)
            {
                abilitySet.ProcessTraits(traitLib);
            }
        }

        public List<Trait> GetActualTraits()
        {
            return mActualTraits;
        }

        public int GetDash()
        {
            return (int)Math.Ceiling((float)Speed.GetValueOrDefault(0) * mDashMultiplier);
        }

        public double GetEncounterBudget()
        {
            return mEncounterBudget;
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

        private string mOrigName;

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

        public string GetOriginalName()
        {
            if (String.IsNullOrEmpty(mOrigName))
            {
                return Name;
            }

            return mOrigName;
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

                newTrait.mOrigName = Name;
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
        public string AreaEffect { get; set; }
        public string Effect { get; set; }
        public string Mark { get; set; }
        public string Stance { get; set; }
        public string Collide { get; set; }
        public string Blightboost { get; set; }
        public string TerrainEffect { get; set; }
        public string SpecialInterrupt { get; set; }
        public string SpecialRecharge { get; set; }
        public string Delay { get; set; }
        public string DelayAreaEffect { get; set; }
        public SummonData Summon { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<RollData>))]
        public List<RollData> Rolls { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Combos { get; set; }

        public string PostAction { get; set; }

        public Action()
        {
            Tags = new List<string>();
            Combos = new List<Action>();
            Rolls = new List<RollData>();
        }
    }

    public class ChapterData
    {
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveInterrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Interrupt>))]
        public List<Interrupt> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveActions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }

        public ChapterData()
        {
            RemoveTraits = new List<string>();
            Traits = new List<string>();
            RemoveInterrupts = new List<string>();
            Interrupts = new List<Interrupt>();
            RemoveActions = new List<string>();
            Actions = new List<Action>();
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

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }

        private List<Trait> mActualTraits;

        public Phase()
        {
            Traits = new List<string>();
            Actions = new List<Action>();
        }

        public void ProcessTraits(List<Trait> traitLib)
        {
            mActualTraits = Statistics.BuildTraitList(Traits, traitLib);
        }

        public List<Trait> GetActualTraits()
        {
            return mActualTraits;
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

        private List<Trait> mActualTraits;

        public AbilitySet()
        {
            Traits = new List<string>();
            Actions = new List<Action>();
        }

        public void ProcessTraits(List<Trait> traitLib)
        {
            mActualTraits = Statistics.BuildTraitList(Traits, traitLib);
        }

        public List<Trait> GetActualTraits()
        {
            return mActualTraits;
        }
    }

    public class SummonData
    {
        public string Name { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Tags { get; set; }

        public string Effect { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<Action>))]
        public List<Action> Actions { get; set; }

        public SummonData()
        {
            Tags = new List<string>();
            Actions = new List<Action>();
        }

        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(Name);
        }
    }

    public class RollData
    {
        [JsonConverter(typeof(SingleOrArrayConverter<int>))]
        public List<int> Values { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public RollData()
        {
            Values = new List<int>();
        }
    }
}
