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
            statBuilder.BuildStatistics();

            dropdownOptions = new DropdownOptions();
            dropdownOptions.ConvertToDropdownItems(statBuilder);

            UpdateFactionOptions();
            UpdateTemplateOptions();
            UpdateUniqueFoeOptions();
            UpdateSpecialTemplateOptions();
            UpdateClassOptions();
            UpdateJobOptions();

            Faction_comboBox.SelectionChanged += OnFactionChanged;
            Template_comboBox.SelectionChanged += OnTemplateChanged;
            UniqueFoe_comboBox.SelectionChanged += OnUniqueFoeChanged;
            Special_comboBox.SelectionChanged += OnSpecialTemplateChanged;
            Class_comboBox.SelectionChanged += OnClassChanged;
            Job_comboBox.SelectionChanged += OnJobChanged;

            UpdateDescription();
        }

        private void OnFactionChanged(object sender, EventArgs e)
        {
            UpdateTemplateOptions();
            UpdateUniqueFoeOptions();
            UpdateSpecialTemplateOptions();
        }

        private void OnTemplateChanged(object sender, EventArgs e)
        {
            UpdateUniqueFoeDropdownState();
            UpdateDescription();
        }

        private void OnUniqueFoeChanged(object sender, EventArgs e)
        {
            UpdateTemplateDropdownState();
            UpdateJobDropdownState();
            UpdateDescription();
        }

        private void OnSpecialTemplateChanged(object sender, EventArgs e)
        {
            UpdateDescription();
        }

        private void OnClassChanged(object sender, EventArgs e)
        {
            UpdateTemplateOptions();
            UpdateUniqueFoeOptions();
            UpdateJobOptions();
            UpdateMobCheckBoxState();
            UpdateEliteCheckBoxState();
        }

        private void OnJobChanged(object sender, EventArgs e)
        {
            UpdateUniqueFoeDropdownState();
            UpdateTemplateOptions();
            UpdateMobCheckBoxState();
            UpdateEliteCheckBoxState();

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

        private Group GetComboBoxGroup(object item)
        {
            if (item != null)
            {
                ComboBoxItem templateItem = (ComboBoxItem)item;
                return (Group)templateItem.Content;
            }

            return null;
        }

        private void UpdateDescription()
        {
            List<Statistics> statsToMerge = new List<Statistics>();

            Statistics job = GetComboBoxStats(Job_comboBox.SelectedItem);
            if (job != null && Statistics.IsValid(job))
            {
                statsToMerge.Add(job);
            }

            Statistics template = GetComboBoxStats(Template_comboBox.SelectedItem);
            if (template != null && Statistics.IsValid(template))
            {
                statsToMerge.Add(template);
            }

            Statistics uniqueFoe = GetComboBoxStats(UniqueFoe_comboBox.SelectedItem);
            if (uniqueFoe != null && Statistics.IsValid(uniqueFoe))
            {
                statsToMerge.Add(uniqueFoe);
            }

            Statistics special = GetComboBoxStats(Special_comboBox.SelectedItem);
            if (special != null && Statistics.IsValid(special))
            {
                statsToMerge.Add(special);
            }

            if (Mob_checkBox.IsChecked.GetValueOrDefault(false))
            {
                Statistics mobTemplate = statBuilder.Specials.Find(stat => stat.Name.ToLower() == "mob (template)");
                if (mobTemplate != null)
                {
                    statsToMerge.Add(mobTemplate);
                }
            }
            else if (Elite_checkBox.IsChecked.GetValueOrDefault(false))
            {
                Statistics eliteTemplate = statBuilder.Specials.Find(stat => stat.Name.ToLower() == "elite (template)");
                if (eliteTemplate != null)
                {
                    statsToMerge.Add(eliteTemplate);
                }
            }

            DescriptionCreator.UpdateDescription(
                Description_richTextBox,
                Setup_richTextBox,
                statsToMerge,
                DamageValues_checkBox.IsChecked.GetValueOrDefault(),
                NonessentialTraits_checkBox.IsChecked.GetValueOrDefault());
        }

        private void UpdateTemplateDropdownState()
        {
            // Disable if the unique foe has been chosen
            Statistics selectedUniqueFoe = GetComboBoxStats(UniqueFoe_comboBox.SelectedItem);
            if (selectedUniqueFoe != null && selectedUniqueFoe.ToString() != EMPTY_STAT)
            {
                Template_comboBox.SelectedIndex = 0;
                Template_comboBox.IsEnabled = false;
            }
            else if (!Template_comboBox.IsEnabled)
            {
                Template_comboBox.IsEnabled = true;
            }
        }

        private void UpdateUniqueFoeDropdownState()
        {
            // Disable if the job or template has been chosen
            // Also disable if the mob or elite check boxes have been checked
            Statistics selectedJob = GetComboBoxStats(Job_comboBox.SelectedItem);
            Statistics selectedTemplate = GetComboBoxStats(Template_comboBox.SelectedItem);
            if ((selectedJob != null && selectedJob.ToString() != EMPTY_STAT)
                || (selectedTemplate != null && selectedTemplate.ToString() != EMPTY_STAT)
                || Mob_checkBox.IsChecked.GetValueOrDefault(false)
                || Elite_checkBox.IsChecked.GetValueOrDefault(false))
            {
                UniqueFoe_comboBox.SelectedIndex = 0;
                UniqueFoe_comboBox.IsEnabled = false;
            }
            else if (!UniqueFoe_comboBox.IsEnabled)
            {
                UniqueFoe_comboBox.IsEnabled = true;
            }
        }

        private void UpdateJobDropdownState()
        {
            // Disable if the unique foe has been chosen
            Statistics selectedUniqueFoe = GetComboBoxStats(UniqueFoe_comboBox.SelectedItem);
            if (selectedUniqueFoe != null && selectedUniqueFoe.ToString() != EMPTY_STAT)
            {
                Job_comboBox.SelectedIndex = 0;
                Job_comboBox.IsEnabled = false;
            }
            else if (!Job_comboBox.IsEnabled)
            {
                Job_comboBox.IsEnabled = true;
            }
        }

        private void UpdateMobCheckBoxState()
        {
            // Uncheck if the elite check box is checked
            if (Elite_checkBox.IsChecked.GetValueOrDefault(false))
            {
                Mob_checkBox.IsChecked = false;
            }

            // Disable if class or job is not core class
            bool enable = true;
            if (Class_comboBox.SelectedItem != null)
            {
                Group selectedClass = GetComboBoxGroup(Class_comboBox.SelectedItem);
                if (selectedClass != null && !String.IsNullOrEmpty(selectedClass.Name))
                {
                    string className = selectedClass.Name.ToLower();
                    if (className != ANY_GROUP.ToLower() && Array.Find(StatisticBuilder.CORE_CLASSES, name => name == className) == null)
                    {
                        enable = false;
                    }
                }
            }
            if (Job_comboBox.SelectedItem != null)
            {
                Statistics selectedJob = GetComboBoxStats(Job_comboBox.SelectedItem);
                if (!String.IsNullOrEmpty(selectedJob.Group))
                {
                    string groupName = selectedJob.Group.ToLower();
                    if (Array.Find(StatisticBuilder.CORE_CLASSES, name => name == groupName) == null)
                    {
                        enable = false;
                    }
                }
            }

            Mob_checkBox.IsEnabled = enable;
            if (!enable) { Mob_checkBox.IsChecked = false; }
        }

        private void UpdateEliteCheckBoxState()
        {
            // Uncheck if the mob check box is checked
            if (Mob_checkBox.IsChecked.GetValueOrDefault(false))
            {
                Elite_checkBox.IsChecked = false;
            }

            // Disable if class or job is not core class
            bool enable = true;
            Group selectedClass = GetComboBoxGroup(Class_comboBox.SelectedItem);
            if (selectedClass != null && !String.IsNullOrEmpty(selectedClass.Name))
            {
                string className = selectedClass.Name.ToLower();
                if (className != ANY_GROUP.ToLower() && Array.Find(StatisticBuilder.CORE_CLASSES, name => name == className) == null)
                {
                    enable = false;
                }
            }

            Statistics selectedJob = GetComboBoxStats(Job_comboBox.SelectedItem);
            if (selectedJob != null && !String.IsNullOrEmpty(selectedJob.Group))
            {
                string groupName = selectedJob.Group.ToLower();
                if (Array.Find(StatisticBuilder.CORE_CLASSES, name => name == groupName) == null)
                {
                    enable = false;
                }
            }

            Elite_checkBox.IsEnabled = enable;
            if (!enable) { Elite_checkBox.IsChecked = false; }
        }

        private void UpdateFactionOptions()
        {
            UpdateDropdownOptions(Faction_comboBox, GetAvailableFactions);
        }

        private void UpdateTemplateOptions()
        {
            UpdateDropdownOptions(Template_comboBox, GetAvailableTemplates);
        }

        private void UpdateUniqueFoeOptions()
        {
            UpdateDropdownOptions(UniqueFoe_comboBox, GetAvailableUniqueFoes);
        }

        private void UpdateSpecialTemplateOptions()
        {
            UpdateDropdownOptions(Special_comboBox, GetAvailableSpecialTemplates);
        }

        private void UpdateClassOptions()
        {
            UpdateDropdownOptions(Class_comboBox, GetAvailableClasses);
        }

        private void UpdateJobOptions()
        {
            UpdateDropdownOptions(Job_comboBox, GetAvailableJobs);
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

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableFactions = RemoveHomebrewGroups(availableFactions);
            }

            availableFactions.Insert(0, new ComboBoxItem() { Content = new Group() { Name = ANY_GROUP, OnlyHomebrew = false } });

            return availableFactions;
        }

        private List<ComboBoxItem> GetAvailableTemplates()
        {
            List<ComboBoxItem> availableTemplates = new List<ComboBoxItem>(dropdownOptions.Templates);

            Group selectedFaction = GetComboBoxGroup(Faction_comboBox.SelectedItem);
            if (selectedFaction != null && !String.IsNullOrEmpty(selectedFaction.Name) && selectedFaction.Name.ToLower() != ANY_GROUP.ToLower())
            {
                availableTemplates = RemoveStatsOfOtherGroups(availableTemplates, selectedFaction.Name);
            }

            if (!RemoveRestrictionFiltering_checkBox.IsChecked.GetValueOrDefault(false))
            {
                Group selectedClass = GetComboBoxGroup(Class_comboBox.SelectedItem);
                if (selectedClass != null && !String.IsNullOrEmpty(selectedClass.Name) && selectedClass.Name.ToLower() != ANY_GROUP.ToLower())
                {
                    availableTemplates = RemoveStatsOfWrongClass(availableTemplates, selectedClass.Name);
                }

                Statistics selectedJob = GetComboBoxStats(Job_comboBox.SelectedItem);
                if (selectedJob != null)
                {
                    string classGroup = selectedJob.Group;
                    if (!String.IsNullOrEmpty(classGroup))
                    {
                        availableTemplates = RemoveStatsOfWrongClass(availableTemplates, classGroup);
                    }

                    if (selectedJob.RestrictToBasicTemplates.GetValueOrDefault(false))
                    {
                        availableTemplates = RemoveNonBasicTemplates(availableTemplates);
                    }
                }
            }

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableTemplates = RemoveHomebrewStats(availableTemplates);
            }

            availableTemplates = RemoveStatsThatDoNotDisplay(availableTemplates);

            availableTemplates.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableTemplates;
        }

        private List<ComboBoxItem> GetAvailableUniqueFoes()
        {
            List<ComboBoxItem> availableUniques = new List<ComboBoxItem>(dropdownOptions.UniqueFoes);

            Group selectedFaction = GetComboBoxGroup(Faction_comboBox.SelectedItem);
            if (selectedFaction != null && !String.IsNullOrEmpty(selectedFaction.Name) && selectedFaction.Name.ToLower() != ANY_GROUP.ToLower())
            {
                availableUniques = RemoveStatsOfOtherGroups(availableUniques, selectedFaction.Name);
            }

            if (!RemoveRestrictionFiltering_checkBox.IsChecked.GetValueOrDefault(false))
            {
                Group selectedClass = GetComboBoxGroup(Class_comboBox.SelectedItem);
                if (selectedClass != null && !String.IsNullOrEmpty(selectedClass.Name) && selectedClass.Name.ToLower() != ANY_GROUP.ToLower())
                {
                    availableUniques = RemoveUniqueFoesOfWrongClass(availableUniques, selectedClass.Name);
                }
            }

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableUniques = RemoveHomebrewStats(availableUniques);
            }

            availableUniques = RemoveStatsThatDoNotDisplay(availableUniques);

            availableUniques.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableUniques;
        }

        private List<ComboBoxItem> GetAvailableSpecialTemplates()
        {
            List<ComboBoxItem> availableSpecials = new List<ComboBoxItem>(dropdownOptions.Specials);

            Group selectedFaction = GetComboBoxGroup(Faction_comboBox.SelectedItem);
            if (selectedFaction != null && !String.IsNullOrEmpty(selectedFaction.Name) && selectedFaction.Name.ToLower() != ANY_GROUP.ToLower())
            {
                availableSpecials = RemoveStatsOfWrongFaction(availableSpecials, selectedFaction.Name);
            }

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableSpecials = RemoveHomebrewStats(availableSpecials);
            }

            availableSpecials = RemoveStatsThatDoNotDisplay(availableSpecials);

            availableSpecials.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableSpecials;
        }

        private List<ComboBoxItem> GetAvailableClasses()
        {
            List<ComboBoxItem> availableClasses = new List<ComboBoxItem>(dropdownOptions.Classes);

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableClasses = RemoveHomebrewGroups(availableClasses);
            }

            availableClasses.Insert(0, new ComboBoxItem() { Content = new Group() { Name = ANY_GROUP, OnlyHomebrew = false } });

            return availableClasses;
        }

        private List<ComboBoxItem> GetAvailableJobs()
        {
            List<ComboBoxItem> availableJobs = new List<ComboBoxItem>(dropdownOptions.Jobs);

            Group selectedClass = GetComboBoxGroup(Class_comboBox.SelectedItem);
            if (selectedClass != null && !String.IsNullOrEmpty(selectedClass.Name) && selectedClass.Name.ToLower() != ANY_GROUP.ToLower())
            {
                availableJobs = RemoveStatsOfOtherGroups(availableJobs, selectedClass.Name);
            }

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableJobs = RemoveHomebrewStats(availableJobs);
            }

            availableJobs = RemoveStatsThatDoNotDisplay(availableJobs);

            availableJobs.Insert(0, new ComboBoxItem() { Content = new Statistics() { Name = EMPTY_STAT } });

            return availableJobs;
        }

        private List<ComboBoxItem> RemoveStatsOfOtherGroups(List<ComboBoxItem> stats, string groupName)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;
                return !String.IsNullOrEmpty(actualStat.Group) && actualStat.Group.ToLower() == groupName.ToLower();
            });
        }

        private List<ComboBoxItem> RemoveStatsOfWrongFaction(List<ComboBoxItem> stats, string factionName)
        {
            string factionNameLower = factionName.ToLower();
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;
                return String.IsNullOrEmpty(actualStat.UsesFaction) || actualStat.UsesFaction.ToLower() == factionNameLower;
            });
        }

        private List<ComboBoxItem> RemoveStatsOfWrongClass(List<ComboBoxItem> templates, string className)
        {
            string classNameLower = className.ToLower();
            return templates.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;
                return String.IsNullOrEmpty(actualStat.UsesClass)
                || classNameLower == StatisticBuilder.MOB
                || actualStat.UsesClass.ToLower() == classNameLower;
            });
        }

        private List<ComboBoxItem> RemoveUniqueFoesOfWrongClass(List<ComboBoxItem> uniqueFoes, string className)
        {
            string classNameLower = className.ToLower();
            return uniqueFoes.FindAll(delegate (ComboBoxItem stat)
            {
                Statistics actualStat = (Statistics)stat.Content;
                if (!String.IsNullOrEmpty(actualStat.UsesClass) && classNameLower == actualStat.UsesClass.ToLower())
                {
                    return true;
                }

                if (actualStat.IsMob.GetValueOrDefault(false) && classNameLower == StatisticBuilder.MOB)
                {
                    return true;
                }
                if (actualStat.IsElite.GetValueOrDefault(false) && classNameLower == StatisticBuilder.ELITE)
                {
                    return true;
                }
                if (actualStat.IsLegend.GetValueOrDefault(false) && classNameLower == StatisticBuilder.LEGEND)
                {
                    return true;
                }

                return false;
            });
        }

        private List<ComboBoxItem> RemoveNonBasicTemplates(List<ComboBoxItem> templates)
        {
            return templates.FindAll(delegate (ComboBoxItem stat)
            {
                return ((Statistics)stat.Content).IsBasicTemplate;
            });
        }

        private List<ComboBoxItem> RemoveHomebrewStats(List<ComboBoxItem> stats)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                return !((Statistics)stat.Content).IsHomebrew;
            });
        }

        private List<ComboBoxItem> RemoveHomebrewGroups(List<ComboBoxItem> groups)
        {
            return groups.FindAll(delegate (ComboBoxItem group)
            {
                return !((Group)group.Content).OnlyHomebrew;
            });
        }

        private List<ComboBoxItem> RemoveStatsThatDoNotDisplay(List<ComboBoxItem> stats)
        {
            return stats.FindAll(delegate (ComboBoxItem stat)
            {
                return !((Statistics)stat.Content).DoNotDisplay;
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
            UpdateUniqueFoeDropdownState();

            UpdateTemplateOptions();
            UpdateUniqueFoeOptions();

            UpdateDescription();
        }

        private void Elite_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateMobCheckBoxState();
            UpdateUniqueFoeDropdownState();

            UpdateTemplateOptions();
            UpdateUniqueFoeOptions();

            UpdateDescription();
        }

        private void RemoveRestrictionFiltering_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateFactionOptions();
            UpdateTemplateOptions();
            UpdateUniqueFoeOptions();
            UpdateSpecialTemplateOptions();
            UpdateClassOptions();
            UpdateJobOptions();

            UpdateDescription();
        }

        private void DamageValues_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDescription();
        }

        private void NonessentialTraits_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDescription();
        }

        private void Homebrew_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateFactionOptions();
            UpdateTemplateOptions();
            UpdateUniqueFoeOptions();
            UpdateSpecialTemplateOptions();
            UpdateClassOptions();
            UpdateJobOptions();
        }

        private void ExportJson_button_Click(object sender, RoutedEventArgs e)
        {
            List<Statistics> statsToMerge = new List<Statistics>();

            Statistics job = GetComboBoxStats(Job_comboBox.SelectedItem);
            if (job != null && Statistics.IsValid(job))
            {
                statsToMerge.Add(job);
            }

            Statistics template = GetComboBoxStats(Template_comboBox.SelectedItem);
            if (template != null && Statistics.IsValid(template))
            {
                statsToMerge.Add(template);
            }

            Statistics uniqueFoe = GetComboBoxStats(UniqueFoe_comboBox.SelectedItem);
            if (uniqueFoe != null && Statistics.IsValid(uniqueFoe))
            {
                statsToMerge.Add(uniqueFoe);
            }

            Statistics special = GetComboBoxStats(Special_comboBox.SelectedItem);
            if (special != null && Statistics.IsValid(special))
            {
                statsToMerge.Add(special);
            }

            if (Mob_checkBox.IsChecked.GetValueOrDefault(false))
            {
                Statistics mobTemplate = statBuilder.Specials.Find(stat => stat.Name.ToLower() == "mob (template)");
                if (mobTemplate != null)
                {
                    statsToMerge.Add(mobTemplate);
                }
            }
            else if (Elite_checkBox.IsChecked.GetValueOrDefault(false))
            {
                Statistics eliteTemplate = statBuilder.Specials.Find(stat => stat.Name.ToLower() == "elite (template)");
                if (eliteTemplate != null)
                {
                    statsToMerge.Add(eliteTemplate);
                }
            }

            Statistics statsToExport = new Statistics();
            string name = String.Empty;
            foreach (Statistics stats in statsToMerge)
            {
                statsToExport = stats.InheritFrom(statsToExport);
                string title = stats.Name;
                if (!String.IsNullOrEmpty(stats.TitleName))
                {
                    title = stats.TitleName;
                }
                name = title + " " + name;
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
