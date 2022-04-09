using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IconFoeCreator
{
    class DropdownOptions
    {
        public static readonly Thickness DEFAULT_BORDER_THICKNESS = new Thickness(1.8);
        public static readonly SolidColorBrush DEFAULT_BRUSH = Brushes.White;

        public List<ComboBoxItem> Factions;
        public List<ComboBoxItem> Templates;
        public List<ComboBoxItem> UniqueFoes;
        public List<ComboBoxItem> Specials;
        public List<ComboBoxItem> Classes;
        public List<ComboBoxItem> Jobs;

        public void ConvertToDropdownItems(StatisticBuilder statBuilder)
        {
            Factions = new List<ComboBoxItem>();
            Templates = new List<ComboBoxItem>();
            UniqueFoes = new List<ComboBoxItem>();
            Specials = new List<ComboBoxItem>();
            Classes = new List<ComboBoxItem>();
            Jobs = new List<ComboBoxItem>();

            ConvertGroupsToComboBoxItems(statBuilder.Factions, Factions);
            ConvertStatisticsToComboBoxItems(statBuilder.Templates, Templates);
            ConvertStatisticsToComboBoxItems(statBuilder.UniqueFoes, UniqueFoes);
            ConvertStatisticsToComboBoxItems(statBuilder.Specials, Specials);
            ConvertGroupsToComboBoxItems(statBuilder.Classes, Classes);
            ConvertStatisticsToComboBoxItems(statBuilder.Jobs, Jobs);
        }

        private void ConvertStatisticsToComboBoxItems(List<Statistics> stats, List<ComboBoxItem> output)
        {
            foreach (Statistics stat in stats)
            {
                string className = String.Empty;
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
                    className = MainWindow.UNIQUE_CLASS;
                }
                else if (!String.IsNullOrEmpty(statConverted.Group))
                {
                    className = statConverted.Group;
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

                output.Add(item);
            }
        }

        private void ConvertGroupsToComboBoxItems(List<Group> names, List<ComboBoxItem> output)
        {
            foreach (Group name in names)
            {
                Brush backgroundBrush = ThemeColors.GetGradientClassColor(name.Name) ?? Brushes.White;
                Brush borderBrush = ThemeColors.GetLinearClassColor(name.Name) ?? Brushes.LightGray;

                ComboBoxItem item = new ComboBoxItem()
                {
                    Content = name,
                    Background = backgroundBrush,
                    BorderBrush = borderBrush,
                    BorderThickness = DEFAULT_BORDER_THICKNESS
                };

                output.Add(item);
            }
        }
    }
}
