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
            int maxArmor = int.MaxValue;
            double addHP = 0.0;
            int addSpeed = 0;
            int addRun = 0;
            int addDash = 0;
            bool noRun = false;
            bool noDash = false;
            foreach (Trait trait in stats.Traits)
            {
                if (trait.AddArmor.HasValue) { addArmor += trait.AddArmor.Value; }
                if (trait.MaxArmor.HasValue) { maxArmor = Math.Min(maxArmor, trait.MaxArmor.Value); }
                if (trait.AddHP.HasValue) { addHP += trait.AddHP.Value; }
                if (trait.AddSpeed.HasValue) { addSpeed += trait.AddSpeed.Value; }
                if (trait.AddRun.HasValue) { addRun += trait.AddRun.Value; }
                if (trait.AddDash.HasValue) { addDash += trait.AddDash.Value; }
                if (trait.NoRun.HasValue) { noRun = trait.NoRun.Value; }
                if (trait.NoDash.HasValue) { noDash = trait.NoDash.Value; }
            }

            int health = stats.Health[index].GetValueOrDefault();
            double hitPoints = health * stats.HPMultiplier.GetValueOrDefault(4) * (1.0 + addHP);
            hitPoints = Math.Max(hitPoints, 1);
            int speed = stats.Speed.GetValueOrDefault() + addSpeed;
            string run = noRun ? "no run" : "run " + (stats.Run.GetValueOrDefault() + addRun);
            string dash = noDash ? "no dash" : "dash " + (stats.Dash.GetValueOrDefault() + addDash);
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
            descTextBox.AppendText(((int)hitPoints).ToString());
            if (stats.HPMultiplyByPlayers.GetValueOrDefault(false))
            {
                descTextBox.AppendText(" x number of players characters");
            }
            descTextBox.AppendText(Environment.NewLine);

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
            descTextBox.AppendText("+" + attack + Environment.NewLine);

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
                descTextBox.AppendText("Traits" + Environment.NewLine);
                descTextBox.SelectionFont = regFont;

                AddTraits(descTextBox, stats.Traits, showNonessentialTraits, regFont, boldFont, italicFont);
            }

            if (stats.Interrupts.Count > 0)
            {
                descTextBox.AppendText(Environment.NewLine + Environment.NewLine);
                descTextBox.SelectionFont = underlineFont;
                descTextBox.AppendText("Interrupts" + Environment.NewLine);
                descTextBox.SelectionFont = regFont;

                AddInterrupts(descTextBox, stats.Interrupts, regFont, boldFont);
            }

            if (stats.Actions.Count > 0)
            {
                descTextBox.AppendText(Environment.NewLine + Environment.NewLine);
                descTextBox.SelectionFont = underlineFont;
                descTextBox.AppendText("Actions" + Environment.NewLine);
                descTextBox.SelectionFont = regFont;

                AddActions(descTextBox, stats.Actions, regFont, boldFont, italicFont);
            }

            if (stats.BodyParts.Count > 0)
            {
                descTextBox.AppendText(Environment.NewLine + Environment.NewLine);
                descTextBox.SelectionFont = underlineFont;
                descTextBox.AppendText("Body Parts" + Environment.NewLine);
                descTextBox.SelectionFont = regFont;

                AddBodyParts(descTextBox, stats.BodyParts, regFont, boldFont, italicFont);
            }

            if (stats.Phases.Count > 0)
            {
                descTextBox.AppendText(Environment.NewLine + Environment.NewLine);
                descTextBox.SelectionFont = underlineFont;
                descTextBox.AppendText("Phases");
                descTextBox.SelectionFont = regFont;
                descTextBox.AppendText(Environment.NewLine + stats.PhasesDescription);
                descTextBox.AppendText(Environment.NewLine + Environment.NewLine);

                AddPhases(descTextBox, stats.Phases, regFont, boldFont, italicFont);
            }

            // Setup traits in other textbox
            setupTextBox.Clear();

            if (stats.SetupTraits.Count > 0)
            {
                AddSetupTraits(setupTextBox, stats.SetupTraits, regFont, boldFont);
            }
        }

        public static void AddTraits(RichTextBox descTextBox, List<Trait> traits, bool showNonessentialTraits, Font regFont, Font boldFont, Font italicFont)
        {
            bool first = true;
            foreach (Trait trait in traits)
            {
                if (showNonessentialTraits || !trait.Nonessential)
                {
                    if (!first) { descTextBox.AppendText(Environment.NewLine); }
                    else { first = false; }

                    descTextBox.SelectionFont = boldFont;
                    descTextBox.AppendText(trait.Name);

                    if (trait.Tags.Count > 0)
                    {
                        descTextBox.AppendText(" (");

                        bool firstTag = true;
                        foreach (string tag in trait.Tags)
                        {
                            if (!firstTag) { descTextBox.AppendText(", "); }
                            else { firstTag = false; }

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

        public static void AddInterrupts(RichTextBox descTextBox, List<Interrupt> interrupts, Font regFont, Font boldFont)
        {
            bool first = true;
            foreach (Interrupt interrupt in interrupts)
            {
                if (!first) { descTextBox.AppendText(Environment.NewLine); }
                else { first = false; }

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

        public static void AddActions(RichTextBox descTextBox, List<Action> actions, Font regFont, Font boldFont, Font italicFont)
        {
            bool first = true;
            foreach (Action action in actions)
            {
                if (!first) { descTextBox.AppendText(Environment.NewLine); }
                else { first = false; }

                AddAction(descTextBox, action, regFont, boldFont, italicFont);
            }
        }

        public static void AddAction(RichTextBox descTextBox, Action action, Font regFont, Font boldFont, Font italicFont)
        {
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

            foreach (Action comboAction in action.Combos)
            {
                descTextBox.AppendText(Environment.NewLine);
                descTextBox.SelectionFont = boldFont;
                descTextBox.AppendText("• Combo: ");
                descTextBox.SelectionFont = regFont;
                AddAction(descTextBox, comboAction, regFont, boldFont, italicFont);
            }
        }

        public static void AddBodyParts(RichTextBox descTextBox, List<BodyPart> bodyParts, Font regFont, Font boldFont, Font italicFont)
        {
            bool first = true;
            foreach (BodyPart bodyPart in bodyParts)
            {
                if (!first) { descTextBox.AppendText(Environment.NewLine); }
                else { first = false; }

                descTextBox.SelectionFont = boldFont;
                descTextBox.AppendText(bodyPart.Name);
                descTextBox.AppendText(" (" + bodyPart.HP + " hp)");

                descTextBox.SelectionFont = regFont;
                descTextBox.AppendText(" - " + bodyPart.Description);
            }
        }

        public static void AddPhases(RichTextBox descTextBox, List<Phase> phases, Font regFont, Font boldFont, Font italicFont)
        {
            for (int i = 0; i < phases.Count; ++i)
            {
                Phase phase = phases[i];
                if (i > 0) { descTextBox.AppendText(Environment.NewLine + Environment.NewLine); }

                descTextBox.SelectionFont = boldFont;
                descTextBox.AppendText("Phase " + (i + 1) + ": ");

                descTextBox.SelectionFont = regFont;
                descTextBox.AppendText(phase.Name);

                if (phase.Description != null && phase.Description != String.Empty)
                {
                    descTextBox.AppendText(Environment.NewLine + phase.Description);
                }

                if (phase.Traits.Count > 0)
                {
                    descTextBox.AppendText(Environment.NewLine);
                    AddTraits(descTextBox, phase.Traits, true, regFont, boldFont, italicFont);
                }

                if (phase.Actions.Count > 0)
                {
                    descTextBox.AppendText(Environment.NewLine);
                    AddActions(descTextBox, phase.Actions, regFont, boldFont, italicFont);
                }
            }
        }

        public static void AddSetupTraits(RichTextBox setupTextBox, List<Trait> traits, Font regFont, Font boldFont)
        {
            bool first = true;
            foreach (Trait trait in traits)
            {
                if (!first) { setupTextBox.AppendText(Environment.NewLine); }
                else { first = false; }

                setupTextBox.SelectionFont = boldFont;
                setupTextBox.AppendText(trait.Name);

                if (trait.Tags.Count > 0)
                {
                    setupTextBox.AppendText(" (");

                    bool firstTag = true;
                    foreach (string tag in trait.Tags)
                    {
                        if (!firstTag) { setupTextBox.AppendText(", "); }
                        else { firstTag = false; }

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
