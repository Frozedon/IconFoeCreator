using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IconFoeCreator
{
    public static class DescriptionCreator
    {
        public static void UpdateDescription(RichTextBox descTextBox, RichTextBox setupTextBox, List<Statistics> statsList, bool replaceDamageValues, bool showNonessentialTraits)
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
                string title = statsToMerge.Name;
                if (!String.IsNullOrEmpty(statsToMerge.TitleName))
                {
                    title = statsToMerge.TitleName;
                }
                name = title + " " + name;
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

            int frayDamage = stats.FrayDamage.GetValueOrDefault();
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Fray Damage: ");
                AddNormal(paragraph, frayDamage.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            int damageDie = stats.DamageDie.GetValueOrDefault();
            if (damageDie > 0)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "[D]: ");
                AddNormal(paragraph, "1d" + damageDie);
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

                AddTraits(descTextBox, stats.Traits, damageDie, frayDamage, replaceDamageValues, showNonessentialTraits);
            }

            if (stats.Interrupts.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Interrupts");
                descTextBox.Document.Blocks.Add(paragraph);

                AddInterrupts(descTextBox, stats.Interrupts, damageDie, frayDamage, replaceDamageValues);
            }

            if (stats.Actions.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Actions");
                descTextBox.Document.Blocks.Add(paragraph);

                AddActions(descTextBox, stats.Actions, damageDie, frayDamage, replaceDamageValues);
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

                AddPhases(descTextBox, stats.Phases, damageDie, frayDamage, replaceDamageValues);
            }

            // Setup traits in other textbox
            AddEncounterBudget(setupTextBox, stats.EncounterBudget.GetValueOrDefault(1.0));

            if (stats.SetupTraits.Count > 0)
            {
                AddTraits(setupTextBox, stats.SetupTraits, damageDie, frayDamage, replaceDamageValues, true);
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

        private static void AddTraits(RichTextBox textBox, List<Trait> traits, int damageDie, int fray, bool replaceDmg, bool showNonessentialTraits, int indent = 0)
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

                    if (indent > 0)
                    {
                        paragraph.TextIndent = (indent - 1) * 10.0;
                        AddBold(paragraph, "• ");
                    }
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
                    AddNormal(paragraph, ReplaceDamageTokens(trait.Description, damageDie, fray, replaceDmg));

                    if (showNonessentialTraits)
                    {
                        AddNormal(paragraph, " " + trait.DescriptionNonessential);
                    }

                    textBox.Document.Blocks.Add(paragraph);
                }
            }
        }

        private static void AddInterrupts(RichTextBox textBox, List<Interrupt> interrupts, int damageDie, int fray, bool replaceDmg)
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
                    AddNormal(paragraph, " " + ReplaceDamageTokens(interrupt.Description, damageDie, fray, replaceDmg));
                }

                if (!String.IsNullOrEmpty(interrupt.Trigger))
                {
                    AddItalic(paragraph, " Trigger: " );
                    AddNormal(paragraph, ReplaceDamageTokens(interrupt.Trigger, damageDie, fray, replaceDmg));
                }

                if (!String.IsNullOrEmpty(interrupt.Effect))
                {
                    AddItalic(paragraph, " Effect: ");
                    AddNormal(paragraph, ReplaceDamageTokens(interrupt.Effect, damageDie, fray, replaceDmg));
                }

                if (!String.IsNullOrEmpty(interrupt.Collide))
                {
                    AddItalic(paragraph, " Collide: ");
                    AddNormal(paragraph, ReplaceDamageTokens(interrupt.Collide, damageDie, fray, replaceDmg));
                }

                textBox.Document.Blocks.Add(paragraph);
            }
        }

        private static void AddActions(RichTextBox textBox, List<Action> actions, int damageDie, int fray, bool replaceDmg)
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
                AddAction(textBox, action, damageDie, fray, replaceDmg);
            }
        }

        private static void AddAction(RichTextBox textBox, Action action, int damageDie, int fray, bool replaceDmg, bool combo = false)
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
                AddNormal(paragraph, " " + ReplaceDamageTokens(action.Description, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.Hit))
            {
                AddItalic(paragraph, " On hit: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Hit, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.AutoHit))
            {
                AddItalic(paragraph, " Autohit: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.AutoHit, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.Critical))
            {
                AddItalic(paragraph, " Critical hit: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Critical, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.Miss))
            {
                AddItalic(paragraph, " Miss: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Miss, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.PostAttack))
            {
                AddNormal(paragraph, " " + ReplaceDamageTokens(action.PostAttack, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.AreaEffect))
            {
                AddItalic(paragraph, " Area Effect: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.AreaEffect, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.Effect))
            {
                AddItalic(paragraph, " Effect: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Effect, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.Mark))
            {
                AddItalic(paragraph, " Mark: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Mark, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.Stance))
            {
                AddItalic(paragraph, " Stance: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Stance, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.Collide))
            {
                AddItalic(paragraph, " Collide: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Collide, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.Blightboost))
            {
                AddItalic(paragraph, " Blightboost: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Blightboost, damageDie, fray, replaceDmg));
            }

            if (!String.IsNullOrEmpty(action.TerrainEffect))
            {
                AddItalic(paragraph, " Terrain Effect: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.TerrainEffect, damageDie, fray, replaceDmg));
            }

            textBox.Document.Blocks.Add(paragraph);

            foreach (Action comboAction in action.Combos)
            {
                AddAction(textBox, comboAction, damageDie, fray, true);
            }

            if (!String.IsNullOrEmpty(action.PostAction))
            {
                AddNormal(paragraph, " " + ReplaceDamageTokens(action.PostAction, damageDie, fray, replaceDmg));
            }
        }

        private static void AddBodyParts(RichTextBox textBox, List<BodyPart> bodyParts)
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

        private static void AddPhases(RichTextBox textBox, List<Phase> phases, int damageDie, int fray, bool replaceDmg)
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
                    AddTraits(textBox, phase.Traits, damageDie, fray, replaceDmg, true);
                }

                if (phase.Actions.Count > 0)
                {
                    AddActions(textBox, phase.Actions, damageDie, fray, replaceDmg);
                }
            }
        }

        private static void AddEncounterBudget(RichTextBox textBox, double encounterBudget)
        {
            Paragraph paragraph = MakeParagraph();
            AddBold(paragraph, "Encounter Budget. ");

            if (encounterBudget < 0.0)
            {
                AddNormal(paragraph, "This foe is worth the entire encounter budget.");
            }
            else if (encounterBudget > 0.0 && encounterBudget < 1.0)
            {
                int amount = (int)(1.0 / encounterBudget);
                AddNormal(paragraph, "1 point in an encounter budget gets " + amount + " mobs of the same type.");
            }
            else if (encounterBudget == 1.0)
            {
                AddNormal(paragraph, "This foe takes up " + (int)encounterBudget + " point in an encounter budget.");
            }
            else
            {
                AddNormal(paragraph, "This foe takes up " + (int)encounterBudget + " points in an encounter budget.");
            }

            textBox.Document.Blocks.Add(paragraph);
        }

        private static string ReplaceDamageTokens(string text, int damageDie, int fray, bool replaceDmg)
        {
            if (!replaceDmg)
            {
                return text;
            }

            var tokens = new Dictionary<string, string>
            {
                { "[D]", "d" + damageDie.ToString() },
                { "[fray]", fray.ToString() }
            };

            foreach(var key in tokens.Keys)
            {
                text = text.Replace(key, tokens[key]);
            }

            return text;
        }
    }
}
