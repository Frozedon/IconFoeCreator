using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace IconFoeCreator
{
    public partial class MainWindow : Window
    {
        public static readonly string EMPTY_STAT = "...";
        public static readonly string ANY_GROUP = "Any";

        StatisticBuilder statBuilder;
        DropdownOptions dropdownOptions;

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

            UpdateTemplateDropdownState();

            Chapter_comboBox.SelectionChanged += OnChapterChanged;
            Faction_comboBox.SelectionChanged += OnFactionChanged;
            Class_comboBox.SelectionChanged += OnClassChanged;
            SpecialClass_comboBox.SelectionChanged += OnSpecialClassChanged;
            Foe_comboBox.SelectionChanged += OnFoeChanged;
            Template_comboBox.SelectionChanged += OnTemplateChanged;

            UpdateDescription();
        }

        private void OnChapterChanged(object sender, EventArgs e)
        {
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
            UpdateDescription();
        }

        private void OnTemplateChanged(object sender, EventArgs e)
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
            List<Statistics> statsToMerge = new List<Statistics>();

            Statistics foe = GetComboBoxStats(Foe_comboBox.SelectedItem);
            if (foe != null && Statistics.IsValid(foe))
            {
                statsToMerge.Add(foe);
            }

            int chapter = GetComboBoxInt(Chapter_comboBox.SelectedItem);

            DescriptionCreator.UpdateDescription(
                Description_richTextBox,
                Setup_richTextBox,
                statBuilder.Traits,
                statsToMerge,
                chapter,
                DamageValues_checkBox.IsChecked.GetValueOrDefault());
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
            if (selectedFoe != null && selectedFoe.ToString() != EMPTY_STAT && selectedFoe.UsesTemplate)
            {
                Template_comboBox.IsEnabled = true;
            }
            else
            {
                Template_comboBox.SelectedIndex = 0;
                Template_comboBox.IsEnabled = false;
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

            int chapter = GetComboBoxInt(Chapter_comboBox.SelectedItem);
            if (chapter > 0)
            {
                availableFoes = RemoveStatsOfHigherChapter(availableFoes, chapter);
            }

            availableFoes.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableFoes;
        }

        private List<ComboBoxItem> GetAvailableTemplates()
        {
            List<ComboBoxItem> availableTemplates = new List<ComboBoxItem>(dropdownOptions.Templates);

            availableTemplates.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableTemplates;
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

        private List<ComboBoxItem> RemoveStatsOfHigherChapter(List<ComboBoxItem> stats, int chapter)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;
                return actualStat.Chapter <= chapter;
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

        private void DamageValues_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDescription();
        }

        private void Homebrew_checkBox_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void ExportJson_button_Click(object sender, RoutedEventArgs e)
        {
            List<Statistics> statsToMerge = new List<Statistics>();

            Statistics foe = GetComboBoxStats(Foe_comboBox.SelectedItem);
            if (foe != null && Statistics.IsValid(foe))
            {
                statsToMerge.Add(foe);
            }

            Statistics statsToExport = new Statistics();
            string name = String.Empty;
            foreach (Statistics stats in statsToMerge)
            {
                statsToExport = stats.InheritFrom(statsToExport);
                name = stats.Name + " " + name;
            }

            if (!Directory.Exists("export"))
            {
                Directory.CreateDirectory("export");
            }

            string text = JsonConvert.SerializeObject(statsToExport);
            File.WriteAllText($"export/{name}.json", text);
        }
    }
}
