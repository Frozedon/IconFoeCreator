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
        public static void UpdateDescription(RichTextBox descTextBox, RichTextBox setupTextBox, Statistics faction, Statistics job, int chapter, bool useFlatDamage, bool showNonessentialTraits)
        {
            Font regFont = new Font(descTextBox.Font, FontStyle.Regular);
            Font boldFont = new Font(descTextBox.Font, FontStyle.Bold);
            Font italicFont = new Font(descTextBox.Font, FontStyle.Italic);
            Font underlineFont = new Font(descTextBox.Font, FontStyle.Underline);
            Font boldUnderlineFont = new Font(descTextBox.Font, FontStyle.Bold | FontStyle.Underline);

            chapter = Math.Max(1, Math.Min(Constants.ChapterCount, chapter));
            int index = chapter - 1;

            Statistics stats;
            if (Statistics.IsValid(faction))
            {
                stats = job.InheritFrom(faction);
            }
            else
            {
                stats = job;
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

            descTextBox.Clear();

            descTextBox.SelectionFont = boldUnderlineFont;
            if (Statistics.IsValid(faction))
            {
                descTextBox.AppendText(faction.ToString() + " ");
            }
            descTextBox.AppendText(job.ToString() + Environment.NewLine);
            descTextBox.SelectionFont = regFont;

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("Health: ");
            descTextBox.SelectionFont = regFont;
            descTextBox.AppendText(health + Environment.NewLine);

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("HP: ");
            descTextBox.SelectionFont = regFont;
            descTextBox.AppendText((int)hitPoints + Environment.NewLine);

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("Speed: ");
            descTextBox.SelectionFont = regFont;
            descTextBox.AppendText(speed + ", " + run + ", " + dash + Environment.NewLine);

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("Defense: ");
            descTextBox.SelectionFont = regFont;
            descTextBox.AppendText(defense + Environment.NewLine);

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("Armor: ");
            descTextBox.SelectionFont = regFont;
            descTextBox.AppendText(armor + Environment.NewLine);

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("Attack: ");
            descTextBox.SelectionFont = regFont;
            descTextBox.AppendText(attack + Environment.NewLine);

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("Fray Damage: ");
            descTextBox.SelectionFont = regFont;
            descTextBox.AppendText(frayDmg + Environment.NewLine);

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("Damage: ");
            descTextBox.SelectionFont = regFont;

            if ((useFlatDamage && stats.LightDamage.HasValue) || stats.LightDamageDie == null) { descTextBox.AppendText((stats.LightDamage.GetValueOrDefault() + chapter).ToString()); }
            else { descTextBox.AppendText(stats.LightDamageDie + "+" + chapter); }
            descTextBox.AppendText("/");
            if ((useFlatDamage && stats.HeavyDamage.HasValue) || stats.HeavyDamageDie == null) { descTextBox.AppendText((stats.HeavyDamage.GetValueOrDefault() + chapter).ToString()); }
            else { descTextBox.AppendText(stats.HeavyDamageDie + "+" + chapter); }
            descTextBox.AppendText("/");
            if ((useFlatDamage && stats.CriticalDamage.HasValue) || stats.CriticalDamageDie == null) { descTextBox.AppendText((stats.CriticalDamage.GetValueOrDefault() + chapter).ToString()); }
            else { descTextBox.AppendText(stats.CriticalDamageDie + "+" + chapter); }
            descTextBox.AppendText(Environment.NewLine);

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("Damage Type: ");
            descTextBox.SelectionFont = regFont;
            if (stats.DamageType != null) { descTextBox.AppendText(stats.DamageType); }
            descTextBox.AppendText(Environment.NewLine);

            descTextBox.SelectionFont = boldFont;
            descTextBox.AppendText("Faction Blight: ");
            descTextBox.SelectionFont = regFont;
            if (stats.FactionBlight != null) { descTextBox.AppendText(stats.FactionBlight); }

            if (stats.Traits.Count > 0)
            {
                descTextBox.AppendText(Environment.NewLine + Environment.NewLine);
                descTextBox.SelectionFont = underlineFont;
                descTextBox.AppendText("Traits");
                descTextBox.SelectionFont = regFont;

                foreach (Trait trait in stats.Traits)
                {
                    if (showNonessentialTraits || !trait.Nonessential)
                    {
                        descTextBox.SelectionFont = boldFont;
                        descTextBox.AppendText(Environment.NewLine + trait.Name);

                        if (trait.Tags.Count > 0)
                        {
                            descTextBox.AppendText(" (");

                            bool first = true;
                            foreach (string tag in trait.Tags)
                            {
                                if (!first) { descTextBox.AppendText(", "); }
                                else { first = false; }

                                descTextBox.AppendText(tag);
                            }

                            descTextBox.AppendText(")");
                        }

                        descTextBox.AppendText(". ");
                        descTextBox.SelectionFont = regFont;

                        descTextBox.AppendText(trait.Description);
                    }
                }
            }

            if (stats.Interrupts.Count > 0)
            {
                descTextBox.AppendText(Environment.NewLine + Environment.NewLine);
                descTextBox.SelectionFont = underlineFont;
                descTextBox.AppendText("Interrupts");
                descTextBox.SelectionFont = regFont;

                foreach (Interrupt interrupt in stats.Interrupts)
                {
                    descTextBox.AppendText(Environment.NewLine);

                    descTextBox.SelectionFont = boldFont;
                    descTextBox.AppendText(interrupt.Name + " (Interrupt");
                    
                    if (interrupt.Count > 0)
                    {
                        descTextBox.AppendText(" " + interrupt.Count);
                    }

                    foreach (string tag in interrupt.Tags)
                    {
                        descTextBox.AppendText(", " + tag);
                    }

                    descTextBox.AppendText("): ");
                    descTextBox.SelectionFont = regFont;

                    descTextBox.AppendText(interrupt.Description);
                }
            }

            if (stats.Actions.Count > 0)
            {
                descTextBox.AppendText(Environment.NewLine + Environment.NewLine);
                descTextBox.SelectionFont = underlineFont;
                descTextBox.AppendText("Actions");
                descTextBox.SelectionFont = regFont;

                foreach (Action action in stats.Actions)
                {
                    descTextBox.AppendText(Environment.NewLine);

                    descTextBox.SelectionFont = boldFont;
                    descTextBox.AppendText(action.Name + " (");

                    if (action.ActionCost > 0)
                    {
                        descTextBox.AppendText(action.ActionCost.ToString());

                        if (action.ActionCost == 1) { descTextBox.AppendText(" action"); }
                        else { descTextBox.AppendText(" actions"); }
                    }
                    else
                    {
                        descTextBox.AppendText("Free action");
                    }

                    foreach (string tag in action.Tags)
                    {
                        descTextBox.AppendText(", " + tag);
                    }

                    descTextBox.AppendText("):");
                    descTextBox.SelectionFont = regFont;

                    if (action.Description != null && action.Description != String.Empty)
                    {
                        descTextBox.AppendText(" " + action.Description);
                    }

                    if (action.Hit != null && action.Hit != String.Empty)
                    {
                        descTextBox.SelectionFont = italicFont;
                        descTextBox.AppendText(" On hit: ");
                        descTextBox.SelectionFont = regFont;
                        descTextBox.AppendText(action.Hit);
                    }

                    if (action.CriticalHit != null && action.CriticalHit != String.Empty)
                    {
                        descTextBox.SelectionFont = italicFont;
                        descTextBox.AppendText(" Critical hit: ");
                        descTextBox.SelectionFont = regFont;
                        descTextBox.AppendText(action.CriticalHit);
                    }

                    if (action.Miss != null && action.Miss != String.Empty)
                    {
                        descTextBox.SelectionFont = italicFont;
                        descTextBox.AppendText(" Miss: ");
                        descTextBox.SelectionFont = regFont;
                        descTextBox.AppendText(action.Miss);
                    }

                    if (action.AreaEffect != null && action.AreaEffect != String.Empty)
                    {
                        descTextBox.SelectionFont = italicFont;
                        descTextBox.AppendText(" Area Effect: ");
                        descTextBox.SelectionFont = regFont;
                        descTextBox.AppendText(action.AreaEffect);
                    }

                    if (action.Effect != null && action.Effect != String.Empty)
                    {
                        descTextBox.SelectionFont = italicFont;
                        descTextBox.AppendText(" Effect: ");
                        descTextBox.SelectionFont = regFont;
                        descTextBox.AppendText(action.Effect);
                    }

                    if (action.Collide != null && action.Collide != String.Empty)
                    {
                        descTextBox.SelectionFont = italicFont;
                        descTextBox.AppendText(" Collide: ");
                        descTextBox.SelectionFont = regFont;
                        descTextBox.AppendText(action.Collide);
                    }

                    if (action.Blightboost != null && action.Blightboost != String.Empty)
                    {
                        descTextBox.SelectionFont = italicFont;
                        descTextBox.AppendText(" Blightboost: ");
                        descTextBox.SelectionFont = regFont;
                        descTextBox.AppendText(action.Blightboost);
                    }
                }
            }

            // Setup traits in other textbox
            setupTextBox.Clear();

            bool firstSetupTrait = true;
            foreach (Trait trait in stats.SetupTraits)
            {
                if (!firstSetupTrait) { setupTextBox.AppendText(Environment.NewLine); }
                else { firstSetupTrait = false; }

                setupTextBox.SelectionFont = boldFont;
                setupTextBox.AppendText(trait.Name);

                if (trait.Tags.Count > 0)
                {
                    setupTextBox.AppendText(" (");

                    bool first = true;
                    foreach (string tag in trait.Tags)
                    {
                        if (!first) { setupTextBox.AppendText(", "); }
                        else { first = false; }

                        setupTextBox.AppendText(tag);
                    }

                    setupTextBox.AppendText(")");
                }

                setupTextBox.AppendText(". ");
                setupTextBox.SelectionFont = regFont;

                setupTextBox.AppendText(trait.Description);
            }
        }
    }
}
