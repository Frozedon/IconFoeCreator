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
            statBuilder.BuildTestStatistics();

            comboBox_Faction.DataSource = statBuilder.Factions;
            comboBox_Faction.SelectedIndexChanged += OnIndexChanged;

            comboBox_Job.DataSource = statBuilder.Jobs;
            comboBox_Job.SelectedIndexChanged += OnIndexChanged;

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
