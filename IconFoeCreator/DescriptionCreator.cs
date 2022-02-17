using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IconFoeCreator
{
    public static class DescriptionCreator
    {
        public static void UpdateDescription(RichTextBox descTextBox, RichTextBox setupTextBox, List<Statistics> statsList, bool showNonessentialTraits)
        {
            descTextBox.Document.Blocks.Clear();
            setupTextBox.Document.Blocks.Clear();

            if (statsList.Count == 0)
            {
                return;
            }

            Statistics stats = new Statistics();
            string name = String.Empty;
            foreach (Statistics statsToMerge in statsList)
            {
                stats = statsToMerge.InheritFrom(stats);
                name = statsToMerge.Name + " " + name;
            }

            // Traits can add armor, max armor, or alter hit points
            int vitality = stats.Vitality.GetValueOrDefault();
            double addHP = stats.AddHPPercent.GetValueOrDefault(0.0);
            int hitPoints;
            if (stats.HP.HasValue)
            {
                hitPoints = stats.HP.Value;
            }
            else
            {
                hitPoints = vitality * stats.HPMultiplier.GetValueOrDefault(4);
            }
            hitPoints = (int)((double)hitPoints * (1.0 + addHP));
            if (stats.DoubleNormalFoeHP.GetValueOrDefault(false)
                && !stats.IsMob.GetValueOrDefault()
                && !stats.IsElite.GetValueOrDefault()
                && !stats.IsLegend.GetValueOrDefault())
            {
                hitPoints *= 2;
            }

            int speed = stats.Speed.GetValueOrDefault();
            int dash = stats.Dash.GetValueOrDefault();
            string dashText = dash <= 0 ? "No Dash" : ("Dash " + dash);
            int defense = stats.Defense.GetValueOrDefault();
            int armor = stats.Armor.GetValueOrDefault();

            {
                Paragraph paragraph = MakeParagraph();
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, name);
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (vitality > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new System.Windows.Thickness(0);
                AddBold(paragraph, "Vitality: ");
                AddNormal(paragraph, vitality.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "HP: ");

                if (hitPoints > 0)
                {
                    AddNormal(paragraph, hitPoints.ToString());
                    if (stats.HPMultiplyByPlayers.GetValueOrDefault(false))
                    {
                        AddNormal(paragraph, " x number of players characters");
                    }
                }
                else
                {
                    AddNormal(paragraph, "-");
                }

                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Speed: ");
                AddNormal(paragraph, speed + " (" + dashText + ")");
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
                AddBold(paragraph, "Fray Damage: ");
                int frayDmg = stats.FrayDamage.GetValueOrDefault();
                AddNormal(paragraph, frayDmg.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (!String.IsNullOrEmpty(stats.DamageDie))
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "[D]: ");
                if (Char.ToLower(stats.DamageDie[0]) == 'd')
                {
                    AddNormal(paragraph, "1");
                }
                AddNormal(paragraph, stats.DamageDie);
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

                AddBold(paragraph, "):");

                if (!String.IsNullOrEmpty(interrupt.Description))
                {
                    AddNormal(paragraph, " " + interrupt.Description);
                }

                if (!String.IsNullOrEmpty(interrupt.Trigger))
                {
                    AddItalic(paragraph, " Trigger: " );
                    AddNormal(paragraph, interrupt.Trigger);
                }

                if (!String.IsNullOrEmpty(interrupt.Effect))
                {
                    AddItalic(paragraph, " Effect: ");
                    AddNormal(paragraph, interrupt.Effect);
                }

                if (!String.IsNullOrEmpty(interrupt.Collide))
                {
                    AddItalic(paragraph, " Collide: ");
                    AddNormal(paragraph, interrupt.Collide);
                }

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

            if (action.Recharge > 1)
            {
                AddBold(paragraph, ", recharge " + action.Recharge);

                if (action.Recharge < 6)
                {
                    AddBold(paragraph, "+");
                }
            }

            AddBold(paragraph, "):");

            if (!String.IsNullOrEmpty(action.Description))
            {
                AddNormal(paragraph, " " + action.Description);
            }

            if (!String.IsNullOrEmpty(action.Hit))
            {
                AddItalic(paragraph, " On hit: ");
                AddNormal(paragraph, action.Hit);
            }

            if (!String.IsNullOrEmpty(action.AutoHit))
            {
                AddItalic(paragraph, " Autohit: ");
                AddNormal(paragraph, action.AutoHit);
            }

            if (!String.IsNullOrEmpty(action.Critical))
            {
                AddItalic(paragraph, " Critical hit: ");
                AddNormal(paragraph, action.Critical);
            }

            if (!String.IsNullOrEmpty(action.Miss))
            {
                AddItalic(paragraph, " Miss: ");
                AddNormal(paragraph, action.Miss);
            }

            if (!String.IsNullOrEmpty(action.PostAttack))
            {
                AddNormal(paragraph, " " + action.PostAttack);
            }

            if (!String.IsNullOrEmpty(action.AreaEffect))
            {
                AddItalic(paragraph, " Area Effect: ");
                AddNormal(paragraph, action.AreaEffect);
            }

            if (!String.IsNullOrEmpty(action.Effect))
            {
                AddItalic(paragraph, " Effect: ");
                AddNormal(paragraph, action.Effect);
            }

            if (!String.IsNullOrEmpty(action.Mark))
            {
                AddItalic(paragraph, " Mark: ");
                AddNormal(paragraph, action.Mark);
            }

            if (!String.IsNullOrEmpty(action.Stance))
            {
                AddItalic(paragraph, " Stance: ");
                AddNormal(paragraph, action.Stance);
            }

            if (!String.IsNullOrEmpty(action.Collide))
            {
                AddItalic(paragraph, " Collide: ");
                AddNormal(paragraph, action.Collide);
            }

            if (!String.IsNullOrEmpty(action.Blightboost))
            {
                AddItalic(paragraph, " Blightboost: ");
                AddNormal(paragraph, action.Blightboost);
            }

            if (!String.IsNullOrEmpty(action.TerrainEffect))
            {
                AddItalic(paragraph, " Terrain Effect: ");
                AddNormal(paragraph, action.TerrainEffect);
            }

            textBox.Document.Blocks.Add(paragraph);

            foreach (Action comboAction in action.Combos)
            {
                AddAction(textBox, comboAction, true);
            }

            if (!String.IsNullOrEmpty(action.PostAction))
            {
                AddNormal(paragraph, " " + action.PostAction);
            }
        }

        public static void AddBodyParts(RichTextBox textBox, List<BodyPart> bodyParts)
        {
            foreach (BodyPart bodyPart in bodyParts)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, bodyPart.Name + " (" + bodyPart.HP + " hp");
                if (bodyPart.HPMultiplyByPlayers)
                {
                    AddBold(paragraph, "/player");
                }
                AddBold(paragraph, "): ");
                AddNormal(paragraph, bodyPart.Description);
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
