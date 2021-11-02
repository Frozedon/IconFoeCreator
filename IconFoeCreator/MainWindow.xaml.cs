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
        StatisticBuilder statBuilder;

        public MainWindow()
        {
            InitializeComponent();

            statBuilder = new StatisticBuilder();
            statBuilder.BuildStatistics();

            UpdateFactionOptions();
            UpdateTemplateOptions();
            UpdateClassOptions();
            UpdateJobOptions();

            Faction_comboBox.SelectionChanged += OnFactionChanged;
            Class_comboBox.SelectionChanged += OnClassChanged;
            Template_comboBox.SelectionChanged += OnIndexChanged;
            Job_comboBox.SelectionChanged += OnIndexChanged;

            ChapterItem[] chapters = new ChapterItem[Constants.ChapterCount];
            for (int i = 0; i < Constants.ChapterCount; ++i)
            {
                chapters[i] = new ChapterItem() { Value = i + 1 };
            }
            Chapter_comboBox.ItemsSource = chapters;
            Chapter_comboBox.SelectedIndex = 0;
            Chapter_comboBox.SelectionChanged += OnIndexChanged;

            UpdateDescription();
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            UpdateDescription();
        }

        private void OnFactionChanged(object sender, EventArgs e)
        {
            UpdateTemplateOptions();
        }

        private void OnClassChanged(object sender, EventArgs e)
        {
            UpdateJobOptions();
        }

        private void UpdateDescription()
        {
            ChapterItem chapterItem = (ChapterItem)Chapter_comboBox.SelectedItem;
            DescriptionCreator.UpdateDescription(
                Description_richTextBox,
                Setup_richTextBox,
                (Statistics)Template_comboBox.SelectedItem,
                (Statistics)Job_comboBox.SelectedItem,
                chapterItem.Value,
                FlatDamage_checkBox.IsChecked.GetValueOrDefault(),
                NonessentialTraits_checkBox.IsChecked.GetValueOrDefault());
        }

        private void UpdateFactionOptions()
        {
            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            List<StatisticGroup> statGroups = statBuilder.Factions.FindAll(delegate (StatisticGroup stat)
            {
                return showHomebrew || stat.HasBase;
            });

            List<string> statGroupNames = new List<string>();
            foreach (StatisticGroup stat in statGroups)
            {
                statGroupNames.Add(stat.Name);
            }

            Faction_comboBox.ItemsSource = statGroupNames;
            Faction_comboBox.SelectedIndex = 0;
        }

        private void UpdateClassOptions()
        {
            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            List<StatisticGroup> statGroups = statBuilder.Classes.FindAll(delegate (StatisticGroup stat)
            {
                return showHomebrew || stat.HasBase;
            });

            List<string> statGroupNames = new List<string>();
            foreach (StatisticGroup stat in statGroups)
            {
                statGroupNames.Add(stat.Name);
            }

            Class_comboBox.ItemsSource = statGroupNames;
            Class_comboBox.SelectedIndex = 0;
        }

        private void UpdateTemplateOptions()
        {
            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            string factionGroup = Faction_comboBox.SelectedItem.ToString().ToLower();
            if (factionGroup == StatisticBuilder.ANY_GROUP.ToLower())
            {
                Template_comboBox.ItemsSource = statBuilder.Templates.FindAll(delegate (Statistics stat)
                {
                    return showHomebrew || !stat.IsHomebrew;
                });
            }
            else
            {
                Template_comboBox.ItemsSource = statBuilder.Templates.FindAll(delegate (Statistics stat)
                {
                    return stat.Group != null && stat.Group.ToLower() == factionGroup && (showHomebrew || !stat.IsHomebrew);
                });
            }

            Template_comboBox.SelectedIndex = 0;
        }

        private void UpdateJobOptions()
        {
            bool showHomebrew = Homebrew_checkBox.IsChecked.GetValueOrDefault();
            string classGroup = Class_comboBox.SelectedItem.ToString().ToLower();
            if (classGroup == StatisticBuilder.ANY_GROUP.ToLower())
            {
                Job_comboBox.ItemsSource = statBuilder.Jobs.FindAll(delegate (Statistics stat)
                {
                    return showHomebrew || !stat.IsHomebrew;
                });
            }
            else
            {
                Job_comboBox.ItemsSource = statBuilder.Jobs.FindAll(delegate (Statistics stat)
                {
                    return stat.Group != null && stat.Group.ToLower() == classGroup && (showHomebrew || !stat.IsHomebrew);
                });
            }

            Job_comboBox.SelectedIndex = 0;
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
            UpdateFactionOptions();
            UpdateTemplateOptions();
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
