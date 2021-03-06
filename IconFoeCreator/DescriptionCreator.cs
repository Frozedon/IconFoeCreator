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
            if (stats.DoubleHP.GetValueOrDefault(false) ||
                (stats.DoubleHPIfNormalFoe.GetValueOrDefault(false)
                && !stats.IsMob.GetValueOrDefault()
                && !stats.IsElite.GetValueOrDefault()
                && !stats.IsLegend.GetValueOrDefault()))
            {
                hitPoints *= 2;
            }

            int speed = stats.Speed.GetValueOrDefault();
            int dash = stats.Dash.GetValueOrDefault();
            string dashText = dash <= 0 ? "No Dash" : ("Dash " + dash);
            int defense = stats.Defense.GetValueOrDefault();
            int armor = stats.Armor.GetValueOrDefault();
            int minimumRecharge = stats.MinimumRecharge.GetValueOrDefault(0);

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

                AddTraits(descTextBox, stats.Traits, damageInfo, showNonessentialTraits);
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

                AddActions(descTextBox, stats.Actions, damageInfo, minimumRecharge);
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

                AddPhases(descTextBox, stats.Phases, damageInfo, minimumRecharge);
            }

            if (stats.ExtraAbilitySets.Count > 0)
            {
                AddExtraAbilitySets(descTextBox, stats.ExtraAbilitySets, damageInfo, minimumRecharge);
            }

            // Setup traits in other textbox
            AddEncounterBudget(setupTextBox, stats.EncounterBudget.GetValueOrDefault(1.0));

            if (stats.SetupTraits.Count > 0)
            {
                AddTraits(setupTextBox, stats.SetupTraits, damageInfo, true);
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

        private static void AddTraits(RichTextBox textBox, List<Trait> traits, DamageInfo dmgInfo, bool showNonessentialTraits, int indent = 0)
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
                    AddNormal(paragraph, ReplaceDamageTokens(trait.Description, dmgInfo));

                    if (showNonessentialTraits)
                    {
                        AddNormal(paragraph, " " + trait.DescriptionNonessential);
                    }

                    textBox.Document.Blocks.Add(paragraph);
                }
            }
        }

        private static void AddInterrupts(RichTextBox textBox, List<Interrupt> interrupts, DamageInfo dmgInfo)
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
                    AddNormal(paragraph, " " + ReplaceDamageTokens(interrupt.Description, dmgInfo));
                }

                if (!String.IsNullOrEmpty(interrupt.Trigger))
                {
                    AddItalic(paragraph, " Trigger: " );
                    AddNormal(paragraph, ReplaceDamageTokens(interrupt.Trigger, dmgInfo));
                }

                if (!String.IsNullOrEmpty(interrupt.Effect))
                {
                    AddItalic(paragraph, " Effect: ");
                    AddNormal(paragraph, ReplaceDamageTokens(interrupt.Effect, dmgInfo));
                }

                if (!String.IsNullOrEmpty(interrupt.Collide))
                {
                    AddItalic(paragraph, " Collide: ");
                    AddNormal(paragraph, ReplaceDamageTokens(interrupt.Collide, dmgInfo));
                }

                textBox.Document.Blocks.Add(paragraph);
            }
        }

        private static void AddActions(RichTextBox textBox, List<Action> actions, DamageInfo dmgInfo, int minRecharge)
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
                AddAction(textBox, action, dmgInfo, minRecharge);
            }
        }

        private static void AddAction(RichTextBox textBox, Action action, DamageInfo dmgInfo, int minRecharge, bool combo = false)
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
            else if (action.ActionCost == 0)
            {
                AddBold(paragraph, "Free action");
            }

            foreach (string tag in action.Tags)
            {
                AddBold(paragraph, ", " + tag);
            }

            if (action.Recharge > 1)
            {
                int recharge = Math.Max(minRecharge, action.Recharge);
                AddBold(paragraph, ", recharge " + recharge);

                if (recharge < 6)
                {
                    AddBold(paragraph, "+");
                }
            }

            AddBold(paragraph, "):");

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

            if (!String.IsNullOrEmpty(action.Critical))
            {
                AddItalic(paragraph, " Critical hit: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Critical, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.Miss))
            {
                AddItalic(paragraph, " Miss: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Miss, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.PostAttack))
            {
                AddNormal(paragraph, " " + ReplaceDamageTokens(action.PostAttack, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.AreaEffect))
            {
                AddItalic(paragraph, " Area Effect: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.AreaEffect, dmgInfo));
            }

            if (!String.IsNullOrEmpty(action.Effect))
            {
                AddItalic(paragraph, " Effect: ");
                AddNormal(paragraph, ReplaceDamageTokens(action.Effect, dmgInfo));
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

            textBox.Document.Blocks.Add(paragraph);

            foreach (Action comboAction in action.Combos)
            {
                AddAction(textBox, comboAction, dmgInfo, minRecharge, true);
            }

            if (!String.IsNullOrEmpty(action.PostAction))
            {
                AddNormal(paragraph, " " + ReplaceDamageTokens(action.PostAction, dmgInfo));
            }
        }

        private static void AddBodyParts(RichTextBox textBox, List<BodyPart> bodyParts)
        {
            foreach (BodyPart bodyPart in bodyParts)
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

        private static void AddPhases(RichTextBox textBox, List<Phase> phases, DamageInfo dmgInfo, int minRecharge)
        {
            for (int i = 0; i < phases.Count; ++i)
            {
                Phase phase = phases[i];
                Paragraph paragraph1 = MakeParagraph();
                paragraph1.Margin = new Thickness(0, 12, 0, 0);
                AddBold(paragraph1, "Phase " + (i + 1) + ": " + phase.Name);
                textBox.Document.Blocks.Add(paragraph1);

                if (phase.Description != null && phase.Description != String.Empty)
                {
                    Paragraph paragraph2 = MakeParagraph();
                    AddNormal(paragraph2, phase.Description);
                    textBox.Document.Blocks.Add(paragraph2);
                }

                if (phase.Traits.Count > 0)
                {
                    AddTraits(textBox, phase.Traits, dmgInfo, true);
                }

                if (phase.Actions.Count > 0)
                {
                    AddActions(textBox, phase.Actions, dmgInfo, minRecharge);
                }
            }
        }

        private static void AddExtraAbilitySets(RichTextBox textBox, List<AbilitySet> abilitySets, DamageInfo dmgInfo, int minRecharge)
        {
            for (int i = 0; i < abilitySets.Count; ++i)
            {
                AbilitySet abilitySet = abilitySets[i];

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
                    AddTraits(textBox, abilitySet.Traits, dmgInfo, true);
                }

                if (abilitySet.Actions.Count > 0)
                {
                    AddActions(textBox, abilitySet.Actions, dmgInfo, minRecharge);
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
    }
}
