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

            Faction_comboBox.ItemsSource = statBuilder.Factions;
            Faction_comboBox.SelectedIndex = 0;
            Faction_comboBox.SelectionChanged += OnFactionChanged;

            Template_comboBox.ItemsSource = statBuilder.Templates;
            Template_comboBox.SelectedIndex = 0;
            Template_comboBox.SelectionChanged += OnIndexChanged;

            Class_comboBox.ItemsSource = statBuilder.Classes;
            Class_comboBox.SelectedIndex = 0;
            Class_comboBox.SelectionChanged += OnClassChanged;

            Job_comboBox.ItemsSource = statBuilder.Jobs;
            Job_comboBox.SelectedIndex = 0;
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
            string group = Faction_comboBox.SelectedItem.ToString().ToLower();
            if (group == StatisticBuilder.ANY_GROUP.ToLower())
            {
                Template_comboBox.ItemsSource = statBuilder.Factions;
            }
            else
            {
                List<Statistics> filteredFactions = statBuilder.Templates.FindAll(delegate (Statistics stat)
                {
                    return stat.Group != null && stat.Group.ToLower() == group;
                });
                Template_comboBox.ItemsSource = filteredFactions;
            }

            Template_comboBox.SelectedIndex = 0;
        }

        private void OnClassChanged(object sender, EventArgs e)
        {
            string group = Class_comboBox.SelectedItem.ToString().ToLower();
            if (group == StatisticBuilder.ANY_GROUP.ToLower())
            {
                Job_comboBox.ItemsSource = statBuilder.Jobs;
            }
            else
            {
                List<Statistics> filteredJobs = statBuilder.Jobs.FindAll(delegate (Statistics stat)
                {
                    return stat.Group != null && stat.Group.ToLower() == group;
                });
                Job_comboBox.ItemsSource = filteredJobs;
            }

            Job_comboBox.SelectedIndex = 0;
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
