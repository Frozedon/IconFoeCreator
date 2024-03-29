﻿using System;
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
        public List<ComboBoxItem> Classes;
        public List<ComboBoxItem> SpecialClasses;
        public List<ComboBoxItem> Foes;
        public List<ComboBoxItem> Templates;
        public List<ComboBoxItem> SpecialTemplates;

        public void ConvertToDropdownItems(StatisticBuilder statBuilder)
        {
            Factions = new List<ComboBoxItem>();
            Classes = new List<ComboBoxItem>();
            SpecialClasses = new List<ComboBoxItem>();
            Foes = new List<ComboBoxItem>();
            Templates = new List<ComboBoxItem>();
            SpecialTemplates = new List<ComboBoxItem>();

            ConvertStringsToComboBoxItems(statBuilder.Factions, Factions);
            ConvertStringsToComboBoxItems(statBuilder.Classes, Classes);
            ConvertStringsToComboBoxItems(statBuilder.SpecialClasses, SpecialClasses);
            ConvertStatisticsToComboBoxItems(statBuilder.Foes, Foes);
            ConvertStatisticsToComboBoxItems(statBuilder.Templates, Templates);
            ConvertStatisticsToComboBoxItems(statBuilder.SpecialTemplates, SpecialTemplates);
        }

        private void ConvertStatisticsToComboBoxItems(List<Statistics> stats, List<ComboBoxItem> output)
        {
            foreach (Statistics stat in stats)
            {
                string className = String.Empty;
                Statistics statConverted = (Statistics)(object)stat;
                if (!String.IsNullOrEmpty(statConverted.Class))
                {
                    className = statConverted.Class;
                }
                else if (!String.IsNullOrEmpty(statConverted.SpecialClass) && statConverted.SpecialClass.ToLower() == StatisticBuilder.SPECIAL_CLASS_MOB)
                {
                    className = StatisticBuilder.SPECIAL_CLASS_MOB;
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

        private void ConvertStringsToComboBoxItems(List<string> names, List<ComboBoxItem> output)
        {
            foreach (string name in names)
            {
                Brush backgroundBrush = ThemeColors.GetGradientClassColor(name) ?? Brushes.White;
                Brush borderBrush = ThemeColors.GetLinearClassColor(name) ?? Brushes.LightGray;

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
