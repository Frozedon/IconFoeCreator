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

            Faction_comboBox.ItemsSource = GetAvailableFactions();
            Faction_comboBox.SelectedIndex = 0;
            Class_comboBox.ItemsSource = GetAvailableClasses();
            Class_comboBox.SelectedIndex = 0;
            UpdateTemplateOptions();
            UpdateJobOptions();

            Faction_comboBox.SelectionChanged += OnFactionChanged;
            Template_comboBox.SelectionChanged += OnTemplateChanged;
            Class_comboBox.SelectionChanged += OnClassChanged;
            Job_comboBox.SelectionChanged += OnJobChanged;

            UpdateDescription();
        }

        private void OnFactionChanged(object sender, EventArgs e)
        {
            if (Faction_comboBox.SelectedItem != null)
            {
                UpdateTemplateOptions();
            }
        }

        private void OnClassChanged(object sender, EventArgs e)
        {
            if (Class_comboBox.SelectedItem != null)
            {
                UpdateTemplateOptions();
                UpdateJobOptions();
            }
        }

        private void OnTemplateChanged(object sender, EventArgs e)
        {
            UpdateDescription();
        }

        private void OnJobChanged(object sender, EventArgs e)
        {
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

            DescriptionCreator.UpdateDescription(
                Description_richTextBox,
                Setup_richTextBox,
                statsToMerge,
                NonessentialTraits_checkBox.IsChecked.GetValueOrDefault());
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

        private List<string> GetAvailableClasses()
        {
            List<string> availableClasses = new List<string>(statBuilder.Classes);

            if (availableClasses.Count == 0 || availableClasses[0] != ANY_GROUP)
            {
                availableClasses.Insert(0, ANY_GROUP);
            }

            return availableClasses;
        }

        private List<Statistics> GetAvailableTemplates()
        {
            List<Statistics> availableTemplates = new List<Statistics>(statBuilder.Templates);

            if (Faction_comboBox.SelectedItem != null)
            {
                string factionGroup = Faction_comboBox.SelectedItem.ToString().ToLower();
                if (factionGroup != ANY_GROUP.ToLower())
                {
                    availableTemplates = RemoveStatsOfOtherGroups(availableTemplates, factionGroup);
                }

                string classGroup = Class_comboBox.SelectedItem.ToString().ToLower();
                if (classGroup != ANY_GROUP.ToLower())
                {
                    availableTemplates = RemoveStatsOfOtherClasses(availableTemplates, classGroup);
                }
            }

            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            if (!showHomebrew)
            {
                availableTemplates = RemoveHomebrewStats(availableTemplates);
            }

            availableTemplates.Insert(0, new Statistics() { Name = EMPTY_STAT });

            return availableTemplates;
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

            availableJobs.Insert(0, new Statistics() { Name = EMPTY_STAT });

            return availableJobs;
        }

        private List<string> RemoveGroupsNotInStats(List<string> groups, List<Statistics> stats)
        {
            return groups.FindAll(delegate (string groupName)
            {
                foreach (Statistics stat in stats)
                {
                    if (stat.Group == groupName)
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        private List<Statistics> RemoveStatsOfOtherGroups(List<Statistics> stats, string groupName)
        {
            return stats.FindAll(delegate (Statistics stat)
            {
                return !String.IsNullOrEmpty(stat.Group) && stat.Group.ToLower() == groupName.ToLower();
            });
        }

        private List<Statistics> RemoveStatsOfOtherClasses(List<Statistics> stats, string className)
        {
            return stats.FindAll(delegate (Statistics stat)
            {
                return String.IsNullOrEmpty(stat.UsesClass) || stat.UsesClass.ToLower() == className.ToLower();
            });
        }

        private List<Statistics> RemoveHomebrewStats(List<Statistics> stats)
        {
            return stats.FindAll(delegate (Statistics stat)
            {
                return !stat.IsHomebrew;
            });
        }

        private void ForceChangeTemplateTo(string templateName)
        {
            if (String.IsNullOrEmpty(templateName))
            {
                return;
            }

            for (int i = 0; i < Template_comboBox.Items.Count; ++i)
            {
                Statistics templateStat = (Statistics)Template_comboBox.Items[i];
                if (templateStat.Name == templateName)
                {
                    Template_comboBox.SelectedIndex = i;
                    break;
                }
            }

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

        private void NonessentialTraits_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDescription();
        }

        private void FlatDamage_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDescription();
        }

        private void Homebrew_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTemplateOptions();
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
