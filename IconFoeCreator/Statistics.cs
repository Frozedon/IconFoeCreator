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
        public OptionalInt[] Health { get; set; }
        public OptionalInt HPMultiplier { get; set; }
        public OptionalInt Speed { get; set; }
        public OptionalInt Run { get; set; }
        public OptionalInt Dash { get; set; }
        public OptionalInt Defense { get; set; }
        public OptionalInt[] Armor { get; set; }
        public OptionalInt[] Attack { get; set; }
        public OptionalInt[] FrayDamage { get; set; }
        public OptionalInt LightDamage { get; set; }
        public OptionalInt HeavyDamage { get; set; }
        public OptionalInt CriticalDamage { get; set; }
        public OptionalString LightDamageDie { get; set; }
        public OptionalString HeavyDamageDie { get; set; }
        public OptionalString CriticalDamageDie { get; set; }
        public OptionalString DamageType { get; set; }
        public List<Trait> Traits { get; set; }
        public List<Interrupt> Interrupts { get; set; }
        public List<Action> Actions { get; set; }
        public OptionalString FactionBlight { get; set; }

        public Statistics()
        {
            Health = new OptionalInt[Constants.ChapterCount];
            HPMultiplier = new OptionalInt();
            HPMultiplier.SetDefault(4);
            Speed = new OptionalInt();
            Run = new OptionalInt();
            Dash = new OptionalInt();
            Defense = new OptionalInt();
            Armor = new OptionalInt[Constants.ChapterCount];
            Attack = new OptionalInt[Constants.ChapterCount];
            FrayDamage = new OptionalInt[Constants.ChapterCount];
            LightDamage = new OptionalInt();
            HeavyDamage = new OptionalInt();
            CriticalDamage = new OptionalInt();
            LightDamageDie = new OptionalString();
            HeavyDamageDie = new OptionalString();
            CriticalDamageDie = new OptionalString();
            DamageType = new OptionalString();
            Traits = new List<Trait>();
            Interrupts = new List<Interrupt>();
            Actions = new List<Action>();
            FactionBlight = new OptionalString();

            for (int i = 0; i < Constants.ChapterCount; ++i)
            {
                Health[i] = new OptionalInt();
                Armor[i] = new OptionalInt();
                Attack[i] = new OptionalInt();
                FrayDamage[i] = new OptionalInt();
            }
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
            if (otherStats.HPMultiplier.IsSet) { newStats.HPMultiplier = otherStats.HPMultiplier; } else { newStats.HPMultiplier = HPMultiplier; }
            if (otherStats.Speed.IsSet) { newStats.Speed = otherStats.Speed; } else { newStats.Speed = Speed; }
            if (otherStats.Run.IsSet) { newStats.Run = otherStats.Run; } else { newStats.Run = Run; }
            if (otherStats.Dash.IsSet) { newStats.Dash = otherStats.Dash; } else { newStats.Dash = Dash; }
            if (otherStats.Defense.IsSet) { newStats.Defense = otherStats.Defense; } else { newStats.Defense = Defense; }
            if (otherStats.LightDamage.IsSet) { newStats.LightDamage = otherStats.LightDamage; } else { newStats.LightDamage = LightDamage; }
            if (otherStats.HeavyDamage.IsSet) { newStats.HeavyDamage = otherStats.HeavyDamage; } else { newStats.HeavyDamage = HeavyDamage; }
            if (otherStats.CriticalDamage.IsSet) { newStats.CriticalDamage = otherStats.CriticalDamage; } else { newStats.CriticalDamage = CriticalDamage; }
            if (otherStats.LightDamageDie.IsSet) { newStats.LightDamageDie = otherStats.LightDamageDie; } else { newStats.LightDamageDie = LightDamageDie; }
            if (otherStats.HeavyDamageDie.IsSet) { newStats.HeavyDamageDie = otherStats.HeavyDamageDie; } else { newStats.HeavyDamageDie = HeavyDamageDie; }
            if (otherStats.CriticalDamageDie.IsSet) { newStats.CriticalDamageDie = otherStats.CriticalDamageDie; } else { newStats.CriticalDamageDie = CriticalDamageDie; }
            if (otherStats.DamageType.IsSet) { newStats.DamageType = otherStats.DamageType; } else { newStats.DamageType = DamageType; }
            if (otherStats.FactionBlight.IsSet) { newStats.FactionBlight = otherStats.FactionBlight; } else { newStats.FactionBlight = FactionBlight; }

            // These replace those of a specific size
            for (int i = 0; i < Constants.ChapterCount; ++i)
            {
                if (otherStats.Health[i].IsSet) { newStats.Health[i] = otherStats.Health[i]; } else { newStats.Health[i] = Health[i]; }
                if (otherStats.Armor[i].IsSet) { newStats.Armor[i] = otherStats.Armor[i]; } else { newStats.Armor[i] = Armor[i]; }
                if (otherStats.Attack[i].IsSet) { newStats.Attack[i] = otherStats.Attack[i]; } else { newStats.Attack[i] = Attack[i]; }
                if (otherStats.FrayDamage[i].IsSet) { newStats.FrayDamage[i] = otherStats.FrayDamage[i]; } else { newStats.FrayDamage[i] = FrayDamage[i]; }
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

    public class OptionalInt
    {
        private int ValueSet;
        public bool IsSet { get; private set; }

        public OptionalInt() { }
        public OptionalInt(int value)
        {
            ValueSet = value;
            IsSet = true;
        }
        public OptionalInt(Int64 value)
        {
            ValueSet = (int)value;
            IsSet = true;
        }

        public static implicit operator int(OptionalInt i) => i.Value;
        public static implicit operator Int64(OptionalInt i) => i.Value;
        public static explicit operator OptionalInt(int i) => new OptionalInt(i);
        public static explicit operator OptionalInt(Int64 i) => new OptionalInt(i);

        public int Value
        {
            get { return ValueSet; }
            set
            {
                ValueSet = value;
                IsSet = true;
            }
        }

        public void SetDefault(int value)
        {
            ValueSet = value;
        }
    }

    public class OptionalDouble
    {
        private double ValueSet;
        public bool IsSet { get; private set; }

        public OptionalDouble() { }
        public OptionalDouble(double value)
        {
            ValueSet = value;
            IsSet = true;
        }

        public static implicit operator double(OptionalDouble d) => d.Value;
        public static explicit operator OptionalDouble(double d) => new OptionalDouble(d);

        public double Value
        {
            get { return ValueSet; }
            set
            {
                ValueSet = value;
                IsSet = true;
            }
        }
    }

    public class OptionalBool
    {
        private bool ValueSet;
        public bool IsSet { get; private set; }

        public OptionalBool() { }
        public OptionalBool(bool value)
        {
            ValueSet = value;
            IsSet = true;
        }

        public static implicit operator bool(OptionalBool b) => b.Value;
        public static explicit operator OptionalBool(bool b) => new OptionalBool(b);

        public bool Value
        {
            get { return ValueSet; }
            set
            {
                ValueSet = value;
                IsSet = true;
            }
        }
    }

    public class OptionalString
    {
        private string ValueSet;
        public bool IsSet { get; private set; }

        public OptionalString()
        {
            ValueSet = String.Empty;
        }
        public OptionalString(string value)
        {
            ValueSet = value;
            IsSet = true;
        }

        public static implicit operator string(OptionalString s) => s.Value;
        public static explicit operator OptionalString(string s) => new OptionalString(s);

        public string Value
        {
            get { return ValueSet; }
            set
            {
                ValueSet = value;
                IsSet = true;
            }
        }
    }

    public class Trait
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public OptionalDouble AddHP { get; set; }
        public OptionalInt AddArmor { get; set; }
        public OptionalInt MaxArmor { get; set; }
        public OptionalBool NoRun { get; set; }
        public OptionalBool NoDash { get; set; }

        public Trait()
        {
            Tags = new List<string>();
            AddHP = new OptionalDouble();
            AddArmor = new OptionalInt();
            MaxArmor = new OptionalInt();
            NoRun = new OptionalBool();
            NoDash = new OptionalBool();
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
