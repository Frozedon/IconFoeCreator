using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IconFoeCreator
{
    public partial class Form1 : Form
    {
        StatisticBuilder statBuilder;

        public Form1()
        {
            InitializeComponent();

            statBuilder = new StatisticBuilder();
            statBuilder.BuildStatistics();

            comboBox_Faction.DataSource = statBuilder.Factions;
            comboBox_Faction.SelectedIndexChanged += OnIndexChanged;

            comboBox_Job.DataSource = statBuilder.Jobs;
            comboBox_Job.SelectedIndexChanged += OnIndexChanged;

            comboBox_FactionGroup.DataSource = statBuilder.FactionGroups;
            comboBox_FactionGroup.SelectedIndexChanged += OnFactionGroupChanged;

            comboBox_JobGroup.DataSource = statBuilder.JobGroups;
            comboBox_JobGroup.SelectedIndexChanged += OnJobGroupChanged;

            ChapterItem[] chapters = new ChapterItem[Constants.ChapterCount];
            for (int i = 0; i < Constants.ChapterCount; ++i)
            {
                chapters[i] = new ChapterItem() { Value = i + 1 };
            }
            comboBox_Chapter.DataSource = chapters;
            comboBox_Chapter.SelectedIndexChanged += OnIndexChanged;

            UpdateDescription();
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            UpdateDescription();
        }

        private void checkBox_Damage_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            ChapterItem chapterItem = (ChapterItem)comboBox_Chapter.SelectedItem;
            DescriptionCreator.UpdateDescription(
                richTextBox_Description,
                (Statistics)comboBox_Faction.SelectedItem,
                (Statistics)comboBox_Job.SelectedItem,
                chapterItem.Value,
                checkBox_Damage.Checked);
        }

        private void button_copyClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox_Description.Text);
        }

        private void OnFactionGroupChanged(object sender, EventArgs e)
        {
            string group = comboBox_FactionGroup.SelectedItem.ToString().ToLower();
            if (group == StatisticBuilder.ANY_GROUP.ToLower())
            {
                comboBox_Faction.DataSource = statBuilder.Factions;
            }
            else
            {
                List<Statistics> filteredFactions = statBuilder.Factions.FindAll(delegate (Statistics stat)
                {
                    return stat.Group != null && stat.Group.ToLower() == group;
                });
                comboBox_Faction.DataSource = filteredFactions;
            }

            comboBox_Faction.SelectedIndex = 0;
        }

        private void OnJobGroupChanged(object sender, EventArgs e)
        {
            string group = comboBox_JobGroup.SelectedItem.ToString().ToLower();
            if (group == StatisticBuilder.ANY_GROUP.ToLower())
            {
                comboBox_Job.DataSource = statBuilder.Jobs;
            }
            else
            {
                List<Statistics> filteredJobs = statBuilder.Jobs.FindAll(delegate (Statistics stat)
                {
                    return stat.Group != null && stat.Group.ToLower() == group;
                });
                comboBox_Job.DataSource = filteredJobs;
            }

            comboBox_Job.SelectedIndex = 0;
        }
    }

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
}
