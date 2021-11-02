using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IconFoeCreator
{
    public static class DescriptionCreator
    {
        public static void UpdateDescription(RichTextBox descTextBox, RichTextBox setupTextBox, Statistics template, Statistics job, int chapter, bool useFlatDamage, bool showNonessentialTraits)
        {
            descTextBox.Document.Blocks.Clear();

            bool hasTemplate = template != null && Statistics.IsValid(template);
            bool hasJob = job != null && Statistics.IsValid(job);
            Statistics stats;
            if (hasTemplate && hasJob)
            {
                stats = job.InheritFrom(template);
            }
            else if (hasJob)
            {
                stats = job;
            }
            else if (hasTemplate)
            {
                stats = template;
            }
            else
            {
                return;
            }

            chapter = Math.Max(1, Math.Min(Constants.ChapterCount, chapter));
            int index = chapter - 1;

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
                if (trait.AddHPPercent.HasValue) { addHP += trait.AddHPPercent.Value; }
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

            {
                Paragraph paragraph = MakeParagraph();
                paragraph.TextDecorations = TextDecorations.Underline;
                if (hasTemplate)
                {
                    AddBold(paragraph, template.ToString());
                }
                if (hasTemplate && hasJob)
                {
                    AddBold(paragraph, " ");
                }
                if (hasJob)
                {
                    AddBold(paragraph, job.ToString());
                }
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new System.Windows.Thickness(0);
                AddBold(paragraph, "Health: ");
                AddNormal(paragraph, health.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "HP: ");
                AddNormal(paragraph, ((int)hitPoints).ToString());
                if (stats.HPMultiplyByPlayers.GetValueOrDefault(false))
                {
                    AddNormal(paragraph, " x number of players characters");
                }
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Speed: ");
                AddNormal(paragraph, speed + ", " + run + ", " + dash);
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Defense: ");
                AddNormal(paragraph, defense.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Armor: ");
                AddNormal(paragraph, armor.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Attack: ");
                AddNormal(paragraph, "+" + attack);
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Fray Damage: ");
                AddNormal(paragraph, frayDmg.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Damage: ");

                if ((useFlatDamage && stats.LightDamage.HasValue) || stats.LightDamageDie == null) { AddNormal(paragraph, (stats.LightDamage.GetValueOrDefault() + chapter).ToString()); }
                else { AddNormal(paragraph, stats.LightDamageDie + "+" + chapter); }
                AddNormal(paragraph, "/");
                if ((useFlatDamage && stats.HeavyDamage.HasValue) || stats.HeavyDamageDie == null) { AddNormal(paragraph, (stats.HeavyDamage.GetValueOrDefault() + chapter).ToString()); }
                else { AddNormal(paragraph, stats.HeavyDamageDie + "+" + chapter); }
                AddNormal(paragraph, "/");
                if ((useFlatDamage && stats.CriticalDamage.HasValue) || stats.CriticalDamageDie == null) { AddNormal(paragraph, (stats.CriticalDamage.GetValueOrDefault() + chapter).ToString()); }
                else { AddNormal(paragraph, stats.CriticalDamageDie + "+" + chapter); }

                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Damage Type: ");
                if (stats.DamageType != null) { AddNormal(paragraph, stats.DamageType); }
                else { AddNormal(paragraph, "Either" ); }
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Faction Blight: ");
                if (stats.FactionBlight != null) { AddNormal(paragraph, stats.FactionBlight); }
                else { AddNormal(paragraph, "Any"); }
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (stats.Traits.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Traits");
                descTextBox.Document.Blocks.Add(paragraph);

                AddTraits(descTextBox, stats.Traits, showNonessentialTraits);
            }

            if (stats.Interrupts.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Interrupts");
                descTextBox.Document.Blocks.Add(paragraph);

                AddInterrupts(descTextBox, stats.Interrupts);
            }

            if (stats.Actions.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Actions");
                descTextBox.Document.Blocks.Add(paragraph);

                AddActions(descTextBox, stats.Actions);
            }

            if (stats.BodyParts.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Body Parts");
                descTextBox.Document.Blocks.Add(paragraph);

                AddBodyParts(descTextBox, stats.BodyParts);
            }

            if (stats.Phases.Count > 0)
            {
                Paragraph paragraph1 = MakeParagraph();
                paragraph1.Margin = new Thickness(0, 12, 0, 0);
                paragraph1.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph1, "Phases");
                descTextBox.Document.Blocks.Add(paragraph1);

                Paragraph paragraph2 = MakeParagraph();
                AddNormal(paragraph2, stats.PhasesDescription);
                descTextBox.Document.Blocks.Add(paragraph2);

                AddPhases(descTextBox, stats.Phases);
            }

            // Setup traits in other textbox
            setupTextBox.Document.Blocks.Clear();

            if (stats.SetupTraits.Count > 0)
            {
                AddSetupTraits(setupTextBox, stats.SetupTraits);
            }
        }

        private static Paragraph MakeParagraph()
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Margin = new System.Windows.Thickness(0);
            return paragraph;
        }

        private static void AddNormal(Paragraph paragraph, string text)
        {
            paragraph.Inlines.Add(new Run(text));
        }

        private static void AddBold(Paragraph paragraph, string text)
        {
            paragraph.Inlines.Add(new Bold(new Run(text)));
        }

        private static void AddItalic(Paragraph paragraph, string text)
        {
            paragraph.Inlines.Add(new Italic(new Run(text)));
        }

        public static void AddTraits(RichTextBox textBox, List<Trait> traits, bool showNonessentialTraits)
        {
            traits.Sort(delegate (Trait x, Trait y)
            {
                return x.Name.CompareTo(y.Name);
            });

            foreach (Trait trait in traits)
            {
                if (showNonessentialTraits || !trait.Nonessential)
                {
                    Paragraph paragraph = MakeParagraph();

                    AddBold(paragraph, trait.Name);

                    if (trait.Tags.Count > 0)
                    {
                        AddBold(paragraph, " (");

                        bool firstTag = true;
                        foreach (string tag in trait.Tags)
                        {
                            if (!firstTag) { AddBold(paragraph, ", "); }
                            else { firstTag = false; }

                            AddBold(paragraph, tag);
                        }

                        AddBold(paragraph, ")");
                    }

                    AddBold(paragraph, ". ");
                    AddNormal(paragraph, trait.Description);

                    if (showNonessentialTraits)
                    {
                        AddNormal(paragraph, " " + trait.DescriptionNonessential);
                    }

                    textBox.Document.Blocks.Add(paragraph);
                }
            }
        }

        public static void AddInterrupts(RichTextBox textBox, List<Interrupt> interrupts)
        {
            interrupts.Sort(delegate (Interrupt x, Interrupt y)
            {
                if (x.Count == y.Count)
                {
                    return x.Name.CompareTo(y.Name);
                }
                return x.Count.CompareTo(y.Count);
            });

            foreach (Interrupt interrupt in interrupts)
            {
                Paragraph paragraph = MakeParagraph();

                AddBold(paragraph, interrupt.Name + " (Interrupt");

                if (interrupt.Count > 0)
                {
                    AddBold(paragraph, " " + interrupt.Count);
                }

                foreach (string tag in interrupt.Tags)
                {
                    AddBold(paragraph, ", " + tag);
                }

                AddBold(paragraph, "): ");
                AddNormal(paragraph, interrupt.Description);

                textBox.Document.Blocks.Add(paragraph);
            }
        }

        public static void AddActions(RichTextBox textBox, List<Action> actions)
        {
            actions.Sort(delegate (Action x, Action y)
            {
                if (x.ActionCost == y.ActionCost)
                {
                    return x.Name.CompareTo(y.Name);
                }
                return x.ActionCost.CompareTo(y.ActionCost);
            });

            foreach (Action action in actions)
            {
                AddAction(textBox, action);
            }
        }

        public static void AddAction(RichTextBox textBox, Action action, bool combo = false)
        {
            Paragraph paragraph = MakeParagraph();

            if (combo)
            {
                AddBold(paragraph, "• Combo: ");
            }

            AddBold(paragraph, action.Name + " (");

            if (action.ActionCost > 0)
            {
                AddBold(paragraph, action.ActionCost.ToString());

                if (action.ActionCost == 1) { AddBold(paragraph, " action"); }
                else { AddBold(paragraph, " actions"); }
            }
            else
            {
                AddBold(paragraph, "Free action");
            }

            foreach (string tag in action.Tags)
            {
                AddBold(paragraph, ", " + tag);
            }

            AddBold(paragraph, "):");

            if (action.Description != null && action.Description != String.Empty)
            {
                AddNormal(paragraph, " " + action.Description);
            }

            if (action.Hit != null && action.Hit != String.Empty)
            {
                AddItalic(paragraph, " On hit: ");
                AddNormal(paragraph, action.Hit);
            }

            if (action.CriticalHit != null && action.CriticalHit != String.Empty)
            {
                AddItalic(paragraph, " Critical hit: ");
                AddNormal(paragraph, action.CriticalHit);
            }

            if (action.Miss != null && action.Miss != String.Empty)
            {
                AddItalic(paragraph, " Miss: ");
                AddNormal(paragraph, action.Miss);
            }

            if (action.AreaEffect != null && action.AreaEffect != String.Empty)
            {
                AddItalic(paragraph, " Area Effect: ");
                AddNormal(paragraph, action.AreaEffect);
            }

            if (action.Effect != null && action.Effect != String.Empty)
            {
                AddItalic(paragraph, " Effect: ");
                AddNormal(paragraph, action.Effect);
            }

            if (action.Collide != null && action.Collide != String.Empty)
            {
                AddItalic(paragraph, " Collide: ");
                AddNormal(paragraph, action.Collide);
            }

            if (action.Blightboost != null && action.Blightboost != String.Empty)
            {
                AddItalic(paragraph, " Blightboost: ");
                AddNormal(paragraph, action.Blightboost);
            }

            textBox.Document.Blocks.Add(paragraph);

            foreach (Action comboAction in action.Combos)
            {
                AddAction(textBox, comboAction, true);
            }
        }

        public static void AddBodyParts(RichTextBox textBox, List<BodyPart> bodyParts)
        {
            foreach (BodyPart bodyPart in bodyParts)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, bodyPart.Name + " (" + bodyPart.HP + " hp)");
                AddNormal(paragraph, " - " + bodyPart.Description);
                textBox.Document.Blocks.Add(paragraph);
            }
        }

        public static void AddPhases(RichTextBox textBox, List<Phase> phases)
        {
            for (int i = 0; i < phases.Count; ++i)
            {
                Phase phase = phases[i];
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);

                AddBold(paragraph, "Phase " + (i + 1) + ": ");
                AddNormal(paragraph, phase.Name);

                if (phase.Description != null && phase.Description != String.Empty)
                {
                    AddNormal(paragraph, " (" + phase.Description + ")");
                }

                textBox.Document.Blocks.Add(paragraph);

                if (phase.Traits.Count > 0)
                {
                    AddTraits(textBox, phase.Traits, true);
                }

                if (phase.Actions.Count > 0)
                {
                    AddActions(textBox, phase.Actions);
                }
            }
        }

        public static void AddSetupTraits(RichTextBox textBox, List<Trait> traits)
        {
            foreach (Trait trait in traits)
            {
                Paragraph paragraph = MakeParagraph();

                AddBold(paragraph, trait.Name);

                if (trait.Tags.Count > 0)
                {
                    AddBold(paragraph, " (");

                    bool firstTag = true;
                    foreach (string tag in trait.Tags)
                    {
                        if (!firstTag) { AddBold(paragraph, ", "); }
                        else { firstTag = false; }

                        AddBold(paragraph, tag);
                    }

                    AddBold(paragraph, ")");
                }

                AddBold(paragraph, ". ");
                AddNormal(paragraph, trait.Description);

                textBox.Document.Blocks.Add(paragraph);
            }
        }
    }
}
