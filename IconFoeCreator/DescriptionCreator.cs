using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace IconFoeCreator
{
    public static class DescriptionCreator
    {
        public static void UpdateDescription(RichTextBox textBox, Statistics faction, Statistics type, int chapter, bool useFlatDamage, bool showSetupTraits)
        {
            Font regFont = new Font(textBox.Font, FontStyle.Regular);
            Font boldFont = new Font(textBox.Font, FontStyle.Bold);
            Font italicFont = new Font(textBox.Font, FontStyle.Italic);
            Font underlineFont = new Font(textBox.Font, FontStyle.Underline);
            Font boldUnderlineFont = new Font(textBox.Font, FontStyle.Bold | FontStyle.Underline);

            chapter = Math.Max(1, Math.Min(Constants.ChapterCount, chapter));
            int index = chapter - 1;

            Statistics stats;
            if (faction != null && faction.Name != null && faction.Name != "...")
            {
                stats = type.InheritFrom(faction);
            }
            else
            {
                stats = type;
            }

            // Traits can add armor, max armor, or alter hit points
            int addArmor = 0;
            int maxArmor = Int32.MaxValue;
            double addHP = 0.0;
            bool noRun = false;
            bool noDash = false;
            foreach (Trait trait in stats.Traits)
            {
                if (trait.AddArmor.HasValue) { addArmor += trait.AddArmor.Value; }
                if (trait.MaxArmor.HasValue) { maxArmor = Math.Min(maxArmor, trait.MaxArmor.Value); }
                if (trait.AddHP.HasValue) { addHP += trait.AddHP.Value; }
                if (trait.NoRun.HasValue) { noRun = trait.NoRun.Value; }
                if (trait.NoDash.HasValue) { noDash = trait.NoDash.Value; }
            }

            int health = stats.Health[index].GetValueOrDefault();
            double hitPoints = health * stats.HPMultiplier.GetValueOrDefault(4) * (1.0 + addHP);
            hitPoints = Math.Max(hitPoints, 1);
            int speed = stats.Speed.GetValueOrDefault();
            string run = noRun ? "no run" : "run " + stats.Run.GetValueOrDefault();
            string dash = noDash ? "no dash" : "dash " + stats.Dash.GetValueOrDefault();
            int defense = stats.Defense.GetValueOrDefault() + chapter;
            int armor = Math.Min(stats.Armor[index].GetValueOrDefault() + addArmor, maxArmor);
            int attack = stats.Attack[index].GetValueOrDefault();
            int frayDmg = stats.FrayDamage[index].GetValueOrDefault();

            textBox.Clear();

            textBox.SelectionFont = boldUnderlineFont;
            if (faction != null && faction.Name != null && faction.Name != "...")
            {
                textBox.AppendText(faction.ToString() + " ");
            }
            textBox.AppendText(type.ToString() + Environment.NewLine);
            textBox.SelectionFont = regFont;

            textBox.SelectionFont = boldFont;
            textBox.AppendText("Health: ");
            textBox.SelectionFont = regFont;
            textBox.AppendText(health + Environment.NewLine);

            textBox.SelectionFont = boldFont;
            textBox.AppendText("HP: ");
            textBox.SelectionFont = regFont;
            textBox.AppendText((int)hitPoints + Environment.NewLine);

            textBox.SelectionFont = boldFont;
            textBox.AppendText("Speed: ");
            textBox.SelectionFont = regFont;
            textBox.AppendText(speed + ", " + run + ", " + dash + Environment.NewLine);

            textBox.SelectionFont = boldFont;
            textBox.AppendText("Defense: ");
            textBox.SelectionFont = regFont;
            textBox.AppendText(defense + Environment.NewLine);

            textBox.SelectionFont = boldFont;
            textBox.AppendText("Armor: ");
            textBox.SelectionFont = regFont;
            textBox.AppendText(armor + Environment.NewLine);

            textBox.SelectionFont = boldFont;
            textBox.AppendText("Attack: ");
            textBox.SelectionFont = regFont;
            textBox.AppendText(attack + Environment.NewLine);

            textBox.SelectionFont = boldFont;
            textBox.AppendText("Fray Damage: ");
            textBox.SelectionFont = regFont;
            textBox.AppendText(frayDmg + Environment.NewLine);

            textBox.SelectionFont = boldFont;
            textBox.AppendText("Damage: ");
            textBox.SelectionFont = regFont;

            if ((useFlatDamage && stats.LightDamage.HasValue) || stats.LightDamageDie == null) { textBox.AppendText((stats.LightDamage.GetValueOrDefault() + chapter).ToString()); }
            else { textBox.AppendText(stats.LightDamageDie + "+" + chapter); }
            textBox.AppendText("/");
            if ((useFlatDamage && stats.HeavyDamage.HasValue) || stats.HeavyDamageDie == null) { textBox.AppendText((stats.HeavyDamage.GetValueOrDefault() + chapter).ToString()); }
            else { textBox.AppendText(stats.HeavyDamageDie + "+" + chapter); }
            textBox.AppendText("/");
            if ((useFlatDamage && stats.CriticalDamage.HasValue) || stats.CriticalDamageDie == null) { textBox.AppendText((stats.CriticalDamage.GetValueOrDefault() + chapter).ToString()); }
            else { textBox.AppendText(stats.CriticalDamageDie + "+" + chapter); }
            textBox.AppendText(Environment.NewLine);

            textBox.SelectionFont = boldFont;
            textBox.AppendText("Damage Type: ");
            textBox.SelectionFont = regFont;
            if (stats.DamageType != null) { textBox.AppendText(stats.DamageType); }
            textBox.AppendText(Environment.NewLine);

            textBox.SelectionFont = boldFont;
            textBox.AppendText("Faction Blight: ");
            textBox.SelectionFont = regFont;
            if (stats.FactionBlight != null) { textBox.AppendText(stats.FactionBlight); }

            if (stats.Traits.Count > 0)
            {
                textBox.AppendText(Environment.NewLine + Environment.NewLine);
                textBox.SelectionFont = underlineFont;
                textBox.AppendText("Traits");
                textBox.SelectionFont = regFont;

                foreach (Trait trait in stats.Traits)
                {
                    if (showSetupTraits || !trait.Setup)
                    {
                        textBox.SelectionFont = boldFont;
                        textBox.AppendText(Environment.NewLine + trait.Name);

                        if (trait.Tags.Count > 0)
                        {
                            textBox.AppendText(" (");

                            bool first = true;
                            foreach (string tag in trait.Tags)
                            {
                                if (!first) { textBox.AppendText(", "); }
                                else { first = false; }

                                textBox.AppendText(tag);
                            }

                            textBox.AppendText(")");
                        }

                        textBox.AppendText(". ");
                        textBox.SelectionFont = regFont;

                        textBox.AppendText(trait.Description);
                    }
                }
            }

            if (stats.Interrupts.Count > 0)
            {
                textBox.AppendText(Environment.NewLine + Environment.NewLine);
                textBox.SelectionFont = underlineFont;
                textBox.AppendText("Interrupts");
                textBox.SelectionFont = regFont;

                foreach (Interrupt interrupt in stats.Interrupts)
                {
                    textBox.AppendText(Environment.NewLine);

                    textBox.SelectionFont = boldFont;
                    textBox.AppendText(interrupt.Name + " (Interrupt");
                    
                    if (interrupt.Count > 0)
                    {
                        textBox.AppendText(" " + interrupt.Count);
                    }

                    foreach (string tag in interrupt.Tags)
                    {
                        textBox.AppendText(", " + tag);
                    }

                    textBox.AppendText("): ");
                    textBox.SelectionFont = regFont;

                    textBox.AppendText(interrupt.Description);
                }
            }

            if (stats.Actions.Count > 0)
            {
                textBox.AppendText(Environment.NewLine + Environment.NewLine);
                textBox.SelectionFont = underlineFont;
                textBox.AppendText("Actions");
                textBox.SelectionFont = regFont;

                foreach (Action action in stats.Actions)
                {
                    textBox.AppendText(Environment.NewLine);

                    textBox.SelectionFont = boldFont;
                    textBox.AppendText(action.Name + " (");

                    if (action.ActionCost > 0)
                    {
                        textBox.AppendText(action.ActionCost.ToString());

                        if (action.ActionCost == 1) { textBox.AppendText(" action"); }
                        else { textBox.AppendText(" actions"); }
                    }
                    else
                    {
                        textBox.AppendText("Free action");
                    }

                    foreach (string tag in action.Tags)
                    {
                        textBox.AppendText(", " + tag);
                    }

                    textBox.AppendText("):");
                    textBox.SelectionFont = regFont;

                    if (action.Description != null && action.Description != String.Empty)
                    {
                        textBox.AppendText(" " + action.Description);
                    }

                    if (action.Hit != null && action.Hit != String.Empty)
                    {
                        textBox.SelectionFont = italicFont;
                        textBox.AppendText(" On hit: ");
                        textBox.SelectionFont = regFont;
                        textBox.AppendText(action.Hit);
                    }

                    if (action.CriticalHit != null && action.CriticalHit != String.Empty)
                    {
                        textBox.SelectionFont = italicFont;
                        textBox.AppendText(" Critical hit: ");
                        textBox.SelectionFont = regFont;
                        textBox.AppendText(action.CriticalHit);
                    }

                    if (action.Miss != null && action.Miss != String.Empty)
                    {
                        textBox.SelectionFont = italicFont; 
                        textBox.AppendText(" Miss: ");
                        textBox.SelectionFont = regFont;
                        textBox.AppendText(action.Miss);
                    }

                    if (action.AreaEffect != null && action.AreaEffect != String.Empty)
                    {
                        textBox.SelectionFont = italicFont;
                        textBox.AppendText(" Area Effect: ");
                        textBox.SelectionFont = regFont;
                        textBox.AppendText(action.AreaEffect);
                    }

                    if (action.Effect != null && action.Effect != String.Empty)
                    {
                        textBox.SelectionFont = italicFont;
                        textBox.AppendText(" Effect: ");
                        textBox.SelectionFont = regFont;
                        textBox.AppendText(action.Effect);
                    }

                    if (action.Collide != null && action.Collide != String.Empty)
                    {
                        textBox.SelectionFont = italicFont;
                        textBox.AppendText(" Collide: ");
                        textBox.SelectionFont = regFont;
                        textBox.AppendText(action.Collide);
                    }

                    if (action.Blightboost != null && action.Blightboost != String.Empty)
                    {
                        textBox.SelectionFont = italicFont;
                        textBox.AppendText(" Blightboost: ");
                        textBox.SelectionFont = regFont;
                        textBox.AppendText(action.Blightboost);
                    }
                }
            }
        }
    }
}
