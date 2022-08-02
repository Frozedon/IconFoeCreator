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
        public string DisplayName { get; set; }
        public string Type { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Inherits { get; set; } 

        public int Chapter { get; set; }
        public bool UsesTemplate { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveSetupTraits { get; set; }

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
        public string HPText { get; set; }
        public int? HPMultiplier { get; set; }
        public bool? HPMultiplyByPlayers { get; set; }
        public int? Speed { get; set; }
        public float? DashMultiplier { get; set; }
        public int? Defense { get; set; }
        public int? FrayDamage { get; set; }
        public int? DamageDie { get; set; }
        public int? RechargeMin { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<BodyPartData>))]
        public List<BodyPartData> BodyParts { get; set; }

        public string PhasesDescription { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<PhaseData>))]
        public List<PhaseData> Phases { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<AbilitySetData>))]
        public List<AbilitySetData> ExtraAbilitySets { get; set; }

        public ChapterData Chapter2 { get; set; }
        public ChapterData Chapter3 { get; set; }


        // Additive Inheritance
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<TraitData>))]
        public List<TraitData> SetupTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Actions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<SummonData>))]
        public List<SummonData> Characters { get; set; }


        // Used by code
        private List<TraitData> mActualTraits;
        private float mDashMultiplier;
        private double mEncounterBudget;


        public Statistics()
        {
            Name = String.Empty;
            DisplayName = String.Empty;
            Type = String.Empty;
            Inherits = new List<string>();
            Chapter = 0;
            UsesTemplate = false;
            Faction = String.Empty;
            Class = String.Empty;
            SpecialClass = String.Empty;
            HPText = String.Empty;
            BodyParts = new List<BodyPartData>();
            PhasesDescription = String.Empty;
            Phases = new List<PhaseData>();
            ExtraAbilitySets = new List<AbilitySetData>();
            Chapter2 = new ChapterData();
            Chapter3 = new ChapterData();
            Traits = new List<string>();
            RemoveTraits = new List<string>();
            RemoveSetupTraits = new List<string>();
            RemoveInterrupts = new List<string>();
            RemoveActions = new List<string>();
            SetupTraits = new List<TraitData>();
            Interrupts = new List<InterruptData>();
            Actions = new List<ActionData>();
            Characters = new List<SummonData>();
            mActualTraits = new List<TraitData>();
            mDashMultiplier = 0.5f;
            mEncounterBudget = 1.0;
        }

        public override string ToString()
        {
            return Name;
        }

        public string GetDisplayName()
        {
            if (!String.IsNullOrEmpty(DisplayName))
            {
                return DisplayName;
            }

            return Name;
        }

        public Statistics InheritFrom(Statistics otherStats, bool saveRemoveLists = false)
        {
            Statistics newStats = new Statistics
            {
                Name = Name,
                DisplayName = DisplayName,
                Type = Type,
                Inherits = Inherits,
                Chapter = Chapter,
                UsesTemplate = UsesTemplate,
            };

            // Inherited values
            if (!String.IsNullOrEmpty(Faction)) { newStats.Faction = Faction; } else { newStats.Faction = otherStats.Faction; }
            if (!String.IsNullOrEmpty(Class)) { newStats.Class = Class; } else { newStats.Class = otherStats.Class; }
            if (!String.IsNullOrEmpty(SpecialClass)) { newStats.SpecialClass = SpecialClass; } else { newStats.SpecialClass = otherStats.SpecialClass; }
            if (Vitality.HasValue) { newStats.Vitality = Vitality; } else { newStats.Vitality = otherStats.Vitality; }
            if (HP.HasValue) { newStats.HP = HP; } else { newStats.HP = otherStats.HP; }
            if (!String.IsNullOrEmpty(HPText)) { newStats.HPText = HPText; } else { newStats.HPText = otherStats.HPText; }
            if (HPMultiplier.HasValue) { newStats.HPMultiplier = HPMultiplier; } else { newStats.HPMultiplier = otherStats.HPMultiplier; }
            if (HPMultiplyByPlayers.HasValue) { newStats.HPMultiplyByPlayers = HPMultiplyByPlayers; } else { newStats.HPMultiplyByPlayers = otherStats.HPMultiplyByPlayers; }
            if (Speed.HasValue) { newStats.Speed = Speed; } else { newStats.Speed = otherStats.Speed; }
            if (Defense.HasValue) { newStats.Defense = Defense; } else { newStats.Defense = otherStats.Defense; }
            if (FrayDamage.HasValue) { newStats.FrayDamage = FrayDamage; } else { newStats.FrayDamage = otherStats.FrayDamage; }
            if (DamageDie.HasValue) { newStats.DamageDie = DamageDie; } else { newStats.DamageDie = otherStats.DamageDie; }
            if (RechargeMin.HasValue) { newStats.RechargeMin = RechargeMin; } else { newStats.RechargeMin = otherStats.RechargeMin; }
            if (BodyParts.Count > 0) { newStats.BodyParts = BodyParts; } else { newStats.BodyParts = otherStats.BodyParts; }
            if (!String.IsNullOrEmpty(PhasesDescription)) { newStats.PhasesDescription = PhasesDescription; } else { newStats.PhasesDescription = otherStats.PhasesDescription; }
            if (Phases.Count() > 0) { newStats.Phases = Phases; } else { newStats.Phases = otherStats.Phases; }
            if (ExtraAbilitySets.Count() > 0) { newStats.ExtraAbilitySets = ExtraAbilitySets; } else { newStats.ExtraAbilitySets = otherStats.ExtraAbilitySets; }

            // Only add traits, interrupts, and actions from the inherited that are not on the current removal lists
            newStats.Traits.AddRange(Traits);
            InheritTraits(newStats.Traits, otherStats.Traits, RemoveTraits);
            newStats.Traits = newStats.Traits.Distinct().ToList();

            newStats.SetupTraits.AddRange(SetupTraits);
            InheritSetupTraits(newStats.SetupTraits, otherStats.SetupTraits, RemoveSetupTraits);

            newStats.Interrupts.AddRange(Interrupts);
            InheritInterrupts(newStats.Interrupts, otherStats.Interrupts, RemoveInterrupts);

            newStats.Actions.AddRange(Actions);
            InheritActions(newStats.Actions, otherStats.Actions, RemoveActions);

            if (saveRemoveLists)
            {
                newStats.RemoveTraits.AddRange(RemoveTraits);
                newStats.RemoveSetupTraits.AddRange(RemoveSetupTraits);
                newStats.RemoveInterrupts.AddRange(RemoveInterrupts);
                newStats.RemoveActions.AddRange(RemoveActions);
            }

            // Keep all characters
            newStats.Characters.AddRange(Characters);
            newStats.Characters.AddRange(otherStats.Characters);

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

        public static void InheritSetupTraits(List<TraitData> outputTraits, List<TraitData> inheritedTraits, List<string> traitsToNotInherit)
        {
            foreach (TraitData trait in inheritedTraits)
            {
                if (!traitsToNotInherit.Exists(x => x == trait.Name))
                {
                    outputTraits.Add(trait);
                }
            }
        }

        public static void InheritInterrupts(List<InterruptData> outputInterrupts, List<InterruptData> inheritedInterrupts, List<string> interruptsToNotInherit)
        {
            foreach (InterruptData interrupt in inheritedInterrupts)
            {
                if (!interruptsToNotInherit.Exists(x => x == interrupt.Name))
                {
                    outputInterrupts.Add(interrupt);
                }
            }
        }

        public static void InheritActions(List<ActionData> outputActions, List<ActionData> inheritedActions, List<string> actionsToNotInherit)
        {
            foreach (ActionData action in inheritedActions)
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

        public List<TraitData> GetActualTraits()
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

        public void ProcessData(List<TraitData> traitLib, int chapter)
        {
            // Inherit chapter data
            InheritChapterData(chapter);

            // Build trait list from lib
            mActualTraits = BuildTraitList(Traits, traitLib);

            // Get traits with specific values
            TraitData defenseTrait = mActualTraits.Find(x => x.Defense.HasValue);
            if (defenseTrait != null)
            {
                Defense = defenseTrait.Defense.Value;
            }

            TraitData dashMultiplierTrait = mActualTraits.Find(x => x.DashMultiplier.HasValue);
            if (dashMultiplierTrait != null)
            {
                mDashMultiplier = dashMultiplierTrait.DashMultiplier.Value;
            }

            TraitData encounterBudgetTrait = mActualTraits.Find(x => x.EncounterBudget.HasValue);
            if (encounterBudgetTrait != null)
            {
                mEncounterBudget = encounterBudgetTrait.EncounterBudget.Value;
            }

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Actions.Count > 0))
            {
                foreach (ActionData action in trait.Actions)
                {
                    Actions.Add(action);
                }

                if (String.IsNullOrEmpty(trait.Description) && trait.Summons.Count == 0)
                {
                    mActualTraits.Remove(trait);
                }
            }

            // Get recharge minimum
            int rechargeMin = RechargeMin.GetValueOrDefault(0);
            if (rechargeMin > 0)
            {
                foreach (ActionData action in Actions)
                {
                    if (action.Recharge > 0 && action.Recharge < RechargeMin.Value)
                    {
                        action.Recharge = RechargeMin.Value;
                    }
                }
            }

            // Process data on all the variables
            foreach (ActionData action in Actions)
            {
                action.ProcessData(traitLib, rechargeMin);
            }
            foreach (InterruptData interrupt in Interrupts)
            {
                interrupt.ProcessData(traitLib, rechargeMin);
            }
            foreach (TraitData trait in mActualTraits)
            {
                trait.ProcessData(traitLib, rechargeMin);
            }
            foreach (SummonData character in Characters)
            {
                character.ProcessData(traitLib, rechargeMin);
            }
            foreach (PhaseData phase in Phases)
            {
                phase.ProcessData(traitLib, rechargeMin);
            }
            foreach (AbilitySetData abilitySet in ExtraAbilitySets)
            {
                abilitySet.ProcessData(traitLib, rechargeMin);
            }
        }

        public void InheritChapterData(int chapter)
        {
            if (chapter >= 2)
            {
                List<string> newTraits = new List<string>();
                newTraits.AddRange(Chapter2.Traits); 
                InheritTraits(newTraits, Traits, Chapter2.RemoveTraits);
                newTraits = newTraits.Distinct().ToList();
                Traits = newTraits;

                List<InterruptData> newInterrupts = new List<InterruptData>();
                newInterrupts.AddRange(Chapter2.Interrupts);
                InheritInterrupts(newInterrupts, Interrupts, Chapter2.RemoveInterrupts);
                Interrupts = newInterrupts;

                List<ActionData> newActions = new List<ActionData>();
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

                List<InterruptData> newInterrupts = new List<InterruptData>();
                newInterrupts.AddRange(Chapter3.Interrupts);
                InheritInterrupts(newInterrupts, Interrupts, Chapter3.RemoveInterrupts);
                Interrupts = newInterrupts;

                List<ActionData> newActions = new List<ActionData>();
                newActions.AddRange(Chapter3.Actions);
                InheritActions(newActions, Actions, Chapter3.RemoveActions);
                Actions = newActions;
            }
        }

        public static List<TraitData> BuildTraitList(List<string> traits, List<TraitData> traitLib)
        {
            List<TraitData> updatedTraits = new List<TraitData>();

            foreach (string traitName in traits)
            {
                TraitData traitFound = traitLib.Find(x => x.Matches(traitName));
                if (traitFound != null)
                {
                    updatedTraits.Add(traitFound.MakeExpressedTrait(traitName));
                }
                else
                {
                    updatedTraits.Add(new TraitData()
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
                    if (updatedTraits[i].Name == updatedTraits[j].Name)
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
    }

    public class TraitData
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Tags { get; set; }

        public string Description { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> ExtraItems { get; set; }

        public float? DashMultiplier { get; set; }
        public int? Defense { get; set; }
        public double? EncounterBudget { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Actions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<RollData>))]
        public List<RollData> Rolls { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<SummonData>))]
        public List<SummonData> Summons { get; set; }

        public TraitData()
        {
            Tags = new List<string>();
            ExtraItems = new List<ItemData>();
            Actions = new List<ActionData>();
            Interrupts = new List<InterruptData>();
            Rolls = new List<RollData>();
            Summons = new List<SummonData>();
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

        public string GetDisplayName()
        {
            if (!String.IsNullOrEmpty(DisplayName))
            {
                return DisplayName;
            }

            return Name;
        }

        public TraitData MakeExpressedTrait(string traitName)
        {
            if (Name.Contains(VALUE_TOKEN))
            {
                TraitData newTrait = new TraitData()
                {
                    Name = Name,
                    DisplayName = DisplayName,
                    Description = Description,
                    DashMultiplier = DashMultiplier,
                    Defense = Defense,
                    EncounterBudget = EncounterBudget,
                    Actions = Actions,
                    Summons = Summons
                };

                string strippedTraitName = (string)traitName.Clone();
                string[] substrings = Name.Split(VALUE_TOKEN.ToCharArray());
                if (substrings.Length > 0)
                {
                    strippedTraitName = strippedTraitName.Replace(substrings[0], "");
                }
                string valueStr = Regex.Match(strippedTraitName, @"\d+", RegexOptions.IgnoreCase).Value;

                if (!String.IsNullOrEmpty(newTrait.DisplayName))
                {
                    newTrait.DisplayName = DisplayName.Replace(VALUE_TOKEN, valueStr);
                }
                else
                {
                    newTrait.DisplayName = Name.Replace(VALUE_TOKEN, valueStr);
                }

                newTrait.Description = Description.Replace(VALUE_TOKEN, valueStr);

                return newTrait;
            }
            else
            {
                return this;
            }
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            foreach (ActionData action in Actions)
            {
                action.ProcessData(traitLib, rechargeMin);
            }
            foreach (InterruptData interrupt in Interrupts)
            {
                interrupt.ProcessData(traitLib, rechargeMin);
            }
            foreach (SummonData summon in Summons)
            {
                summon.ProcessData(traitLib, rechargeMin);
            }
        }
    }

    public class InterruptData
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public int Recharge { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Tags { get; set; }

        public string Description { get; set; }
        public string Trigger { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Effects { get; set; }

        public string Collide { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<SummonData>))]
        public List<SummonData> Summons { get; set; }

        public InterruptData()
        {
            Tags = new List<string>();
            Effects = new List<string>();
            Summons = new List<SummonData>();
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            foreach (SummonData summon in Summons)
            {
                summon.ProcessData(traitLib, rechargeMin);
            }
        }
    }

    public class ActionData
    {
        public string Name { get; set; }
        public int ActionCost { get; set; }
        public int Recharge { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Tags { get; set; }

        public string Description { get; set; }
        public string Hit { get; set; }
        public string AutoHit { get; set; }
        public string CriticalHit { get; set; }
        public string Miss { get; set; }
        public string AreaEffect { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Effects { get; set; }

        public string Mark { get; set; }
        public string Stance { get; set; }
        public string Collide { get; set; }
        public string Blightboost { get; set; }
        public string TerrainEffect { get; set; }
        public string SpecialInterrupt { get; set; }
        public string SpecialRecharge { get; set; }
        public string Delay { get; set; }
        public string PostAreaEffect { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ComponentData>))]
        public List<ComponentData> CustomComponents { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> ExtraItems { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<RollData>))]
        public List<RollData> Rolls { get; set; }
        
        [JsonConverter(typeof(SingleOrArrayConverter<SummonData>))]
        public List<SummonData> Summons { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Combos { get; set; }

        public string PostAction { get; set; }

        public ActionData()
        {
            ActionCost = -1;

            Tags = new List<string>();
            Effects = new List<string>();
            CustomComponents = new List<ComponentData>();
            ExtraItems = new List<ItemData>();
            Rolls = new List<RollData>();
            Summons = new List<SummonData>();
            Combos = new List<ActionData>();
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            foreach (SummonData summon in Summons)
            {
                summon.ProcessData(traitLib, rechargeMin);
            }
            foreach (ActionData action in Combos)
            {
                action.ProcessData(traitLib, rechargeMin);
            }

            if (Recharge > 0 && Recharge < rechargeMin)
            {
                Recharge = rechargeMin;
            }
        }
    }

    public class ComponentData
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ChapterData
    {
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveInterrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveActions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Actions { get; set; }

        public ChapterData()
        {
            RemoveTraits = new List<string>();
            Traits = new List<string>();
            RemoveInterrupts = new List<string>();
            Interrupts = new List<InterruptData>();
            RemoveActions = new List<string>();
            Actions = new List<ActionData>();
        }
    }

    public class BodyPartData
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public bool HPMultiplyByPlayers { get; set; }
        public string Description { get; set; }
    }

    public class PhaseData
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Actions { get; set; }

        private List<TraitData> mActualTraits;

        public PhaseData()
        {
            Traits = new List<string>();
            Interrupts = new List<InterruptData>();
            Actions = new List<ActionData>();
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            mActualTraits = Statistics.BuildTraitList(Traits, traitLib);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Actions.Count > 0))
            {
                foreach (ActionData action in trait.Actions)
                {
                    Actions.Add(action);
                }

                if (String.IsNullOrEmpty(trait.Description) && trait.Summons.Count == 0)
                {
                    mActualTraits.Remove(trait);
                }
            }

            // Continue to process data
            foreach (TraitData trait in mActualTraits)
            {
                trait.ProcessData(traitLib, rechargeMin);
            }
            foreach (InterruptData interrupt in Interrupts)
            {
                interrupt.ProcessData(traitLib, rechargeMin);
            }
            foreach (ActionData action in Actions)
            {
                action.ProcessData(traitLib, rechargeMin);
            }
        }

        public List<TraitData> GetActualTraits()
        {
            return mActualTraits;
        }
    }

    public class AbilitySetData
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Actions { get; set; }

        private List<TraitData> mActualTraits;

        public AbilitySetData()
        {
            Traits = new List<string>();
            Interrupts = new List<InterruptData>();
            Actions = new List<ActionData>();
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            mActualTraits = Statistics.BuildTraitList(Traits, traitLib);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Actions.Count > 0))
            {
                foreach (ActionData action in trait.Actions)
                {
                    Actions.Add(action);
                }

                if (String.IsNullOrEmpty(trait.Description) && trait.Summons.Count == 0)
                {
                    mActualTraits.Remove(trait);
                }
            }

            // Continue to process data
            foreach (TraitData trait in mActualTraits)
            {
                trait.ProcessData(traitLib, rechargeMin);
            }
            foreach (InterruptData interrupt in Interrupts)
            {
                interrupt.ProcessData(traitLib, rechargeMin);
            }
            foreach (ActionData action in Actions)
            {
                action.ProcessData(traitLib, rechargeMin);
            }
        }

        public List<TraitData> GetActualTraits()
        {
            return mActualTraits;
        }
    }

    public class SummonData
    {
        public string Name { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Tags { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Effects { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Actions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> ComplexActions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> SpecialActions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> SpecialInterrupts { get; set; }


        private List<TraitData> mActualTraits;

        public SummonData()
        {
            Tags = new List<string>();
            Traits = new List<string>();
            Effects = new List<string>();
            Actions = new List<string>();
            ComplexActions = new List<ActionData>();
            SpecialActions = new List<ActionData>();
            SpecialInterrupts = new List<InterruptData>();
            mActualTraits = new List<TraitData>();
        }

        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(Name);
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            mActualTraits = Statistics.BuildTraitList(Traits, traitLib);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Actions.Count > 0))
            {
                foreach (ActionData action in trait.Actions)
                {
                    ComplexActions.Add(action);
                }

                if (String.IsNullOrEmpty(trait.Description) && trait.Summons.Count == 0)
                {
                    mActualTraits.Remove(trait);
                }
            }

            // Continue to process data
            foreach (TraitData trait in mActualTraits)
            {
                trait.ProcessData(traitLib, rechargeMin);
            }
            foreach (ActionData action in ComplexActions)
            {
                action.ProcessData(traitLib, rechargeMin);
            }
            foreach (ActionData action in SpecialActions)
            {
                action.ProcessData(traitLib, rechargeMin);
            }
        }

        public List<TraitData> GetActualTraits()
        {
            return mActualTraits;
        }
    }

    public class RollData
    {
        [JsonConverter(typeof(SingleOrArrayConverter<int>))]
        public List<int> Values { get; set; }

        public bool Plus { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public RollData()
        {
            Values = new List<int>();
        }
    }

    public class ItemData
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
