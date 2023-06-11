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

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveUsesSpecialTemplates { get; set; }


        // Normal Inheritance
        public string Faction { get; set; }
        public string Class { get; set; }
        public string SpecialClass { get; set; }
        public int? Vitality { get; set; }
        public int? HP { get; set; }
        public string HPText { get; set; }
        public int? HPMultiplier { get; set; }
        public bool? HPMultiplyByPlayers { get; set; }
        public int? Hits { get; set; }
        public int? Speed { get; set; }
        public float? DashMultiplier { get; set; }
        public int? Defense { get; set; }
        public int? FrayDamage { get; set; }
        public int? DamageDie { get; set; }
        public int? RechargeMin { get; set; }
        public int? MembersPerPlayer { get; set; }
        public string Tactics { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<BodyPartData>))]
        public List<BodyPartData> BodyParts { get; set; }

        public string PhasesDescription { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<PhaseData>))]
        public List<PhaseData> Phases { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<AbilitySetData>))]
        public List<AbilitySetData> ExtraAbilitySets { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> UsesSpecialTemplates { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ConditionalAbilityData>))]
        public List<ConditionalAbilityData> ConditionalAbilities { get; set; }


        // Additive Inheritance
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<TraitData>))]
        public List<TraitData> SetupTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Actions { get; set; }


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
            Tactics = String.Empty;
            ExtraAbilitySets = new List<AbilitySetData>();
            ConditionalAbilities = new List<ConditionalAbilityData>();
            UsesSpecialTemplates = new List<string>();
            Traits = new List<string>();
            RemoveTraits = new List<string>();
            RemoveSetupTraits = new List<string>();
            RemoveInterrupts = new List<string>();
            RemoveActions = new List<string>();
            RemoveUsesSpecialTemplates = new List<string>();
            SetupTraits = new List<TraitData>();
            Interrupts = new List<InterruptData>();
            Actions = new List<ActionData>();
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
            if (Hits.HasValue) { newStats.Hits = Hits; } else { newStats.Hits = otherStats.Hits; }
            if (Speed.HasValue) { newStats.Speed = Speed; } else { newStats.Speed = otherStats.Speed; }
            if (Defense.HasValue) { newStats.Defense = Defense; } else { newStats.Defense = otherStats.Defense; }
            if (FrayDamage.HasValue) { newStats.FrayDamage = FrayDamage; } else { newStats.FrayDamage = otherStats.FrayDamage; }
            if (DamageDie.HasValue) { newStats.DamageDie = DamageDie; } else { newStats.DamageDie = otherStats.DamageDie; }
            if (RechargeMin.HasValue) { newStats.RechargeMin = RechargeMin; } else { newStats.RechargeMin = otherStats.RechargeMin; }
            if (MembersPerPlayer.HasValue) { newStats.MembersPerPlayer = MembersPerPlayer; } else { newStats.MembersPerPlayer = otherStats.MembersPerPlayer; }
            if (BodyParts.Count > 0) { newStats.BodyParts = BodyParts; } else { newStats.BodyParts = otherStats.BodyParts; }
            if (!String.IsNullOrEmpty(PhasesDescription)) { newStats.PhasesDescription = PhasesDescription; } else { newStats.PhasesDescription = otherStats.PhasesDescription; }
            if (Phases.Count() > 0) { newStats.Phases = Phases; } else { newStats.Phases = otherStats.Phases; }
            if (!String.IsNullOrEmpty(Tactics)) { newStats.Tactics = Tactics; } else { newStats.Tactics = otherStats.Tactics; }
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

            newStats.UsesSpecialTemplates.AddRange(UsesSpecialTemplates);
            InheritUsesSpecialTemplates(newStats.UsesSpecialTemplates, otherStats.UsesSpecialTemplates, RemoveUsesSpecialTemplates);

            InheritConditionalAbilities(newStats.ConditionalAbilities, ConditionalAbilities, otherStats.ConditionalAbilities, RemoveTraits, RemoveSetupTraits, RemoveInterrupts, RemoveActions);

            if (saveRemoveLists)
            {
                newStats.RemoveTraits.AddRange(RemoveTraits);
                newStats.RemoveSetupTraits.AddRange(RemoveSetupTraits);
                newStats.RemoveInterrupts.AddRange(RemoveInterrupts);
                newStats.RemoveActions.AddRange(RemoveActions); ;
                newStats.RemoveUsesSpecialTemplates.AddRange(RemoveUsesSpecialTemplates);
            }

            return newStats;
        }

        public static void InheritConditionalAbilities(List<ConditionalAbilityData> outputAbilities, List<ConditionalAbilityData> thisAbilities, List<ConditionalAbilityData> otherAbilities, List<string> traitsToNotInherit, List<string> setupTraitsToNotInherit, List<string> interruptsToNotInherit, List<string> actionsToNotInherit)
        {
            foreach (ConditionalAbilityData otherData in otherAbilities)
            {
                ConditionalAbilityData newData = new ConditionalAbilityData()
                { 
                    Chapter = otherData.Chapter,
                    IsSpecialClasses = otherData.IsSpecialClasses,
                    IsNotSpecialClasses = otherData.IsNotSpecialClasses,
                    RemoveTraits = otherData.RemoveTraits,
                    RemoveSetupTraits = otherData.RemoveSetupTraits,
                    RemoveInterrupts = otherData.RemoveInterrupts,
                    RemoveActions = otherData.RemoveActions,
                    UsesSpecialTemplates = otherData.UsesSpecialTemplates,
                    HPMultiplier = otherData.HPMultiplier,
                    SpecialClass = otherData.SpecialClass
                };

                InheritTraits(newData.Traits, otherData.Traits, traitsToNotInherit);
                InheritSetupTraits(newData.SetupTraits, otherData.SetupTraits, setupTraitsToNotInherit);
                InheritInterrupts(newData.Interrupts, otherData.Interrupts, interruptsToNotInherit);
                InheritActions(newData.Actions, otherData.Actions, actionsToNotInherit);

                outputAbilities.Add(newData);
            }

            outputAbilities.AddRange(thisAbilities);
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

        public static void InheritUsesSpecialTemplates(List<string> outputTemplates, List<string> inheritedTemplates, List<string> templatesToNotInherit)
        {
            foreach (string trait in inheritedTemplates)
            {
                if (!templatesToNotInherit.Exists(x => x == trait))
                {
                    outputTemplates.Add(trait);
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

        public string GetClass()
        {
            if (!String.IsNullOrEmpty(Class))
            {
                return Class;
            }

            return GetSpecialClass();
        }

        public string GetSpecialClass()
        {
            return String.IsNullOrEmpty(SpecialClass) ? "Normal" : SpecialClass;
        }

        public List<ActionData> GetActions()
        {
            List<ActionData> actionList = new List<ActionData>();
            actionList.AddRange(Actions);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Actions.Count > 0))
            {
                foreach (ActionData action in trait.Actions)
                {
                    actionList.Add(action);
                }
            }

            return actionList;
        }

        public List<InterruptData> GetInterrupts()
        {
            List<InterruptData> interruptList = new List<InterruptData>();
            interruptList.AddRange(Interrupts);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Interrupts.Count > 0))
            {
                foreach (InterruptData interrupt in trait.Interrupts)
                {
                    interruptList.Add(interrupt);
                }
            }

            return interruptList;
        }

        public void ProcessData(List<TraitData> traitLib, int chapter)
        {
            // Inherit chapter data
            ProcessConditionalAbilities(chapter, GetSpecialClass());

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

            List<TraitData> encounterBudgetAddTraits = mActualTraits.FindAll(x => x.EncounterBudgetAdd.HasValue);
            foreach (TraitData trait in encounterBudgetAddTraits)
            {
                mEncounterBudget += trait.EncounterBudgetAdd.Value;
            }

            // Get recharge minimum
            int rechargeMin = RechargeMin.GetValueOrDefault(0);

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
            foreach (PhaseData phase in Phases)
            {
                phase.ProcessData(traitLib, rechargeMin);
            }
            foreach (AbilitySetData abilitySet in ExtraAbilitySets)
            {
                abilitySet.ProcessData(traitLib, rechargeMin);
            }
        }

        public void ProcessConditionalAbilities(int chapter, string specialClass)
        {
            foreach (ConditionalAbilityData abilityData in ConditionalAbilities)
            {
                if (abilityData.Chapter <= chapter 
                    && (abilityData.IsSpecialClasses.Count == 0 || abilityData.IsSpecialClasses.Find(x => x.ToLower() == specialClass.ToLower()) != null)
                    && (abilityData.IsNotSpecialClasses.Count == 0 || abilityData.IsNotSpecialClasses.Find(x => x.ToLower() == specialClass.ToLower()) == null))
                {
                    List<string> newTraits = new List<string>();
                    newTraits.AddRange(abilityData.Traits);
                    InheritTraits(newTraits, Traits, abilityData.RemoveTraits);
                    Traits = newTraits.Distinct().ToList();

                    List<TraitData> newSetupTraits = new List<TraitData>();
                    newSetupTraits.AddRange(abilityData.SetupTraits);
                    InheritSetupTraits(newSetupTraits, SetupTraits, abilityData.RemoveSetupTraits);
                    SetupTraits = newSetupTraits;

                    List<InterruptData> newInterrupts = new List<InterruptData>();
                    newInterrupts.AddRange(abilityData.Interrupts);
                    InheritInterrupts(newInterrupts, Interrupts, abilityData.RemoveInterrupts);
                    Interrupts = newInterrupts;

                    List<ActionData> newActions = new List<ActionData>();
                    newActions.AddRange(abilityData.Actions);
                    InheritActions(newActions, Actions, abilityData.RemoveActions);
                    Actions = newActions;

                    UsesSpecialTemplates.AddRange(abilityData.UsesSpecialTemplates);

                    if (!String.IsNullOrEmpty(abilityData.SpecialClass)) { SpecialClass = abilityData.SpecialClass; }
                    if (abilityData.HPMultiplier.HasValue) { HPMultiplier = abilityData.HPMultiplier; }
                }
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
                    if (updatedTraits[i].Name.ToLower() == updatedTraits[j].Name.ToLower())
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
        public List<ItemData> CustomComponents { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> ListedItems { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Actions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> ExtraActions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<RollData>))]
        public List<RollData> Rolls { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<SummonData>))]
        public List<SummonData> Summons { get; set; }

        // Statistic adjustments
        public float? DashMultiplier { get; set; }
        public int? Defense { get; set; }
        public double? EncounterBudget { get; set; }
        public double? EncounterBudgetAdd { get; set; }

        public TraitData()
        {
            Tags = new List<string>();
            CustomComponents = new List<ItemData>();
            ListedItems = new List<ItemData>();
            Actions = new List<ActionData>();
            ExtraActions = new List<ActionData>();
            Interrupts = new List<InterruptData>();
            Rolls = new List<RollData>();
            Summons = new List<SummonData>();
        }

        private static readonly string VALUE_TOKEN = "[X]";

        public bool ShouldDisplay()
        {
            if (Actions.Count > 0 || Interrupts.Count > 0)
            {
                return Tags.Count > 0
                    || !String.IsNullOrEmpty(Description)
                    || CustomComponents.Count > 0
                    || ListedItems.Count > 0
                    || ExtraActions.Count > 0
                    || Rolls.Count > 0
                    || Summons.Count > 0;
            }

            return true;
        }

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
                    EncounterBudgetAdd = EncounterBudgetAdd,
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

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> ListedItems { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<SummonData>))]
        public List<SummonData> Summons { get; set; }

        public InterruptData()
        {
            Tags = new List<string>();
            Effects = new List<string>();
            ListedItems = new List<ItemData>();
            Summons = new List<SummonData>();
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            foreach (SummonData summon in Summons)
            {
                summon.ProcessData(traitLib, rechargeMin);
            }

            if (Recharge > 0 && Recharge < rechargeMin)
            {
                Recharge = rechargeMin;
            }
        }
    }

    public class ActionData
    {
        public string Name { get; set; }
        public int ActionCost { get; set; }
        public bool RoundAction { get; set; }
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
        public List<string> PreEffects { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Effects { get; set; }

        public string Mark { get; set; }
        public string Stance { get; set; }
        public string Collide { get; set; }
        public string Blightboost { get; set; }
        public string TerrainEffect { get; set; }
        public string SpecialInterrupt { get; set; }
        public string SpecialRecharge { get; set; }
        public string Charge { get; set; }
        public string Delay { get; set; }
        public string PostAreaEffect { get; set; }
        public string PostCollide { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> CustomComponents { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> ListedItems { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<RollData>))]
        public List<RollData> Rolls { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> ExtraActions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<SummonData>))]
        public List<SummonData> Summons { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        public ActionData Combo { get; set; }

        public string PostAction { get; set; }

        public ActionData()
        {
            ActionCost = -1;
            RoundAction = false;

            Tags = new List<string>();
            PreEffects = new List<string>();
            Effects = new List<string>();
            CustomComponents = new List<ItemData>();
            ListedItems = new List<ItemData>();
            Rolls = new List<RollData>();
            ExtraActions = new List<ActionData>();
            Interrupts = new List<InterruptData>();
            Summons = new List<SummonData>();
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            foreach (SummonData summon in Summons)
            {
                summon.ProcessData(traitLib, rechargeMin);
            }
            foreach (ActionData action in ExtraActions)
            {
                action.ProcessData(traitLib, rechargeMin);
            }
            foreach (InterruptData interrupt in Interrupts)
            {
                interrupt.ProcessData(traitLib, rechargeMin);
            }

            if (Combo != null)
            {
                Combo.ProcessData(traitLib, rechargeMin);
            }

            if (Recharge > 0 && Recharge < rechargeMin)
            {
                Recharge = rechargeMin;
            }
        }
    }

    public class ConditionalAbilityData
    {
        // Conditionals
        public int Chapter { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> IsSpecialClasses { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> IsNotSpecialClasses { get; set; }

        // Data
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

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> RemoveSetupTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<TraitData>))]
        public List<TraitData> SetupTraits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> UsesSpecialTemplates { get; set; }

        public string SpecialClass { get; set; }
        public int? HPMultiplier { get; set; }


        public ConditionalAbilityData()
        {
            IsSpecialClasses = new List<string>();
            IsNotSpecialClasses = new List<string>();
            RemoveTraits = new List<string>();
            Traits = new List<string>();
            RemoveInterrupts = new List<string>();
            Interrupts = new List<InterruptData>();
            RemoveActions = new List<string>();
            Actions = new List<ActionData>();
            RemoveSetupTraits = new List<string>();
            SetupTraits = new List<TraitData>();
            UsesSpecialTemplates = new List<string>();
            SpecialClass = String.Empty;
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

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> ListedItems { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Actions { get; set; }

        private List<TraitData> mActualTraits;

        public PhaseData()
        {
            ListedItems = new List<ItemData>();
            Traits = new List<string>();
            Interrupts = new List<InterruptData>();
            Actions = new List<ActionData>();
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            mActualTraits = Statistics.BuildTraitList(Traits, traitLib);

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

        public List<ActionData> GetActions()
        {
            List<ActionData> actionList = new List<ActionData>();
            actionList.AddRange(Actions);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Actions.Count > 0))
            {
                foreach (ActionData action in trait.Actions)
                {
                    actionList.Add(action);
                }
            }

            return actionList;
        }

        public List<InterruptData> GetInterrupts()
        {
            List<InterruptData> interruptList = new List<InterruptData>();
            interruptList.AddRange(Interrupts);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Interrupts.Count > 0))
            {
                foreach (InterruptData interrupt in trait.Interrupts)
                {
                    interruptList.Add(interrupt);
                }
            }

            return interruptList;
        }
    }

    public class AbilitySetData
    {
        public string Name { get; set; }
        public string Class { get; set; }
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

        public List<ActionData> GetActions()
        {
            List<ActionData> actionList = new List<ActionData>();
            actionList.AddRange(Actions);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Actions.Count > 0))
            {
                foreach (ActionData action in trait.Actions)
                {
                    actionList.Add(action);
                }
            }

            return actionList;
        }

        public List<InterruptData> GetInterrupts()
        {
            List<InterruptData> interruptList = new List<InterruptData>();
            interruptList.AddRange(Interrupts);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Interrupts.Count > 0))
            {
                foreach (InterruptData interrupt in trait.Interrupts)
                {
                    interruptList.Add(interrupt);
                }
            }

            return interruptList;
        }
    }

    public class SummonData
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public bool IsObject { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Tags { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Traits { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> SummonEffects { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> SummonActions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> Actions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> Interrupts { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> ListedItems { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ActionData>))]
        public List<ActionData> ListedActions { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<InterruptData>))]
        public List<InterruptData> ListedInterrupts { get; set; }


        private List<TraitData> mActualTraits;

        public SummonData()
        {
            Name = String.Empty;
            Class = String.Empty;
            Description = String.Empty;
            Tags = new List<string>();
            Traits = new List<string>();
            SummonEffects = new List<string>();
            SummonActions = new List<string>();
            Actions = new List<ActionData>();
            Interrupts = new List<InterruptData>();
            ListedItems = new List<ItemData>();
            ListedActions = new List<ActionData>();
            ListedInterrupts = new List<InterruptData>();
            mActualTraits = new List<TraitData>();
        }

        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(Name);
        }

        public void ProcessData(List<TraitData> traitLib, int rechargeMin)
        {
            mActualTraits = Statistics.BuildTraitList(Traits, traitLib);

            // Continue to process data
            foreach (TraitData trait in mActualTraits)
            {
                trait.ProcessData(traitLib, rechargeMin);
            }
            foreach (ActionData action in Actions)
            {
                action.ProcessData(traitLib, rechargeMin);
            }
            foreach (ActionData action in ListedActions)
            {
                action.ProcessData(traitLib, rechargeMin);
            }
        }

        public List<TraitData> GetActualTraits()
        {
            return mActualTraits;
        }

        public List<ActionData> GetActions()
        {
            List<ActionData> actionList = new List<ActionData>();
            actionList.AddRange(Actions);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Actions.Count > 0))
            {
                foreach (ActionData action in trait.Actions)
                {
                    actionList.Add(action);
                }
            }

            return actionList;
        }

        public List<InterruptData> GetInterrupts()
        {
            List<InterruptData> interruptList = new List<InterruptData>();
            interruptList.AddRange(Interrupts);

            // Collect Actions from Traits
            foreach (TraitData trait in mActualTraits.FindAll(x => x.Interrupts.Count > 0))
            {
                foreach (InterruptData interrupt in trait.Interrupts)
                {
                    interruptList.Add(interrupt);
                }
            }

            return interruptList;
        }
    }

    public class RollData
    {
        [JsonConverter(typeof(SingleOrArrayConverter<int>))]
        public List<int> Values { get; set; }

        public bool Plus { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> CustomComponents { get; set; }

        public RollData()
        {
            Values = new List<int>();
            CustomComponents = new List<ItemData>();
        }
    }

    public class ItemData
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<ItemData>))]
        public List<ItemData> CustomComponents { get; set; }

        public ItemData()
        {
            CustomComponents = new List<ItemData>();
        }
    }
}
