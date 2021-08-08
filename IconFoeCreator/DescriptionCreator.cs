using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconFoeCreator
{
    public static class DescriptionCreator
    {
        public static string CreateDescription(Statistics faction, Statistics type, int chapter, bool useFlatDamage)
        {
            chapter = Math.Max(1, Math.Min(Constants.ChapterCount, chapter));
            int index = chapter - 1;
            Statistics stats = type.InheritFrom(faction);

            // Traits can add armor, max armor, or alter hit points
            int addArmor = 0;
            int maxArmor = Int32.MaxValue;
            double addHP = 0.0;
            bool noRun = false;
            bool noDash = false;
            foreach (Trait trait in stats.Traits)
            {
                if (trait.AddArmor.IsSet) { addArmor += trait.AddArmor.Value; }
                if (trait.MaxArmor.IsSet) { maxArmor = Math.Min(maxArmor, trait.MaxArmor.Value); }
                if (trait.AddHP.IsSet) { addHP += trait.AddHP.Value; }
                if (trait.NoRun.IsSet) { noRun = trait.NoRun.Value; }
                if (trait.NoDash.IsSet) { noDash = trait.NoDash.Value; }
            }

            int health = stats.Health[index].Value;
            double hitPoints = health * stats.HPMultiplier.Value * (1.0 + addHP);
            string run = noRun ? "no run" : "run " + stats.Run.Value;
            string dash = noDash ? "no dash" : "dash " + stats.Dash.Value;
            int defense = stats.Defense.Value + chapter;
            int armor = Math.Min(stats.Armor[index].Value + addArmor, maxArmor);
            int attack = stats.Attack[index].Value;
            int frayDmg = stats.FrayDamage[index].Value;

            string desc = String.Empty;
            desc += faction.ToString() + " " + type.ToString() + Environment.NewLine;
            desc += "Health: " + health + Environment.NewLine;
            desc += "HP: " + (int)hitPoints + Environment.NewLine;
            desc += "Speed: " + stats.Speed.Value + ", " + run + ", " + dash + Environment.NewLine;
            desc += "Defense: " + defense + Environment.NewLine;
            desc += "Armor: " + armor + Environment.NewLine;
            desc += "Attack: " + attack + Environment.NewLine;
            desc += "Fray Damage: " + frayDmg + Environment.NewLine;

            desc += "Damage: ";
            if ((useFlatDamage && stats.LightDamage.IsSet) || !stats.LightDamageDie.IsSet) { desc += (stats.LightDamage.Value + chapter); }
            else { desc += stats.LightDamageDie.Value + "+" + chapter; }
            desc += "/";
            if ((useFlatDamage && stats.HeavyDamage.IsSet) || !stats.HeavyDamageDie.IsSet) { desc += (stats.HeavyDamage.Value + chapter); }
            else { desc += stats.HeavyDamageDie.Value + "+" + chapter; }
            desc += "/";
            if ((useFlatDamage && stats.CriticalDamage.IsSet) || !stats.CriticalDamageDie.IsSet) { desc += (stats.CriticalDamage.Value + chapter); }
            else { desc += stats.CriticalDamageDie.Value + "+" + chapter; }
            desc += Environment.NewLine;

            desc += "Damage Type: " + stats.DamageType.Value + Environment.NewLine;
            desc += "Faction Blight: " + stats.FactionBlight.Value;

            if (stats.Traits.Count > 0)
            {
                desc += Environment.NewLine + Environment.NewLine + "Traits";
                foreach (Trait trait in stats.Traits)
                {
                    desc += Environment.NewLine + trait.Name;
                    
                    if (trait.Tags.Count > 0)
                    {
                        desc += " (";

                        bool first = true;
                        foreach (string tag in trait.Tags)
                        {
                            if (!first) { desc += ", "; }
                            else { first = false; }

                            desc += tag;
                        }

                        desc += ")";
                    }
                    
                    desc += ". " + trait.Description;
                }
            }

            if (stats.Interrupts.Count > 0)
            {
                desc += Environment.NewLine + Environment.NewLine + "Interrupts";
                foreach (Interrupt interrupt in stats.Interrupts)
                {
                    desc += Environment.NewLine;
                    desc += interrupt.Name + " (Interrupt";
                    
                    if (interrupt.Count > 0)
                    {
                        desc += " " + interrupt.Count;
                    }

                    foreach (string tag in interrupt.Tags)
                    {
                        desc += ", " + tag;
                    }

                    desc += "): ";
                    desc += interrupt.Description;
                }
            }

            if (stats.Actions.Count > 0)
            {
                desc += Environment.NewLine + Environment.NewLine + "Actions";
                foreach (Action action in stats.Actions)
                {
                    desc += Environment.NewLine;
                    desc += action.Name + " (";

                    if (action.ActionCost > 0)
                    {
                        desc += action.ActionCost;

                        if (action.ActionCost == 1) { desc += " action"; }
                        else { desc += " actions"; }
                    }
                    else
                    {
                        desc += "Free action";
                    }

                    foreach (string tag in action.Tags)
                    {
                        desc += ", " + tag;
                    }

                    desc += ").";

                    if (action.Description != null && action.Description != String.Empty) { desc += " " + action.Description; }
                    if (action.Hit != null && action.Hit != String.Empty) { desc += " On hit: " + action.Hit; }
                    if (action.CriticalHit != null && action.CriticalHit != String.Empty) { desc += " Critical hit: " + action.CriticalHit; }
                    if (action.Miss != null && action.Miss != String.Empty) { desc += " Miss: " + action.Miss; }
                    if (action.AreaEffect != null && action.AreaEffect != String.Empty) { desc += " Area Effect: " + action.AreaEffect; }
                    if (action.Effect != null && action.Effect != String.Empty) { desc += " Effect: " + action.Effect; }
                    if (action.Collide != null && action.Collide != String.Empty) { desc += " Collide: " + action.Collide; }
                    if (action.Blightboost != null && action.Blightboost != String.Empty) { desc += " Blightboost: " + action.Blightboost; }
                }
            }

            return desc;
        }
    }
}
