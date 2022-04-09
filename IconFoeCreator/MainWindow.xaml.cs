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
        public static readonly string EMPTY_STAT = "...";
        public static readonly string ANY_GROUP = "Any";

        public static readonly string UNIQUE_CLASS = "unique";

        public static readonly Thickness DEFAULT_BORDER_THICKNESS = new Thickness(1.8);
        public static readonly SolidColorBrush DEFAULT_BRUSH = Brushes.White;

        StatisticBuilder statBuilder;

        public MainWindow()
        {
            InitializeComponent();

            statBuilder = new StatisticBuilder();
            statBuilder.BuildStatistics();

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
                string selectedClass = GetComboBoxString(Class_comboBox.SelectedItem);
                if (!String.IsNullOrEmpty(selectedClass))
                {
                    string className = selectedClass.ToString().ToLower();
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
            string selectedClass = GetComboBoxString(Class_comboBox.SelectedItem);
            if (!String.IsNullOrEmpty(selectedClass))
            {
                string className = selectedClass.ToString().ToLower();
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

            comboBox.ItemsSource = AddColorToOptions(getStats());

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

        private List<ComboBoxItem> AddColorToOptions<T>(List<T> stats)
        {
            List<ComboBoxItem> comboBoxItems = new List<ComboBoxItem>();

            foreach (T stat in stats)
            {
                string className = String.Empty;

                if (typeof(T) == typeof(Statistics))
                {
                    Statistics statConverted = (Statistics)(object)stat;
                    if (!String.IsNullOrEmpty(statConverted.UsesClass))
                    {
                        className = statConverted.UsesClass;
                    }
                    else if (statConverted.IsMob.GetValueOrDefault(false))
                    {
                        className = StatisticBuilder.MOB;
                    }
                    else if (statConverted.Type.ToLower() == StatisticBuilder.TYPE_TEMPLATE && !statConverted.IsBasicTemplate)
                    {
                        className = UNIQUE_CLASS;
                    }
                    else if (!String.IsNullOrEmpty(statConverted.Group))
                    {
                        className = statConverted.Group;
                    }
                }
                else if (typeof(T) == typeof(string))
                {
                    className = (string)(object)stat;
                }

                // Change color based on class
                Brush backgroundBrush = ThemeColors.GetGradientClassColor(className) ?? Brushes.White;
                Brush borderBrush = ThemeColors.GetLinearClassColor(className) ?? Brushes.LightGray;

                ComboBoxItem item = new ComboBoxItem()
                {
                    Content = stat,
                    Background = backgroundBrush,
                    BorderBrush = borderBrush,
                    BorderThickness = DEFAULT_BORDER_THICKNESS
                };

                comboBoxItems.Add(item);
            }

            return comboBoxItems;
        }

        private List<string> GetAvailableFactions()
        {
            List<Group> availableFactions = new List<Group>(statBuilder.Factions);

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableFactions = RemoveHomebrewGroups(availableFactions);
            }

            List<string> avaiableFactionNames = new List<string>();

            avaiableFactionNames.Insert(0, ANY_GROUP);
            foreach (Group group in availableFactions)
            {
                avaiableFactionNames.Add(group.Name);
            }

            return avaiableFactionNames;
        }

        private List<Statistics> GetAvailableTemplates()
        {
            List<Statistics> availableTemplates = new List<Statistics>(statBuilder.Templates);

            string selectedFaction = GetComboBoxString(Faction_comboBox.SelectedItem);
            if (!String.IsNullOrEmpty(selectedFaction) && selectedFaction.ToLower() != ANY_GROUP.ToLower())
            {
                availableTemplates = RemoveStatsOfOtherGroups(availableTemplates, selectedFaction);
            }

            if (!RemoveRestrictionFiltering_checkBox.IsChecked.GetValueOrDefault(false))
            {
                string selectedClass = GetComboBoxString(Class_comboBox.SelectedItem);
                if (!String.IsNullOrEmpty(selectedClass) && selectedClass.ToLower() != ANY_GROUP.ToLower())
                {
                    availableTemplates = RemoveStatsOfWrongClass(availableTemplates, selectedClass);
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

            availableTemplates.Insert(0, new Statistics() { Name = EMPTY_STAT });

            return availableTemplates;
        }

        private List<Statistics> GetAvailableUniqueFoes()
        {
            List<Statistics> availableUniques = new List<Statistics>(statBuilder.UniqueFoes);

            string selectedFaction = GetComboBoxString(Faction_comboBox.SelectedItem);
            if (!String.IsNullOrEmpty(selectedFaction) && selectedFaction.ToLower() != ANY_GROUP.ToLower())
            {
                availableUniques = RemoveStatsOfOtherGroups(availableUniques, selectedFaction);
            }

            if (!RemoveRestrictionFiltering_checkBox.IsChecked.GetValueOrDefault(false))
            {
                string selectedClass = GetComboBoxString(Class_comboBox.SelectedItem);
                if (!String.IsNullOrEmpty(selectedClass) && selectedClass.ToLower() != ANY_GROUP.ToLower())
                {
                    availableUniques = RemoveUniqueFoesOfWrongClass(availableUniques, selectedClass);
                }
            }

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableUniques = RemoveHomebrewStats(availableUniques);
            }

            availableUniques = RemoveStatsThatDoNotDisplay(availableUniques);

            availableUniques.Insert(0, new Statistics() { Name = EMPTY_STAT });

            return availableUniques;
        }
        
        private List<Statistics> GetAvailableSpecialTemplates()
        {
            List<Statistics> availableSpecials = new List<Statistics>(statBuilder.Specials);

            string selectedFaction = GetComboBoxString(Faction_comboBox.SelectedItem);
            if (!String.IsNullOrEmpty(selectedFaction) && selectedFaction.ToLower() != ANY_GROUP.ToLower())
            {
                availableSpecials = RemoveStatsOfWrongFaction(availableSpecials, selectedFaction);
            }

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableSpecials = RemoveHomebrewStats(availableSpecials);
            }

            availableSpecials = RemoveStatsThatDoNotDisplay(availableSpecials);

            availableSpecials.Insert(0, new Statistics() { Name = EMPTY_STAT });

            return availableSpecials;
        }

        private List<string> GetAvailableClasses()
        {
            List<Group> availableClasses = new List<Group>(statBuilder.Classes);

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableClasses = RemoveHomebrewGroups(availableClasses);
            }

            List<string> availableClassNames = new List<string>();

            availableClassNames.Insert(0, ANY_GROUP);
            foreach (Group group in availableClasses)
            {
                availableClassNames.Add(group.Name);
            }

            return availableClassNames;
        }

        private List<Statistics> GetAvailableJobs()
        {
            List<Statistics> availableJobs = new List<Statistics>(statBuilder.Jobs);

            string selectedClass = GetComboBoxString(Class_comboBox.SelectedItem);
            if (!String.IsNullOrEmpty(selectedClass) && selectedClass.ToLower() != ANY_GROUP.ToLower())
            {
                availableJobs = RemoveStatsOfOtherGroups(availableJobs, selectedClass);
            }

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableJobs = RemoveHomebrewStats(availableJobs);
            }

            availableJobs = RemoveStatsThatDoNotDisplay(availableJobs);

            availableJobs.Insert(0, new Statistics() { Name = EMPTY_STAT });

            return availableJobs;
        }

        private List<Statistics> RemoveStatsOfOtherGroups(List<Statistics> stats, string groupName)
        {
            return stats.FindAll(delegate (Statistics stat)
            {
                return !String.IsNullOrEmpty(stat.Group) && stat.Group.ToLower() == groupName.ToLower();
            });
        }

        private List<Statistics> RemoveStatsOfWrongFaction(List<Statistics> stats, string factionName)
        {
            string factionNameLower = factionName.ToLower();
            return stats.FindAll(delegate (Statistics stat)
            {
                return String.IsNullOrEmpty(stat.UsesFaction) || stat.UsesFaction.ToLower() == factionNameLower;
            });
        }

        private List<Statistics> RemoveStatsOfWrongClass(List<Statistics> templates, string className)
        {
            string classNameLower = className.ToLower();
            return templates.FindAll(delegate (Statistics stat)
            {
                return String.IsNullOrEmpty(stat.UsesClass)
                || classNameLower == StatisticBuilder.MOB
                || stat.UsesClass.ToLower() == classNameLower;
            });
        }

        private List<Statistics> RemoveUniqueFoesOfWrongClass(List<Statistics> uniqueFoes, string className)
        {
            string classNameLower = className.ToLower();
            return uniqueFoes.FindAll(delegate (Statistics stat)
            {
                if (!String.IsNullOrEmpty(stat.UsesClass) && classNameLower == stat.UsesClass.ToLower())
                {
                    return true;
                }

                if (stat.IsMob.GetValueOrDefault(false) && classNameLower == StatisticBuilder.MOB)
                {
                    return true;
                }
                if (stat.IsElite.GetValueOrDefault(false) && classNameLower == StatisticBuilder.ELITE)
                {
                    return true;
                }
                if (stat.IsLegend.GetValueOrDefault(false) && classNameLower == StatisticBuilder.LEGEND)
                {
                    return true;
                }

                return false;
            });
        }

        private List<Statistics> RemoveNonBasicTemplates(List<Statistics> templates)
        {
            return templates.FindAll(delegate (Statistics stat)
            {
                return stat.IsBasicTemplate;
            });
        }

        private List<Statistics> RemoveHomebrewStats(List<Statistics> stats)
        {
            return stats.FindAll(delegate (Statistics stat)
            {
                return !stat.IsHomebrew;
            });
        }

        private List<Group> RemoveHomebrewGroups(List<Group> groups)
        {
            return groups.FindAll(delegate (Group group)
            {
                return !group.OnlyHomebrew;
            });
        }

        private List<Statistics> RemoveStatsThatDoNotDisplay(List<Statistics> stats)
        {
            return stats.FindAll(delegate (Statistics stat)
            {
                return !stat.DoNotDisplay;
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
            UpdateClassOptions();
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
