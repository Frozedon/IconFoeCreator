using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace IconFoeCreator
{
    public partial class MainWindow : Window
    {
        public static readonly string EMPTY_STAT = "...";
        public static readonly string ANY_GROUP = "Any";

        private StatisticBuilder statBuilder;
        private DropdownOptions dropdownOptions;

        public MainWindow()
        {
            InitializeComponent();

            statBuilder = new StatisticBuilder();
            statBuilder.BuildStatistics(false);

            dropdownOptions = new DropdownOptions();
            dropdownOptions.ConvertToDropdownItems(statBuilder);

            UpdateChapterOptions();
            UpdateFactionOptions();
            UpdateClassOptions();
            UpdateSpecialClassOptions();
            UpdateFoeOptions();
            UpdateTemplateOptions();
            UpdateSpecialTemplateOptions();

            UpdateTemplateDropdownState();
            UpdateSpecialTemplateDropdownState();
            UpdateInvalidChapterLabelState();

            Chapter_comboBox.SelectionChanged += OnChapterChanged;
            Faction_comboBox.SelectionChanged += OnFactionChanged;
            Class_comboBox.SelectionChanged += OnClassChanged;
            SpecialClass_comboBox.SelectionChanged += OnSpecialClassChanged;
            Foe_comboBox.SelectionChanged += OnFoeChanged;
            Template_comboBox.SelectionChanged += OnTemplateChanged;
            SpecialTemplate_comboBox.SelectionChanged += OnSpecialTemplateChanged;

            UpdateDescription();
        }

        private void OnChapterChanged(object sender, EventArgs e)
        {
            UpdateInvalidChapterLabelState();
            UpdateFoeOptions();
            UpdateDescription();
        }

        private void OnFactionChanged(object sender, EventArgs e)
        {
            UpdateFoeOptions();
            UpdateTemplateOptions();
        }

        private void OnClassChanged(object sender, EventArgs e)
        {
            UpdateFoeOptions();
        }

        private void OnSpecialClassChanged(object sender, EventArgs e)
        {
            UpdateFoeOptions();
        }

        private void OnFoeChanged(object sender, EventArgs e)
        {
            UpdateTemplateDropdownState();
            UpdateInvalidChapterLabelState();
            UpdateSpecialTemplateDropdownState();
            UpdateSpecialTemplateOptions();
            UpdateMobCheckBoxState();
            UpdateEliteCheckBoxState();
            UpdateDescription();
        }

        private void OnTemplateChanged(object sender, EventArgs e)
        {
            UpdateInvalidChapterLabelState();
            UpdateSpecialTemplateDropdownState();
            UpdateSpecialTemplateOptions();
            UpdateMobCheckBoxState();
            UpdateEliteCheckBoxState();
            UpdateDescription();
        }

        private void OnSpecialTemplateChanged(object sender, EventArgs e)
        {
            UpdateDescription();
        }

        private Statistics GetComboBoxStats(object item)
        {
            if (item != null)
            {
                ComboBoxItem templateItem = (ComboBoxItem)item;
                return (Statistics)templateItem.Content;
            }

            return null;
        }

        private string GetComboBoxString(object item)
        {
            if (item != null)
            {
                ComboBoxItem templateItem = (ComboBoxItem)item;
                return (string)templateItem.Content;
            }

            return null;
        }

        private int GetComboBoxInt(object item)
        {
            if (item != null)
            {
                ComboBoxItem templateItem = (ComboBoxItem)item;
                return (int)templateItem.Content;
            }

            return 1;
        }

        private void UpdateDescription()
        {
            Statistics stats = MakeCompiledStats();

            if (Statistics.IsValid(stats))
            {
                DescriptionCreator.UpdateDescription(
                    Description_richTextBox,
                    Setup_richTextBox,
                    stats,
                    DamageValues_checkBox.IsChecked.GetValueOrDefault());
            }
            else
            {
                Description_richTextBox.Document.Blocks.Clear();
                Setup_richTextBox.Document.Blocks.Clear();

                /*Paragraph paragraph = new Paragraph()
                {
                    Margin = new Thickness(0)
                };
                paragraph.Inlines.Add(new Bold(new Run("Traits Missing Entries")));
                foreach (string trait in statBuilder.Debug_CheckForTraitsExisting())
                {
                    paragraph.Inlines.Add(new Run("\n" + trait));
                }

                paragraph.Inlines.Add(new Bold(new Run("\nTrait Entries Doubled")));
                foreach (string trait in statBuilder.Debug_CheckForDoubleTraits())
                {
                    paragraph.Inlines.Add(new Run("\n" + trait));
                }

                Description_richTextBox.Document.Blocks.Add(paragraph);*/
            }
        }

        private Statistics MakeCompiledStats(bool includeSpecialTemplate = true, bool includeMobEliteTemplate = true)
        {
            List<Statistics> statsToMerge = new List<Statistics>()
            {
                GetComboBoxStats(Foe_comboBox.SelectedItem),
                GetComboBoxStats(Template_comboBox.SelectedItem)
            };
            
            if (includeSpecialTemplate)
            {
                statsToMerge.Add(GetComboBoxStats(SpecialTemplate_comboBox.SelectedItem));
            }

            if (includeMobEliteTemplate)
            {
                if (Mob_checkBox.IsChecked.GetValueOrDefault(false))
                {
                    Statistics mobTemplate = statBuilder.SpecialTemplates.Find(stat => stat.Name.ToLower() == "mob template");
                    if (mobTemplate != null)
                    {
                        statsToMerge.Add(mobTemplate);
                    }
                }
                else if (Elite_checkBox.IsChecked.GetValueOrDefault(false))
                {
                    Statistics eliteTemplate = statBuilder.SpecialTemplates.Find(stat => stat.Name.ToLower() == "elite template");
                    if (eliteTemplate != null)
                    {
                        statsToMerge.Add(eliteTemplate);
                    }
                }
            }

            Statistics compiledStats = new Statistics();
            string compiledName = String.Empty;

            bool valid = false;
            foreach (Statistics stat in statsToMerge)
            {
                if (stat != null && Statistics.IsValid(stat))
                {
                    valid = true;

                    compiledStats = stat.InheritFrom(compiledStats);

                    if (String.IsNullOrEmpty(compiledName))
                    {
                        compiledName = stat.GetDisplayName();
                    }
                    else
                    {
                        compiledName = stat.GetDisplayName() + " " + compiledName;
                    }
                }
            }

            if (!valid)
            {
                return null;
            }

            int chapter = GetComboBoxInt(Chapter_comboBox.SelectedItem);

            compiledStats.ProcessData(statBuilder.Traits, chapter);
            compiledStats.DisplayName = compiledName;

            return compiledStats;
        }

        private void UpdateChapterOptions()
        {
            UpdateDropdownOptions(Chapter_comboBox, GetAvailableChapters);
        }

        private void UpdateFactionOptions()
        {
            UpdateDropdownOptions(Faction_comboBox, GetAvailableFactions);
        }

        private void UpdateClassOptions()
        {
            UpdateDropdownOptions(Class_comboBox, GetAvailableClasses);
        }

        private void UpdateSpecialClassOptions()
        {
            UpdateDropdownOptions(SpecialClass_comboBox, GetAvailableSpecialClasses);
        }

        private void UpdateFoeOptions()
        {
            UpdateDropdownOptions(Foe_comboBox, GetAvailableFoes);
        }

        private void UpdateTemplateOptions()
        {
            UpdateDropdownOptions(Template_comboBox, GetAvailableTemplates);
        }

        private void UpdateSpecialTemplateOptions()
        {
            UpdateDropdownOptions(SpecialTemplate_comboBox, GetAvailableSpecialTemplates);
        }

        private void UpdateDropdownOptions<T>(System.Windows.Controls.ComboBox comboBox, Func<List<T>> getStats)
        {
            string selectedItem = String.Empty;
            if (comboBox.SelectedItem != null)
            {
                selectedItem = comboBox.SelectedItem.ToString();
            }

            comboBox.ItemsSource = getStats();

            int index = 0;
            if (selectedItem.Length > 0)
            {
                for (int i = 0; i < comboBox.Items.Count; ++i)
                {
                    if (selectedItem == comboBox.Items[i].ToString())
                    {
                        index = i;
                        break;
                    }
                }
            }
            comboBox.SelectedIndex = index;
        }

        private void UpdateTemplateDropdownState()
        {
            // Disable if the unique foe has been chosen
            Statistics selectedFoe = GetComboBoxStats(Foe_comboBox.SelectedItem);
            if (Statistics.IsValid(selectedFoe) && selectedFoe.UsesTemplate)
            {
                Template_comboBox.IsEnabled = true;
            }
            else
            {
                Template_comboBox.SelectedIndex = 0;
                Template_comboBox.IsEnabled = false;
            }
        }

        private void UpdateSpecialTemplateDropdownState()
        {
            // Disable if the unique foe has been chosen
            Statistics selectedFoe = MakeCompiledStats(false, true);
            if (Statistics.IsValid(selectedFoe) && selectedFoe.UsesSpecialTemplates.Count > 0)
            {
                SpecialTemplate_comboBox.IsEnabled = true;
            }
            else
            {
                SpecialTemplate_comboBox.SelectedIndex = 0;
                SpecialTemplate_comboBox.IsEnabled = false;
            }
        }

        private void UpdateInvalidChapterLabelState()
        {
            Statistics selectedFoe = GetComboBoxStats(Foe_comboBox.SelectedItem);
            int chapter = GetComboBoxInt(Chapter_comboBox.SelectedItem);
            if (selectedFoe != null && selectedFoe.ToString() != EMPTY_STAT && selectedFoe.Chapter > chapter)
            {
                InvalidChapter_label.Visibility = Visibility.Visible;
            }
            else
            {
                InvalidChapter_label.Visibility = Visibility.Hidden;
            }
        }
        private void UpdateMobCheckBoxState()
        {
            Statistics mobTemplate = statBuilder.SpecialTemplates.Find(stat => stat.Name.ToLower() == "mob template");

            // Unchecked if the elite check box is checked
            if (Elite_checkBox.IsChecked.GetValueOrDefault(false) || mobTemplate == null)
            {
                Mob_checkBox.IsChecked = false;
                Mob_checkBox.IsEnabled = false;
            }
            else
            {
                // Disable if special class is not normal
                Statistics selectedFoe = MakeCompiledStats(false, false);
                if (Statistics.IsValid(selectedFoe) && (String.IsNullOrEmpty(selectedFoe.SpecialClass) || selectedFoe.SpecialClass.ToLower() == "normal"))
                {
                    Mob_checkBox.IsEnabled = true;
                }
                else
                {
                    Mob_checkBox.IsEnabled = false;
                }
            }
        }

        private void UpdateEliteCheckBoxState()
        {
            Statistics eliteTemplate = statBuilder.SpecialTemplates.Find(stat => stat.Name.ToLower() == "elite template");

            // Unchecked if the mob check box is checked
            if (Mob_checkBox.IsChecked.GetValueOrDefault(false) || eliteTemplate == null)
            {
                Elite_checkBox.IsChecked = false;
                Elite_checkBox.IsEnabled = false;
            }
            else
            {
                // Disable if special class is not normal
                Statistics selectedFoe = MakeCompiledStats(false, false);
                if (Statistics.IsValid(selectedFoe) && (String.IsNullOrEmpty(selectedFoe.SpecialClass) || selectedFoe.SpecialClass.ToLower() == "normal"))
                {
                    Elite_checkBox.IsEnabled = true;
                }
                else
                {
                    Elite_checkBox.IsEnabled = false;
                }
            }
        }

        private List<ComboBoxItem> GetAvailableChapters()
        {
            List<ComboBoxItem> availableChapters = new List<ComboBoxItem>();

            availableChapters.Add(new ComboBoxItem() { Content = 1 });
            availableChapters.Add(new ComboBoxItem() { Content = 2 });
            availableChapters.Add(new ComboBoxItem() { Content = 3 });

            return availableChapters;
        }

        private List<ComboBoxItem> GetAvailableFactions()
        {
            List<ComboBoxItem> availableFactions = new List<ComboBoxItem>(dropdownOptions.Factions);

            availableFactions.Insert(0, new ComboBoxItem() { Content = ANY_GROUP });

            return availableFactions;
        }

        private List<ComboBoxItem> GetAvailableClasses()
        {
            List<ComboBoxItem> availableClasses = new List<ComboBoxItem>(dropdownOptions.Classes);

            availableClasses.Insert(0, new ComboBoxItem() { Content = ANY_GROUP });

            return availableClasses;
        }

        private List<ComboBoxItem> GetAvailableSpecialClasses()
        {
            List<ComboBoxItem> availableSpecialClasses = new List<ComboBoxItem>(dropdownOptions.SpecialClasses);

            availableSpecialClasses.Insert(0, new ComboBoxItem() { Content = ANY_GROUP });

            return availableSpecialClasses;
        }

        private List<ComboBoxItem> GetAvailableFoes()
        {
            List<ComboBoxItem> availableFoes = new List<ComboBoxItem>(dropdownOptions.Foes);

            string selectedFaction = GetComboBoxString(Faction_comboBox.SelectedItem);
            if (selectedFaction != null && !String.IsNullOrEmpty(selectedFaction) && selectedFaction.ToLower() != ANY_GROUP.ToLower())
            {
                availableFoes = RemoveStatsOfOtherFactions(availableFoes, selectedFaction);
            }

            string selectedClass = GetComboBoxString(Class_comboBox.SelectedItem);
            if (selectedClass != null && !String.IsNullOrEmpty(selectedClass) && selectedClass.ToLower() != ANY_GROUP.ToLower())
            {
                availableFoes = RemoveStatsOfOtherClasses(availableFoes, selectedClass);
            }

            string selectedSpecialClass = GetComboBoxString(SpecialClass_comboBox.SelectedItem);
            if (selectedSpecialClass != null && !String.IsNullOrEmpty(selectedSpecialClass) && selectedSpecialClass.ToLower() != ANY_GROUP.ToLower())
            {
                availableFoes = RemoveStatsOfOtherSpecialClasses(availableFoes, selectedSpecialClass);
            }

            /*int chapter = GetComboBoxInt(Chapter_comboBox.SelectedItem);
            if (chapter > 0)
            {
                availableFoes = RemoveStatsOfHigherChapter(availableFoes, chapter);
            }*/

            availableFoes.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableFoes;
        }

        private List<ComboBoxItem> GetAvailableTemplates()
        {
            List<ComboBoxItem> availableTemplates = new List<ComboBoxItem>(dropdownOptions.Templates);

            availableTemplates.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableTemplates;
        }

        private List<ComboBoxItem> GetAvailableSpecialTemplates()
        {
            List<ComboBoxItem> availableSpecialTemplates = new List<ComboBoxItem>(dropdownOptions.SpecialTemplates);

            Statistics compiledStats = MakeCompiledStats(false, true);
            if (Statistics.IsValid(compiledStats))
            {
                availableSpecialTemplates = RemoveStatsOfInvalidSpecialTemplates(availableSpecialTemplates, compiledStats.UsesSpecialTemplates);
            }

            availableSpecialTemplates.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableSpecialTemplates;
        }

        private List<ComboBoxItem> RemoveStatsOfOtherFactions(List<ComboBoxItem> stats, string factionName)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;

                if (factionName.ToLower() == StatisticBuilder.FACTION_BASIC_READABLE.ToLower())
                {
                    return String.IsNullOrEmpty(actualStat.Faction) || actualStat.Faction.ToLower() == StatisticBuilder.FACTION_BASIC;
                }
                else
                {
                    return !String.IsNullOrEmpty(actualStat.Faction) && actualStat.Faction.ToLower() == factionName.ToLower();
                }
            });
        }

        private List<ComboBoxItem> RemoveStatsOfOtherClasses(List<ComboBoxItem> stats, string className)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;
                return !String.IsNullOrEmpty(actualStat.Class) && actualStat.Class.ToLower() == className.ToLower();
            });
        }

        private List<ComboBoxItem> RemoveStatsOfOtherSpecialClasses(List<ComboBoxItem> stats, string specialClassName)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;

                if (specialClassName.ToLower() == StatisticBuilder.SPECIAL_CLASS_NORMAL)
                {
                    return String.IsNullOrEmpty(actualStat.SpecialClass) || actualStat.SpecialClass.ToLower() == StatisticBuilder.SPECIAL_CLASS_NORMAL;
                }
                else
                {
                    return !String.IsNullOrEmpty(actualStat.SpecialClass) && actualStat.SpecialClass.ToLower() == specialClassName.ToLower();
                }
            });
        }

        /*private List<ComboBoxItem> RemoveStatsOfHigherChapter(List<ComboBoxItem> stats, int chapter)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;
                return actualStat.Chapter <= chapter;
            });
        }*/

        private List<ComboBoxItem> RemoveStatsOfInvalidSpecialTemplates(List<ComboBoxItem> stats, List<string> specialTemplates)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;

                foreach (string validSpecial in specialTemplates)
                {
                    return validSpecial.ToLower() == actualStat.Name.ToLower();
                }

                return false;
            });
        }

        private void CopyDescription_button_Click(object sender, RoutedEventArgs e)
        {
            Description_richTextBox.SelectAll();
            Description_richTextBox.Copy();
        }

        private void CopySetup_button_Click(object sender, RoutedEventArgs e)
        {
            Setup_richTextBox.SelectAll();
            Setup_richTextBox.Copy();
        }

        private void Mob_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateEliteCheckBoxState();
            UpdateTemplateOptions();
            UpdateDescription();
        }

        private void Elite_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateMobCheckBoxState();
            UpdateTemplateOptions();
            UpdateDescription();
        }

        private void DamageValues_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDescription();
        }

        private void Homebrew_checkBox_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void ExportJson_button_Click(object sender, RoutedEventArgs e)
        {
            Statistics stats = MakeCompiledStats();

            if (Statistics.IsValid(stats))
            {
                if (!Directory.Exists("export"))
                {
                    Directory.CreateDirectory("export");
                }

                string text = JsonConvert.SerializeObject(stats);
                File.WriteAllText($"export/{stats.Name}.json", text);
            }
        }
    }
}
