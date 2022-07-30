using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IconFoeCreator
{
    public class DamageInfo
    {
        public bool replaceDesc;
        public int damageDie;
        public int fray;
    }

    public static class DescriptionCreator
    {
        public static readonly double MARGIN_LEN = 16.0;

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
            DamageInfo damageInfo = new DamageInfo {
                replaceDesc = replaceDamageValues,
                damageDie = damageDie,
                fray = frayDamage
            };

            {
                Paragraph paragraph = MakeParagraph();
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, stats.GetDisplayName());
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
                AddNormal(paragraph, speed + " (" + dashText + ")");
                descTextBox.Document.Blocks.Add(paragraph);
            }

            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Defense: ");
                AddNormal(paragraph, defense.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (frayDamage > 0)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "Fray Damage: ");
                AddNormal(paragraph, frayDamage.ToString());
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (damageDie > 0)
            {
                Paragraph paragraph = MakeParagraph();
                AddBold(paragraph, "[D]: ");
                AddNormal(paragraph, "1d" + damageDie);
                descTextBox.Document.Blocks.Add(paragraph);
            }

            if (stats.Traits.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Traits");
                descTextBox.Document.Blocks.Add(paragraph);

                AddTraits(descTextBox, stats.GetActualTraits(), damageInfo);
            }

            if (stats.Interrupts.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Interrupts");
                descTextBox.Document.Blocks.Add(paragraph);

                AddInterrupts(descTextBox, stats.Interrupts, damageInfo);
            }

            if (stats.Actions.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness(0, 12, 0, 0);
                paragraph.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph, "Actions");
                descTextBox.Document.Blocks.Add(paragraph);

                AddActions(descTextBox, stats.Actions, damageInfo);
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

            if (!String.IsNullOrEmpty(stats.PhasesDescription) || stats.Phases.Count > 0)
            {
                Paragraph paragraph1 = MakeParagraph();
                paragraph1.Margin = new Thickness(0, 12, 0, 0);
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
                    AddPhases(descTextBox, stats.Phases, damageInfo);
                }
            }

            if (stats.ExtraAbilitySets.Count > 0)
            {
                AddExtraAbilitySets(descTextBox, stats.ExtraAbilitySets, damageInfo);
            }

            // Setup traits in other textbox
            AddEncounterBudget(setupTextBox, stats.GetEncounterBudget());

            if (stats.SetupTraits.Count > 0)
            {
                AddTraits(setupTextBox, stats.SetupTraits, damageInfo);
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

        private static void AddTraits(RichTextBox textBox, List<TraitData> traits, DamageInfo dmgInfo, int indent = 0)
        {
            traits.Sort(delegate (TraitData x, TraitData y)
            {
                return x.Name.CompareTo(y.Name);
            });

            foreach (TraitData trait in traits)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.TextIndent = MARGIN_LEN * indent;

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

                AddBold(paragraph, ". ");
                
                if (!String.IsNullOrEmpty(trait.Description))
                {
                    AddNormal(paragraph, ReplaceDamageTokens(trait.Description, dmgInfo));
                }

                textBox.Document.Blocks.Add(paragraph);

                foreach (SummonData summon in trait.Summons)
                {
                    AddSummon(textBox, summon, dmgInfo, indent + 1);
                }
            }
        }

        private static void AddInterrupts(RichTextBox textBox, List<InterruptData> interrupts, DamageInfo dmgInfo, int indent = 0)
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
                paragraph.TextIndent = MARGIN_LEN * indent;

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
                    AddNormal(paragraph, " " + ReplaceDamageTokens(interrupt.Description, dmgInfo));
                }

                if (!String.IsNullOrEmpty(interrupt.Trigger))
                {
                    AddItalic(paragraph, " Trigger: " );
                    AddNormal(paragraph, ReplaceDamageTokens(interrupt.Trigger, dmgInfo));
                }

                foreach (string effect in interrupt.Effects)
                {
                    AddItalic(paragraph, " Effect: ");
                    AddNormal(paragraph, ReplaceDamageTokens(effect, dmgInfo));
                }

                if (!String.IsNullOrEmpty(interrupt.Collide))
                {
                    AddItalic(paragraph, " Collide: ");
                    AddNormal(paragraph, ReplaceDamageTokens(interrupt.Collide, dmgInfo));
                }

                textBox.Document.Blocks.Add(paragraph);
            }
        }

        private static void AddActions(RichTextBox textBox, List<ActionData> actions, DamageInfo dmgInfo)
        {
            actions.Sort(delegate (ActionData x, ActionData y)
            {
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
                AddAction(textBox, action, dmgInfo);
            }
        }

        private static void AddAction(RichTextBox textBox, ActionData action, DamageInfo dmgInfo, int indent = 0, bool combo = false)
        {
            Paragraph paragraph = MakeParagraph();
            paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };

            if (combo)
            {
                AddBold(paragraph, "• Combo: ");
            }

            if (!String.IsNullOrEmpty(action.Name))
            {
                AddBold(paragraph, action.Name);
            }

            if (action.ActionCost >= 0 || action.Tags.Count > 0 || action.Recharge > 1)
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

                if (action.Combos.Count > 0)
                {
                    if (firstItem) { firstItem = false; }
                    else { AddBold(paragraph, ", "); }

                    AddBold(paragraph, "combo");
                }

                AddBold(paragraph, "):");
            }

            if (!String.IsNullOrEmpty(action.Description))
            {
                AddNormal(paragraph, " " + ReplaceDamageTokens(action.Description, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.Hit))
            {
                AddItalic(paragraph, " On hit: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Hit, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.AutoHit))
            {
                AddItalic(paragraph, " Autohit: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.AutoHit, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.Miss) && !String.IsNullOrEmpty(action.AreaEffect) && action.Miss == action.AreaEffect)
            {
                AddItalic(paragraph, " Miss or Area Effect: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Miss, dmgInfo));
            }
            else
            {
                if (!String.IsNullOrEmpty(action.Miss))
                {
                    AddItalic(paragraph, " Miss: ");
                    AddNormal(paragraph, ReplaceDamageTokens(action.Miss, dmgInfo));
                }
                
                if (!String.IsNullOrEmpty(action.AreaEffect))
                {
                    AddItalic(paragraph, " Area Effect: ");
                    AddNormal(paragraph, ReplaceDamageTokens(action.AreaEffect, dmgInfo));
                }
            }

            if (!String.IsNullOrEmpty(action.CriticalHit))
            {
                AddItalic(paragraph, " Critical hit: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.CriticalHit, dmgInfo));
            }

            foreach (string effect in action.Effects)
            {
                AddItalic(paragraph, " Effect: ");
                AddNormal(paragraph, ReplaceDamageTokens(effect, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.Mark))
            {
                AddItalic(paragraph, " Mark: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Mark, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.Stance))
            {
                AddItalic(paragraph, " Stance: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Stance, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.Collide))
            {
                AddItalic(paragraph, " Collide: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Collide, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.Blightboost))
            {
                AddItalic(paragraph, " Blightboost: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Blightboost, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.TerrainEffect))
            {
                AddItalic(paragraph, " Terrain Effect: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.TerrainEffect, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.SpecialInterrupt))
            {
                AddItalic(paragraph, " Interrupt: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.SpecialInterrupt, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.SpecialRecharge))
            {
                AddItalic(paragraph, " Recharge: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.SpecialRecharge, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.Delay))
            {
                AddItalic(paragraph, " Delay: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Delay, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.PostAreaEffect))
            {
                AddItalic(paragraph, " Area Effect: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.PostAreaEffect, dmgInfo));
            }

            foreach (ComponentData component in action.CustomComponents)
            {
                if (!String.IsNullOrEmpty(component.Name))
                {
                    AddItalic(paragraph, " " + component.Name);
                }
                if (!String.IsNullOrEmpty(component.Description))
                {
                    AddNormal(paragraph, " " + ReplaceDamageTokens(action.Description, dmgInfo));
                }
            }

            textBox.Document.Blocks.Add(paragraph);

            foreach (RollData roll in action.Rolls)
            {
                AddRoll(textBox, roll, dmgInfo, indent + 1);
            }

            foreach (SummonData summon in action.Summons)
            {
                AddSummon(textBox, summon, dmgInfo, indent + 1);
            }

            foreach (ActionData comboAction in action.Combos)
            {
                AddAction(textBox, comboAction, dmgInfo, indent + 1, true);
            }

            if (!String.IsNullOrEmpty(action.PostAction))
            {
                AddNormal(paragraph, " " + ReplaceDamageTokens(action.PostAction, dmgInfo));
            }
        }

        private static void AddRoll(RichTextBox textBox, RollData roll, DamageInfo dmgInfo, int indent = 0)
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
                    if (!String.IsNullOrEmpty(valueStr))
                    {
                        if (lastAddedValue != lastValue)
                        {
                            valueStr += "-" + lastValue;
                        }
                        valueStr +=  ",";
                    }

                    valueStr += value;
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
                        valueStr += ",";
                    }
                    else
                    {
                        valueStr += "-";
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
                AddNormal(paragraph, ReplaceDamageTokens(roll.Description, dmgInfo));
            }

            textBox.Document.Blocks.Add(paragraph);
        }

        private static void AddSummon(RichTextBox textBox, SummonData summon, DamageInfo dmgInfo, int indent = 0)
        {
            if (!String.IsNullOrEmpty(summon.Name) || summon.Tags.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };
                paragraph.TextDecorations = TextDecorations.Underline;

                AddBold(paragraph, summon.Name);

                textBox.Document.Blocks.Add(paragraph);
            }

            if (summon.Tags.Count > 0)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };

                bool first = true;
                foreach (string tag in summon.Tags)
                {
                    if (!first)
                    {
                        AddBold(paragraph, ", ");
                    }
                    else
                    {
                        first = false;
                    }

                    AddBold(paragraph, tag);
                }

                textBox.Document.Blocks.Add(paragraph);
            }

            AddTraits(textBox, summon.GetActualTraits(), dmgInfo, indent);

            foreach (string effect in summon.Effects)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };
                AddBold(paragraph, "Summon Effect: ");
                AddNormal(paragraph, effect);
                textBox.Document.Blocks.Add(paragraph);
            }

            foreach (string action in summon.Actions)
            {
                Paragraph paragraph = MakeParagraph();
                paragraph.Margin = new Thickness() { Left = MARGIN_LEN * indent };
                AddBold(paragraph, "Summon Action: ");
                AddNormal(paragraph, action);
                textBox.Document.Blocks.Add(paragraph);
            }

            foreach (ActionData action in summon.ComplexActions)
            {
                AddAction(textBox, action, dmgInfo, indent, false);
            }

            foreach (ActionData action in summon.SpecialActions)
            {
                AddAction(textBox, action, dmgInfo, indent + 1, false);
            }

            AddInterrupts(textBox, summon.SpecialInterrupts, dmgInfo, indent + 1);
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

        private static void AddPhases(RichTextBox textBox, List<PhaseData> phases, DamageInfo dmgInfo)
        {
            for (int i = 0; i < phases.Count; ++i)
            {
                PhaseData phase = phases[i];
                Paragraph paragraph1 = MakeParagraph();
                paragraph1.Margin = new Thickness(0, 12, 0, 0);
                AddBold(paragraph1, "Phase " + ToRoman(i + 1));
                if (!String.IsNullOrEmpty(phase.Name)) { AddBold(paragraph1, ": " + phase.Name); }
                textBox.Document.Blocks.Add(paragraph1);

                if (phase.Description != null && phase.Description != String.Empty)
                {
                    Paragraph paragraph2 = MakeParagraph();
                    AddNormal(paragraph2, phase.Description);
                    textBox.Document.Blocks.Add(paragraph2);
                }

                if (phase.Traits.Count > 0)
                {
                    AddTraits(textBox, phase.GetActualTraits(), dmgInfo);
                }

                if (phase.Interrupts.Count > 0)
                {
                    AddInterrupts(textBox, phase.Interrupts, dmgInfo);
                }

                if (phase.Actions.Count > 0)
                {
                    AddActions(textBox, phase.Actions, dmgInfo);
                }
            }
        }

        private static void AddExtraAbilitySets(RichTextBox textBox, List<AbilitySetData> abilitySets, DamageInfo dmgInfo)
        {
            for (int i = 0; i < abilitySets.Count; ++i)
            {
                AbilitySetData abilitySet = abilitySets[i];

                Paragraph paragraph1 = MakeParagraph();
                paragraph1.Margin = new Thickness(0, 12, 0, 0);
                paragraph1.TextDecorations = TextDecorations.Underline;
                AddBold(paragraph1, abilitySet.Name);
                textBox.Document.Blocks.Add(paragraph1);

                if (!String.IsNullOrEmpty(abilitySet.Description))
                {
                    Paragraph paragraph2 = MakeParagraph();
                    AddNormal(paragraph2, abilitySet.Description);
                    textBox.Document.Blocks.Add(paragraph2);
                }

                if (abilitySet.Traits.Count > 0)
                {
                    AddTraits(textBox, abilitySet.GetActualTraits(), dmgInfo);
                }

                if (abilitySet.Interrupts.Count > 0)
                {
                    AddInterrupts(textBox, abilitySet.Interrupts, dmgInfo);
                }

                if (abilitySet.Actions.Count > 0)
                {
                    AddActions(textBox, abilitySet.Actions, dmgInfo);
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

        private static string ReplaceDamageTokens(string text, DamageInfo dmgInfo)
        {
            if (!dmgInfo.replaceDesc)
            {
                return text;
            }

            var tokens = new Dictionary<string, string>
            {
                { "[D]", "d" + dmgInfo.damageDie.ToString() },
                { "[fray]", dmgInfo.fray.ToString() }
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
