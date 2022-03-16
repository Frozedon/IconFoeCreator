using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

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
            if (Faction_comboBox.SelectedItem != null)
            {
                UpdateTemplateOptions();
                UpdateUniqueFoeOptions();
                UpdateSpecialTemplateOptions();
            }
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

        private void UpdateDescription()
        {
            List<Statistics> statsToMerge = new List<Statistics>();

            Statistics job = (Statistics)Job_comboBox.SelectedItem;
            if (job != null && Statistics.IsValid(job))
            {
                statsToMerge.Add(job);
            }

            Statistics template = (Statistics)Template_comboBox.SelectedItem;
            if (template != null && Statistics.IsValid(template))
            {
                statsToMerge.Add(template);
            }

            Statistics uniqueFoe = (Statistics)UniqueFoe_comboBox.SelectedItem;
            if (uniqueFoe != null && Statistics.IsValid(uniqueFoe))
            {
                statsToMerge.Add(uniqueFoe);
            }

            Statistics special = (Statistics)Special_comboBox.SelectedItem;
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
            if (UniqueFoe_comboBox.SelectedItem != null && UniqueFoe_comboBox.SelectedItem.ToString() != EMPTY_STAT)
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
            if ((Job_comboBox.SelectedItem != null && Job_comboBox.SelectedItem.ToString() != EMPTY_STAT)
                || (Template_comboBox.SelectedItem != null && Template_comboBox.SelectedItem.ToString() != EMPTY_STAT)
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
            if (UniqueFoe_comboBox.SelectedItem != null && UniqueFoe_comboBox.SelectedItem.ToString() != EMPTY_STAT)
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
                string className = Class_comboBox.SelectedItem.ToString().ToLower();
                if (className != ANY_GROUP.ToLower() && Array.Find(StatisticBuilder.CoreClasses, name => name == className) == null)
                {
                    enable = false;
                }
            }
            if (Job_comboBox.SelectedItem != null)
            {
                Statistics job = (Statistics)Job_comboBox.SelectedItem;
                if (!String.IsNullOrEmpty(job.Group))
                {
                    string groupName = job.Group.ToLower();
                    if (Array.Find(StatisticBuilder.CoreClasses, name => name == groupName) == null)
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
            if (Class_comboBox.SelectedItem != null)
            {
                string className = Class_comboBox.SelectedItem.ToString().ToLower();
                if (className != ANY_GROUP.ToLower() && Array.Find(StatisticBuilder.CoreClasses, name => name == className) == null)
                {
                    enable = false;
                }
            }
            if (Job_comboBox.SelectedItem != null)
            {
                Statistics job = (Statistics)Job_comboBox.SelectedItem;
                if (!String.IsNullOrEmpty(job.Group))
                {
                    string groupName = job.Group.ToLower();
                    if (Array.Find(StatisticBuilder.CoreClasses, name => name == groupName) == null)
                    {
                        enable = false;
                    }
                }
            }

            Elite_checkBox.IsEnabled = enable;
            if (!enable) { Elite_checkBox.IsChecked = false; }
        }

        private void UpdateFactionOptions()
        {
            Faction_comboBox.ItemsSource = GetAvailableFactions();
            Faction_comboBox.SelectedIndex = 0;
        }

        private void UpdateTemplateOptions()
        {
            string selectedItem = String.Empty;
            if (Template_comboBox.SelectedItem != null)
            {
                selectedItem = Template_comboBox.SelectedItem.ToString();
            }


            Template_comboBox.ItemsSource = GetAvailableTemplates();


            int index = 0;
            if (selectedItem.Length > 0)
            {
                for (int i = 0; i < Template_comboBox.Items.Count; ++i)
                {
                    if (selectedItem == Template_comboBox.Items[i].ToString())
                    {
                        index = i;
                        break;
                    }
                }
            }
            Template_comboBox.SelectedIndex = index;
        }

        private void UpdateUniqueFoeOptions()
        {
            string selectedItem = String.Empty;
            if (UniqueFoe_comboBox.SelectedItem != null)
            {
                selectedItem = UniqueFoe_comboBox.SelectedItem.ToString();
            }


            UniqueFoe_comboBox.ItemsSource = GetAvailableUniqueFoes();


            int index = 0;
            if (selectedItem.Length > 0)
            {
                for (int i = 0; i < UniqueFoe_comboBox.Items.Count; ++i)
                {
                    if (selectedItem == UniqueFoe_comboBox.Items[i].ToString())
                    {
                        index = i;
                        break;
                    }
                }
            }
            UniqueFoe_comboBox.SelectedIndex = index;
        }

        private void UpdateSpecialTemplateOptions()
        {
            string selectedItem = String.Empty;
            if (Special_comboBox.SelectedItem != null)
            {
                selectedItem = Special_comboBox.SelectedItem.ToString();
            }


            Special_comboBox.ItemsSource = GetAvailableSpecialTemplates();


            int index = 0;
            if (selectedItem.Length > 0)
            {
                for (int i = 0; i < Special_comboBox.Items.Count; ++i)
                {
                    if (selectedItem == Special_comboBox.Items[i].ToString())
                    {
                        index = i;
                        break;
                    }
                }
            }
            Special_comboBox.SelectedIndex = index;
        }

        private void UpdateClassOptions()
        {
            Class_comboBox.ItemsSource = GetAvailableClasses();
            Class_comboBox.SelectedIndex = 0;
        }

        private void UpdateJobOptions()
        {
            string selectedItem = String.Empty;
            if (Job_comboBox.SelectedItem != null)
            {
                selectedItem = Job_comboBox.SelectedItem.ToString();
            }


            Job_comboBox.ItemsSource = GetAvailableJobs();


            int index = 0;
            if (selectedItem.Length > 0)
            {
                for (int i = 0; i < Job_comboBox.Items.Count; ++i)
                {
                    if (selectedItem == Job_comboBox.Items[i].ToString())
                    {
                        index = i;
                        break;
                    }
                }
            }
            Job_comboBox.SelectedIndex = index;
        }

        private List<string> GetAvailableFactions()
        {
            List<string> availableFactions = new List<string>(statBuilder.Factions);

            if (availableFactions.Count == 0 || availableFactions[0] != ANY_GROUP)
            {
                availableFactions.Insert(0, ANY_GROUP);
            }

            return availableFactions;
        }

        private List<Statistics> GetAvailableTemplates()
        {
            List<Statistics> availableTemplates = new List<Statistics>(statBuilder.Templates);

            if (Faction_comboBox.SelectedItem != null)
            {
                string factionGroup = Faction_comboBox.SelectedItem.ToString().ToLower();
                if (!String.IsNullOrEmpty(factionGroup) && factionGroup != ANY_GROUP.ToLower())
                {
                    availableTemplates = RemoveStatsOfOtherGroups(availableTemplates, factionGroup);
                }
            }

            if (Class_comboBox.SelectedItem != null)
            {
                string classGroup = Class_comboBox.SelectedItem.ToString().ToLower();
                if (!String.IsNullOrEmpty(classGroup) && classGroup != ANY_GROUP.ToLower())
                {
                    availableTemplates = RemoveStatsOfWrongClass(availableTemplates, classGroup);
                }
            }

            if (Job_comboBox.SelectedItem != null)
            {
                Statistics selectedJob = (Statistics)Job_comboBox.SelectedItem;
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

            if (Elite_checkBox.IsChecked.GetValueOrDefault())
            {
                availableTemplates = RemoveNonBasicTemplates(availableTemplates);
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

            if (Faction_comboBox.SelectedItem != null)
            {
                string factionGroup = Faction_comboBox.SelectedItem.ToString().ToLower();
                if (!String.IsNullOrEmpty(factionGroup) && factionGroup != ANY_GROUP.ToLower())
                {
                    availableUniques = RemoveStatsOfOtherGroups(availableUniques, factionGroup);
                }
            }

            if (Class_comboBox.SelectedItem != null)
            {
                string classGroup = Class_comboBox.SelectedItem.ToString().ToLower();
                if (!String.IsNullOrEmpty(classGroup) && classGroup != ANY_GROUP.ToLower())
                {
                    availableUniques = RemoveUniqueFoesOfWrongClass(availableUniques, classGroup);
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

            if (Faction_comboBox.SelectedItem != null)
            {
                string factionGroup = Faction_comboBox.SelectedItem.ToString().ToLower();
                if (!String.IsNullOrEmpty(factionGroup) && factionGroup != ANY_GROUP.ToLower())
                {
                    availableSpecials = RemoveStatsOfWrongFaction(availableSpecials, factionGroup);
                }
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
            List<string> availableClasses = new List<string>(statBuilder.Classes);

            if (availableClasses.Count == 0 || availableClasses[0] != ANY_GROUP)
            {
                availableClasses.Insert(0, ANY_GROUP);
            }

            return availableClasses;
        }

        private List<Statistics> GetAvailableJobs()
        {
            List<Statistics> availableJobs = new List<Statistics>(statBuilder.Jobs);

            if (Class_comboBox.SelectedItem != null)
            {
                string classGroup = Class_comboBox.SelectedItem.ToString().ToLower();
                if (classGroup != ANY_GROUP.ToLower())
                {
                    availableJobs = RemoveStatsOfOtherGroups(availableJobs, classGroup);
                }
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
                || classNameLower == "mob"
                || stat.UsesClass.ToLower() == classNameLower;
            });
        }

        private List<Statistics> RemoveUniqueFoesOfWrongClass(List<Statistics> uniqueFoes, string className)
        {
            string classNameLower = className.ToLower();
            return uniqueFoes.FindAll(delegate (Statistics stat)
            {
                if (!String.IsNullOrEmpty(stat.UsesClass) && classNameLower != stat.UsesClass.ToLower())
                {
                    return false;
                }

                if (stat.IsMob.GetValueOrDefault(false) && classNameLower != "mob")
                {
                    return false;
                }
                if (stat.IsElite.GetValueOrDefault(false) && classNameLower != "elite")
                {
                    return false;
                }
                if (stat.IsLegend.GetValueOrDefault(false) && classNameLower != "legend")
                {
                    return false;
                }

                return true;
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
            Statistics job = (Statistics)Job_comboBox.SelectedItem;
            Statistics template = (Statistics)Template_comboBox.SelectedItem;
            Statistics statsToExport;

            bool hasTemplate = template != null && Statistics.IsValid(template);
            bool hasJob = job != null && Statistics.IsValid(job);
            string name;

            if (hasTemplate && hasJob)
            {
                statsToExport = job.InheritFrom(template);
                name = template.ToString() + " " + job.ToString();
            }
            else if (hasTemplate)
            {
                statsToExport = template;
                name = template.ToString();
            }
            else if (hasJob)
            {
                statsToExport = job;
                name = job.ToString();
            }
            else
            {
                return;
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
