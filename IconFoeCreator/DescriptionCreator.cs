using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace IconFoeCreator
{
    public class StatData
    {
        public bool replaceDmg;
        public int damageDie;
        public int fray;
        public string className;
        public string foeName;
    }

    public static class DescriptionCreator
    {
        public static readonly double MARGIN_LEN = 16.0;
        public static readonly double MARGIN_BEFORE = 12.0;

        public static void UpdateDescription(RichTextBox descTextBox, RichTextBox setupTextBox, Statistics stats, bool replaceDamageValues)
        {
            descTextBox.Document.Blocks.Clear();
            setupTextBox.Document.Blocks.Clear();

            // Traits can add armor, max armor, or alter hit points
            int vitality = stats.Vitality.GetValueOrDefault();
            int hp;
            if (stats.HP.HasValue)
            {
                hp = stats.HP.Value;
            }
            else
            {
                hp = vitality * stats.HPMultiplier.GetValueOrDefault(4);
            }

            int speed = stats.Speed.GetValueOrDefault();
            int dash = stats.GetDash();
            string dashText = dash <= 0 ? "No Dash" : ("Dash " + dash);
            int defense = stats.Defense.GetValueOrDefault();
            int damageDie = stats.DamageDie.GetValueOrDefault();
            int frayDamage = stats.FrayDamage.GetValueOrDefault();
            int membersPerPlayer = stats.MembersPerPlayer.GetValueOrDefault(0);
            int hits = stats.Hits.GetValueOrDefault(0);
            StatData statData = new StatData {
                replaceDmg = replaceDamageValues,
                damageDie = damageDie,
                fray = frayDamage,
                className = stats.GetClass(),
                foeName = stats.GetDisplayName()
            };

            {
                Paragraph paragraph = MakeParagraph();

                Brush backColor = ThemeColors.GetLinearClassColor(statData.className);
                Brush foreColor = (backColor != null) ? Brushes.White : null;

                AddBold(paragraph, stats.GetDisplayName(), foreColor, backColor);
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (membersPerPlayer > 0)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Members: ");
                AddNormal(paragraph, membersPerPlayer + "/player");
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (vitality > 0)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Vitality: ");
                AddNormal(paragraph, vitality.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (hits > 0)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Hits: ");
                AddNormal(paragraph, hits.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }
            else
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "HP: ");

                string minHPText = "(min " + (hp * 2) + ")";

                if (!String.IsNullOrEmpty(stats.HPText))
                {
                    AddNormal(paragraph, stats.HPText.Replace("[HP]", hp.ToString()).Replace("[MIN]", minHPText));
                }
                else if (hp > 0)
                {
                    AddNormal(paragraph, hp.ToString());
                    if (stats.HPMultiplyByPlayers.GetValueOrDefault(false))
                    {
                        AddNormal(paragraph, " per player character " + minHPText);
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
                if (speed == 0)
                {
                    AddNormal(paragraph, "-");
                }
                else
                {
                    AddNormal(paragraph, speed + " (" + dashText + ")");
                }
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Defense: ");
                AddNormal(paragraph, defense.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (frayDamage > 0 && !replaceDamageValues)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Fray Damage: ");
                AddNormal(paragraph, frayDamage.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (damageDie > 0 && !replaceDamageValues)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "[D]: ");
                AddNormal(paragraph, "1d" + damageDie);
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (stats.Traits.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Top = MARGIN_BEFORE };
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Traits");
                descTextBox.Document.Blocks.Add(paragraph);

                AddTraits(descTextBox, stats.GetActualTraits(), statData);
            }

            var interruptList = stats.GetInterrupts();
            if (interruptList.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Top = MARGIN_BEFORE };
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Interrupts");
                descTextBox.Document.Blocks.Add(paragraph);

                AddInterrupts(descTextBox, interruptList, statData);
            }

            var actionList = stats.GetActions();
            if (actionList.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Top = MARGIN_BEFORE };
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Actions");
                descTextBox.Document.Blocks.Add(paragraph);

                AddActions(descTextBox, actionList, statData);
            }

            if (stats.BodyParts.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Top = MARGIN_BEFORE };
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Body Parts");
                descTextBox.Document.Blocks.Add(paragraph);

                AddBodyParts(descTextBox, stats.BodyParts);
            }

            if (!String.IsNullOrEmpty(stats.PhasesDescription) || stats.Phases.Count > 0)
            {
                Paragraph paragraph1 = MakeParagraph();
                paragraph1.Margin = new Thickness() { Top = MARGIN_BEFORE };
                paragraph1.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph1, "Phases");
                descTextBox.Document.Blocks.Add(paragraph1);

                if (!String.IsNullOrEmpty(stats.PhasesDescription))
                {
                    Paragraph paragraph2 = MakeParagraph();
                    AddNormal(paragraph2, stats.PhasesDescription);
                    descTextBox.Document.Blocks.Add(paragraph2);
                }

                if (stats.Phases.Count > 0)
                {
                    AddPhases(descTextBox, stats.Phases, statData);
                }
            }

            if (stats.ExtraAbilitySets.Count > 0)
            {
                AddExtraAbilitySets(descTextBox, stats.ExtraAbilitySets, statData);
            }

            if (!String.IsNullOrEmpty(stats.Tactics))
            {
                Paragraph paragraph1 = MakeParagraph();
                paragraph1.Margin = new Thickness() { Top = MARGIN_BEFORE };
                paragraph1.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph1, "Tactics");
                descTextBox.Document.Blocks.Add(paragraph1);

                Paragraph paragraph2 = MakeParagraph();
                AddNormal(paragraph2, ReplaceTokens(stats.Tactics, statData));
                descTextBox.Document.Blocks.Add(paragraph2);
            }

            // Setup traits in other textbox
            AddChapter(setupTextBox, stats.Chapter);
            AddEncounterBudget(setupTextBox, stats.GetEncounterBudget());

            if (stats.SetupTraits.Count > 0)
            {
                AddTraits(setupTextBox, stats.SetupTraits, statData);
            }
        }

        private static Paragraph MakeParagraph()
        {
            Paragraph paragraph = new Paragraph()
            {
                Margin = new Thickness(0)
            };
            return paragraph;
        }

        private static void AddNormal(Paragraph paragraph, string text, Brush foreBrush = null, Brush backBrush = null)
        {
            Run formattedText = new Run(text);
            if (foreBrush != null) { formattedText.Foreground = foreBrush; }
            if (backBrush != null) { formattedText.Background = backBrush; }
            paragraph.Inlines.Add(formattedText);
        }

        private static void AddBold(Paragraph paragraph, string text, Brush foreBrush = null, Brush backBrush = null)
        {
            Bold formattedText = new Bold(new Run(text));
            if (foreBrush != null) { formattedText.Foreground = foreBrush; }
            if (backBrush != null) { formattedText.Background = backBrush; }
            paragraph.Inlines.Add(formattedText);
        }

        private static void AddItalic(Paragraph paragraph, string text, Brush foreBrush = null, Brush backBrush = null)
        {
            Italic formattedText = new Italic(new Run(text));
            if (foreBrush != null) { formattedText.Foreground = foreBrush; }
            if (backBrush != null) { formattedText.Background = backBrush; }
            paragraph.Inlines.Add(formattedText);
        }

        private static void AddTraits(RichTextBox textBox, List<TraitData> traits, StatData statData, int indent = 0)
        {
            traits.Sort(delegate (TraitData x, TraitData y)
            {
                return x.Name.CompareTo(y.Name);
            });

            foreach (TraitData trait in traits)
            {
                if (!trait.ShouldDisplay())
                {
                    continue;
                }

                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };

                AddBold(paragraph, trait.GetDisplayName());

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
                
                if (!String.IsNullOrEmpty(trait.Description))
                {
                    AddBold(paragraph, ": ");
                    AddNormal(paragraph, ReplaceTokens(trait.Description, statData));
                }

                foreach (ItemData component in trait.CustomComponents)
                {
                    if (!String.IsNullOrEmpty(component.Name))
                    {
                        AddItalic(paragraph, " " + component.Name + ":");
                    }
                    if (!String.IsNullOrEmpty(component.Description))
                    {
                        AddNormal(paragraph, " " + ReplaceTokens(component.Description, statData));
                    }
                }

                textBox.Document.Blocks.Add(paragraph);


                foreach (ItemData item in trait.ListedItems)
                {
                    AddItemData(textBox, item, statData, indent);
                }

                foreach (RollData roll in trait.Rolls)
                {
                    AddRoll(textBox, roll, statData, indent + 1);
                }

                foreach (ActionData extraAction in trait.ExtraActions)
                {
                    AddAction(textBox, extraAction, statData, indent + 1, false, true);
                }

                AddSummons(textBox, trait.Summons, statData, indent + 1);
            }
        }

        private static void AddInterrupts(RichTextBox textBox, List<InterruptData> interrupts, StatData statData, int indent = 0, bool dot = false)
        {
            interrupts.Sort(delegate (InterruptData x, InterruptData y)
            {
                if (x.Count == y.Count)
                {
                    return x.Name.CompareTo(y.Name);
                }
                return x.Count.CompareTo(y.Count);
            });

            foreach (InterruptData interrupt in interrupts)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };

                if (dot)
                {
                    AddBold(paragraph, "• ");
                }

                AddBold(paragraph, interrupt.Name + " (Interrupt");

                if (interrupt.Count > 0)
                {
                    AddBold(paragraph, " " + interrupt.Count);
                }

                foreach (string tag in interrupt.Tags)
                {
                    AddBold(paragraph, ", " + tag);
                }

                if (interrupt.Recharge > 1)
                {
                    AddBold(paragraph, ", recharge " + interrupt.Recharge.ToString());
                }

                AddBold(paragraph, "):");

                if (!String.IsNullOrEmpty(interrupt.Description))
                {
                    AddNormal(paragraph, " " + ReplaceTokens(interrupt.Description, statData));
                }

                if (!String.IsNullOrEmpty(interrupt.Trigger))
                {
                    AddItalic(paragraph, " Trigger: " );
                    AddNormal(paragraph, ReplaceTokens(interrupt.Trigger, statData));
                }

                foreach (string effect in interrupt.Effects)
                {
                    AddItalic(paragraph, " Effect: ");
                    AddNormal(paragraph, ReplaceTokens(effect, statData));
                }

                if (!String.IsNullOrEmpty(interrupt.Collide))
                {
                    AddItalic(paragraph, " Collide: ");
                    AddNormal(paragraph, ReplaceTokens(interrupt.Collide, statData));
                }

                textBox.Document.Blocks.Add(paragraph);

                foreach (ItemData item in interrupt.ListedItems)
                {
                    AddItemData(textBox, item, statData, indent);
                }

                AddSummons(textBox, interrupt.Summons, statData, indent + 1);
            }
        }

        private static void AddActions(RichTextBox textBox, List<ActionData> actions, StatData statData, int indent = 0, bool combo = false, bool dot = false)
        {
            actions.Sort(delegate (ActionData x, ActionData y)
            {
                if (x.RoundAction != y.RoundAction)
                {
                    return x.RoundAction ? -1 : 1;
                }

                if (x.ActionCost == y.ActionCost)
                {
                    bool xHasAtk = x.Tags.Find(tag => tag.Contains("attack")) != null;
                    bool yHasAtk = y.Tags.Find(tag => tag.Contains("attack")) != null;

                    if (xHasAtk == yHasAtk)
                    {
                        return x.Name.CompareTo(y.Name);
                    }
                    else if (xHasAtk && !yHasAtk)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                return x.ActionCost.CompareTo(y.ActionCost);
            });

            foreach (ActionData action in actions)
            {
                AddAction(textBox, action, statData, indent, combo, dot);
            }
        }

        private static void AddAction(RichTextBox textBox, ActionData action, StatData statData, int indent = 0, bool combo = false, bool dot = false)
        {
            Paragraph paragraph = MakeParagraph();
            paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };
            
            if (combo)
            {
                AddBold(paragraph, "• Combo: ");
            }
            else if (dot)
            {
                AddBold(paragraph, "• ");
            }

            if (!String.IsNullOrEmpty(action.Name))
            {
                AddBold(paragraph, action.Name);
            }

            if (action.ActionCost >= 0 || action.RoundAction == true || action.Tags.Count > 0 || action.Recharge > 1)
            {
                AddBold(paragraph, " (");

                bool firstItem = true;
                if (action.ActionCost > 0)
                {
                    AddBold(paragraph, action.ActionCost.ToString());

                    if (action.ActionCost == 1) { AddBold(paragraph, " action"); }
                    else { AddBold(paragraph, " actions"); }

                    firstItem = false;
                }
                else if (action.ActionCost == 0)
                {
                    AddBold(paragraph, "free action");

                    firstItem = false;
                }

                if (action.RoundAction == true)
                {
                    if (firstItem) { firstItem = false; }
                    else { AddBold(paragraph, ", "); }

                    AddBold(paragraph, "Round Action");
                }

                foreach (string tag in action.Tags)
                {
                    if (firstItem) { firstItem = false; }
                    else { AddBold(paragraph, ", "); }

                    AddBold(paragraph, tag);
                }

                if (action.Recharge > 1)
                {
                    if (firstItem) { firstItem = false; }
                    else { AddBold(paragraph, ", "); }

                    AddBold(paragraph, "recharge " + action.Recharge);

                    if (action.Recharge < 6)
                    {
                        AddBold(paragraph, "+");
                    }
                }

                if (action.Combo != null)
                {
                    if (firstItem) { firstItem = false; }
                    else { AddBold(paragraph, ", "); }

                    AddBold(paragraph, "combo");
                }

                AddBold(paragraph, "):");
            }

            if (!String.IsNullOrEmpty(action.Description))
            {
                AddNormal(paragraph, " " + ReplaceTokens(action.Description, statData));
            }

            foreach (string effect in action.PreEffects)
            {
                AddItalic(paragraph, " Effect: ");
                AddNormal(paragraph, ReplaceTokens(effect, statData));
            }

            if (!String.IsNullOrEmpty(action.Hit))
            {
                AddItalic(paragraph, " On hit: ");
                AddNormal(paragraph, ReplaceTokens(action.Hit, statData));
            }

            if (!String.IsNullOrEmpty(action.AutoHit))
            {
                AddItalic(paragraph, " Autohit: ");
                AddNormal(paragraph, ReplaceTokens(action.AutoHit, statData));
            }

            if (!String.IsNullOrEmpty(action.Miss) && !String.IsNullOrEmpty(action.AreaEffect) && action.Miss == action.AreaEffect)
            {
                AddItalic(paragraph, " Miss or Area Effect: ");
                AddNormal(paragraph, ReplaceTokens(action.Miss, statData));
            }
            else
            {
                if (!String.IsNullOrEmpty(action.Miss))
                {
                    AddItalic(paragraph, " Miss: ");
                    AddNormal(paragraph, ReplaceTokens(action.Miss, statData));
                }
                
                if (!String.IsNullOrEmpty(action.AreaEffect))
                {
                    AddItalic(paragraph, " Area Effect: ");
                    AddNormal(paragraph, ReplaceTokens(action.AreaEffect, statData));
                }
            }

            if (!String.IsNullOrEmpty(action.CriticalHit))
            {
                AddItalic(paragraph, " Critical hit: ");
                AddNormal(paragraph, ReplaceTokens(action.CriticalHit, statData));
            }

            foreach (string effect in action.Effects)
            {
                AddItalic(paragraph, " Effect: ");
                AddNormal(paragraph, ReplaceTokens(effect, statData));
            }

            if (!String.IsNullOrEmpty(action.Mark))
            {
                AddItalic(paragraph, " Mark: ");
                AddNormal(paragraph, ReplaceTokens(action.Mark, statData));
            }

            if (!String.IsNullOrEmpty(action.Stance))
            {
                AddItalic(paragraph, " Stance: ");
                AddNormal(paragraph, ReplaceTokens(action.Stance, statData));
            }

            if (!String.IsNullOrEmpty(action.TerrainEffect))
            {
                AddItalic(paragraph, " Terrain Effect: ");
                AddNormal(paragraph, ReplaceTokens(action.TerrainEffect, statData));
            }

            if (!String.IsNullOrEmpty(action.Collide))
            {
                AddItalic(paragraph, " Collide: ");
                AddNormal(paragraph, ReplaceTokens(action.Collide, statData));
            }

            if (!String.IsNullOrEmpty(action.Slay))
            {
                AddItalic(paragraph, " Slay: ");
                AddNormal(paragraph, ReplaceTokens(action.Slay, statData));
            }

            if (!String.IsNullOrEmpty(action.Exceed))
            {
                AddItalic(paragraph, " Exceed: ");
                AddNormal(paragraph, ReplaceTokens(action.Exceed, statData));
            }

            if (!String.IsNullOrEmpty(action.Special))
            {
                AddItalic(paragraph, " Special: ");
                AddNormal(paragraph, ReplaceTokens(action.Special, statData));
            }

            if (!String.IsNullOrEmpty(action.SpecialInterrupt))
            {
                AddItalic(paragraph, " Interrupt: ");
                AddNormal(paragraph, ReplaceTokens(action.SpecialInterrupt, statData));
            }

            if (!String.IsNullOrEmpty(action.SpecialRecharge))
            {
                AddItalic(paragraph, " Recharge: ");
                AddNormal(paragraph, ReplaceTokens(action.SpecialRecharge, statData));
            }

            if (!String.IsNullOrEmpty(action.Charge))
            {
                AddItalic(paragraph, " Charge: ");
                AddNormal(paragraph, ReplaceTokens(action.Charge, statData));
            }

            if (!String.IsNullOrEmpty(action.Delay))
            {
                AddItalic(paragraph, " Delay: ");
                AddNormal(paragraph, ReplaceTokens(action.Delay, statData));
            }

            foreach (ItemData component in action.CustomComponents)
            {
                if (!String.IsNullOrEmpty(component.Name))
                {
                    AddItalic(paragraph, " " + component.Name + ":");
                }
                if (!String.IsNullOrEmpty(component.Description))
                {
                    AddNormal(paragraph, " " + ReplaceTokens(component.Description, statData));
                }
            }

            foreach (string effect in action.PostEffects)
            {
                AddItalic(paragraph, " Effect: ");
                AddNormal(paragraph, ReplaceTokens(effect, statData));
            }

            if (!String.IsNullOrEmpty(action.PostAreaEffect))
            {
                AddItalic(paragraph, " Area Effect: ");
                AddNormal(paragraph, ReplaceTokens(action.PostAreaEffect, statData));
            }

            if (!String.IsNullOrEmpty(action.PostCollide))
            {
                AddItalic(paragraph, " Collide: ");
                AddNormal(paragraph, ReplaceTokens(action.PostCollide, statData));
            }

            textBox.Document.Blocks.Add(paragraph);

            foreach (ItemData item in action.ListedItems)
            {
                AddItemData(textBox, item, statData, indent);
            }

            foreach (RollData roll in action.Rolls)
            {
                AddRoll(textBox, roll, statData, indent + 1);
            }

            foreach (ActionData extraAction in action.ExtraActions)
            {
                AddAction(textBox, extraAction, statData, indent + 1, false, true);
            }

            AddSummons(textBox, action.Summons, statData, (action.Combo != null) ? indent + 2 : indent + 1);

            AddInterrupts(textBox, action.Interrupts, statData, indent + 1, false);

            if (action.Combo != null)
            {
                AddAction(textBox, action.Combo, statData, indent + 1, true);
            }

            if (!String.IsNullOrEmpty(action.PostAction))
            {
                AddNormal(paragraph, " " + ReplaceTokens(action.PostAction, statData));
            }
        }

        private static void AddRoll(RichTextBox textBox, RollData roll, StatData statData, int indent = 0)
        {
            Paragraph paragraph = MakeParagraph();
            paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };

            // Write values like "1." or "1-3." or "1-2, 4."
            roll.Values.Sort();
            int lastValue = int.MinValue;
            int lastAddedValue = int.MinValue;
            string valueStr = String.Empty;
            foreach (int value in roll.Values)
            {
                if (String.IsNullOrEmpty(valueStr))
                {
                    valueStr += value;
                    lastAddedValue = value;
                }
                else if (value - lastValue > 1)
                {
                    if (lastAddedValue != lastValue)
                    {
                        valueStr += "-" + lastValue;
                    }
                    valueStr += "," + value;
                    lastAddedValue = value;
                }

                lastValue = value;
            }

            if (lastAddedValue != int.MinValue && lastAddedValue != lastValue)
            {
                if (!String.IsNullOrEmpty(valueStr))
                {
                    if (lastValue - lastAddedValue > 1)
                    {
                        valueStr += "-";
                    }
                    else
                    {
                        valueStr += ",";
                    }

                }

                valueStr += lastValue;
            }

            if (!String.IsNullOrEmpty(valueStr))
            {
                AddBold(paragraph, valueStr);
                if (roll.Plus) { AddBold(paragraph, "+"); }
                AddBold(paragraph, ". ");
            }

            if (!String.IsNullOrEmpty(roll.Name))
            {
                AddBold(paragraph, roll.Name + ": ");
            }

            if (!String.IsNullOrEmpty(roll.Description))
            {
                AddNormal(paragraph, ReplaceTokens(roll.Description, statData));
            }

            foreach (ItemData component in roll.CustomComponents)
            {
                if (!String.IsNullOrEmpty(component.Name))
                {
                    AddItalic(paragraph, " " + component.Name + ":");
                }
                if (!String.IsNullOrEmpty(component.Description))
                {
                    AddNormal(paragraph, " " + ReplaceTokens(component.Description, statData));
                }
            }

            textBox.Document.Blocks.Add(paragraph);
        }

        private static void AddSummons(RichTextBox textBox, List<SummonData> summons, StatData statData, int indent = 0)
        {
            bool first = true;
            foreach (SummonData summon in summons)
            {
                int spaceBefore = 0;
                if (first) { first = false; }
                else { spaceBefore = 1; }

                AddSummon(textBox, summon, statData, indent, spaceBefore);
            }
        }

        private static void AddSummon(RichTextBox textBox, SummonData summon, StatData statData, int indent = 0, int spaceBefore = 0)
        {
            if (!String.IsNullOrEmpty(summon.Name) || summon.Tags.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent, Top = MARGIN_BEFORE * spaceBefore };

                string className = summon.Class;
                if (String.IsNullOrEmpty(className))
                {
                    className = statData.className;
                }

                Brush backColor = ThemeColors.GetLinearClassColor(className);
                Brush foreColor = (backColor != null) ? Brushes.White : null;

                AddBold(paragraph, summon.Name, foreColor, backColor);

                textBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };

                if (summon.IsObject)
                {
                    AddBold(paragraph, "Object");
                }
                else
                {
                    AddBold(paragraph, "Summon");
                }

                if (summon.Tags.Count > 0)
                {
                    foreach (string tag in summon.Tags)
                    {
                        AddBold(paragraph, ", " + tag);
                    }
                }

                textBox.Document.Blocks.Add(paragraph);
            }

            AddTraits(textBox, summon.GetActualTraits(), statData, indent);

            if (!String.IsNullOrEmpty(summon.Description))
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };
                AddNormal(paragraph, summon.Description);
                textBox.Document.Blocks.Add(paragraph);
            }

            foreach (string effect in summon.SummonEffects)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };
                if (summon.IsObject) { AddBold(paragraph, "Object "); }
                else { AddBold(paragraph, "Summon "); }
                AddBold(paragraph, "Effect: ");
                AddNormal(paragraph, ReplaceTokens(effect, statData));
                textBox.Document.Blocks.Add(paragraph);
            }

            foreach (string action in summon.SummonActions)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };
                if (summon.IsObject) { AddBold(paragraph, "Object "); }
                else { AddBold(paragraph, "Summon "); }
                AddBold(paragraph, "Action: ");
                AddNormal(paragraph, ReplaceTokens(action, statData));
                textBox.Document.Blocks.Add(paragraph);
            }

            var actionList = summon.GetActions();
            AddActions(textBox, actionList, statData, indent, false, false);

            var interruptList = summon.GetInterrupts();
            AddInterrupts(textBox, interruptList, statData, indent, false);

            foreach (ItemData itemData in summon.ListedItems)
            {
                AddItemData(textBox, itemData, statData, indent);
            }

            AddActions(textBox, summon.ListedActions, statData, indent + 1, false, true);

            AddInterrupts(textBox, summon.ListedInterrupts, statData, indent + 1, true);
        }

        private static void AddBodyParts(RichTextBox textBox, List<BodyPartData> bodyParts)
        {
            foreach (BodyPartData bodyPart in bodyParts)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, bodyPart.Name);
                
                if (bodyPart.HP > 0) {

                    AddBold(paragraph, " (" + bodyPart.HP + " hp");
                    if (bodyPart.HPMultiplyByPlayers)
                    {
                        AddBold(paragraph, "/player");
                    }
                    AddBold(paragraph, ")");
                }

                AddBold(paragraph, ": ");
                AddNormal(paragraph, bodyPart.Description);
                textBox.Document.Blocks.Add(paragraph);
            }
        }

        private static void AddPhases(RichTextBox textBox, List<PhaseData> phases, StatData statData, int indent = 0)
        {
            for (int i = 0; i < phases.Count; ++i)
            {
                PhaseData phase = phases[i];
                Paragraph paragraph1 = MakeParagraph();
                paragraph1.Margin = new Thickness() { Top = MARGIN_BEFORE };
                AddBold(paragraph1, "Phase " + ToRoman(i + 1));
                if (!String.IsNullOrEmpty(phase.Name)) { AddBold(paragraph1, ": " + phase.Name); }
                textBox.Document.Blocks.Add(paragraph1);

                if (!String.IsNullOrEmpty(phase.Description))
                {
                    Paragraph paragraph2 = MakeParagraph();
                    AddNormal(paragraph2, phase.Description);
                    textBox.Document.Blocks.Add(paragraph2);
                }

                foreach (ItemData itemData in phase.ListedItems)
                {
                    AddItemData(textBox, itemData, statData, indent);
                }

                if (phase.Traits.Count > 0)
                {
                    AddTraits(textBox, phase.GetActualTraits(), statData);
                }

                if (phase.Interrupts.Count > 0)
                {
                    AddInterrupts(textBox, phase.Interrupts, statData);
                }

                var actionList = phase.GetActions();
                AddActions(textBox, actionList, statData, indent, false, false);

                var interruptList = phase.GetInterrupts();
                AddInterrupts(textBox, interruptList, statData, indent, false);
            }
        }

        private static void AddExtraAbilitySets(RichTextBox textBox, List<AbilitySetData> abilitySets, StatData statData, int indent = 0)
        {
            for (int i = 0; i < abilitySets.Count; ++i)
            {
                AbilitySetData abilitySet = abilitySets[i];

                Paragraph paragraph1 = MakeParagraph();
                paragraph1.Margin = new Thickness() { Top = MARGIN_BEFORE };

                string className = abilitySet.Class;
                if (String.IsNullOrEmpty(className))
                {
                    className = statData.className;
                }

                Brush backColor = ThemeColors.GetLinearClassColor(className);
                Brush foreColor = (backColor != null) ? Brushes.White : null;

                AddBold(paragraph1, abilitySet.Name, foreColor, backColor);
                
                textBox.Document.Blocks.Add(paragraph1);

                if (!String.IsNullOrEmpty(abilitySet.Description))
                {
                    Paragraph paragraph2 = MakeParagraph();
                    AddNormal(paragraph2, abilitySet.Description);
                    textBox.Document.Blocks.Add(paragraph2);
                }

                if (abilitySet.Traits.Count > 0)
                {
                    AddTraits(textBox, abilitySet.GetActualTraits(), statData);
                }

                if (abilitySet.Interrupts.Count > 0)
                {
                    AddInterrupts(textBox, abilitySet.Interrupts, statData);
                }

                var actionList = abilitySet.GetActions();
                AddActions(textBox, actionList, statData, indent, false, false);

                var interruptList = abilitySet.GetInterrupts();
                AddInterrupts(textBox, interruptList, statData, indent, false);
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
                AddNormal(paragraph, "1 point in an encounter budget gets " + amount + " of these foes.");
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

        private static void AddChapter(RichTextBox textBox, int chapter)
        {
            Paragraph paragraph = MakeParagraph();
            AddBold(paragraph, "Chapter. ");

            int clampedChapter = Math.Min(Math.Max(chapter, 1), 3);
            AddNormal(paragraph, clampedChapter.ToString());

            if (clampedChapter < 3)
            {
                AddNormal(paragraph, "+");
            }

            textBox.Document.Blocks.Add(paragraph);
        }

        private static void AddItemData(RichTextBox textBox, ItemData item, StatData statData, int indent = 0)
        {
            Paragraph paragraph = MakeParagraph();
            paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };

            AddBold(paragraph, "•");

            if (!String.IsNullOrEmpty(item.Name))
            {
                AddBold(paragraph, " " + item.Name + ":");
            }
            if (!String.IsNullOrEmpty(item.Description))
            {
                AddNormal(paragraph, " " + ReplaceTokens(item.Description, statData));
            }

            foreach (ItemData component in item.CustomComponents)
            {
                if (!String.IsNullOrEmpty(component.Name))
                {
                    AddItalic(paragraph, " " + component.Name + ":");
                }
                if (!String.IsNullOrEmpty(component.Description))
                {
                    AddNormal(paragraph, " " + ReplaceTokens(component.Description, statData));
                }
            }

            foreach (ItemData listedItem in item.ListedItems)
            {
                AddItemData(textBox, listedItem, statData, indent + 1);
            }

            textBox.Document.Blocks.Add(paragraph);
        }

        private static string ReplaceTokens(string text, StatData statData)
        {
            var tokensToAlwaysReplace = new Dictionary<string, string>
            {
                { "[name]", statData.foeName }
            };

            foreach (var key in tokensToAlwaysReplace.Keys)
            {
                text = text.Replace(key, tokensToAlwaysReplace[key]);
            }

            if (!statData.replaceDmg)
            {
                return text;
            }

            var tokens = new Dictionary<string, string>
            {
                { "[D]", "D" + statData.damageDie.ToString() },
                { "[fray]", statData.fray.ToString() }
            };

            foreach (var key in tokens.Keys)
            {
                text = text.Replace(key, tokens[key]);
            }

            return text;
        }

        private static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }
    }
}
