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
    public static class Constants
    {
        public const int ChapterCount = 3;
    }

    public class ChapterItem
    {
        public int Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public partial class MainWindow : Window
    {
        public static readonly string UNIQUE_CLASS = "unique";
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

            UpdateFactionOptions();
            UpdateClassOptions();
            UpdateFoeOptions();

            Faction_comboBox.SelectionChanged += OnFactionChanged;
            Class_comboBox.SelectionChanged += OnClassChanged;
            Foe_comboBox.SelectionChanged += OnFoeChanged;

            UpdateDescription();
        }

        private void OnFactionChanged(object sender, EventArgs e)
        {
            UpdateFoeOptions();
        }

        private void OnClassChanged(object sender, EventArgs e)
        {
            UpdateFoeOptions();
        }

        private void OnFoeChanged(object sender, EventArgs e)
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

        private void UpdateDescription()
        {
            List<Statistics> statsToMerge = new List<Statistics>();

            Statistics foe = GetComboBoxStats(Foe_comboBox.SelectedItem);
            if (foe != null && Statistics.IsValid(foe))
            {
                statsToMerge.Add(foe);
            }

            DescriptionCreator.UpdateDescription(
                Description_richTextBox,
                Setup_richTextBox,
                statBuilder.Traits,
                statsToMerge,
                DamageValues_checkBox.IsChecked.GetValueOrDefault());
        }

        private void UpdateFactionOptions()
        {
            UpdateDropdownOptions(Faction_comboBox, GetAvailableFactions);
        }

        private void UpdateClassOptions()
        {
            UpdateDropdownOptions(Class_comboBox, GetAvailableClasses);
        }

        private void UpdateFoeOptions()
        {
            UpdateDropdownOptions(Foe_comboBox, GetAvailableFoes);
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

        private List<ComboBoxItem> GetAvailableFoes()
        {
            List<ComboBoxItem> availableTemplates = new List<ComboBoxItem>(dropdownOptions.Foes);

            string selectedFaction = GetComboBoxString(Faction_comboBox.SelectedItem);
            if (selectedFaction != null && !String.IsNullOrEmpty(selectedFaction) && selectedFaction.ToLower() != ANY_GROUP.ToLower())
            {
                availableTemplates = RemoveStatsOfOtherFactions(availableTemplates, selectedFaction);
            }

            string selectedClass = GetComboBoxString(Class_comboBox.SelectedItem);
            if (selectedClass != null && !String.IsNullOrEmpty(selectedClass) && selectedClass.ToLower() != ANY_GROUP.ToLower())
            {
                availableTemplates = RemoveStatsOfOtherClasses(availableTemplates, selectedClass);
            }

            availableTemplates.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableTemplates;
        }

        private List<ComboBoxItem> RemoveStatsOfOtherFactions(List<ComboBoxItem> stats, string groupName)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;
                return !String.IsNullOrEmpty(actualStat.Faction) && actualStat.Faction.ToLower() == groupName.ToLower();
            });
        }

        private List<ComboBoxItem> RemoveStatsOfOtherClasses(List<ComboBoxItem> stats, string groupName)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;
                return !String.IsNullOrEmpty(actualStat.Class) && actualStat.Class.ToLower() == groupName.ToLower();
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
