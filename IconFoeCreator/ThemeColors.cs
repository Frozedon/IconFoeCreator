using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IconFoeCreator
{
    class ThemeColors
    {
        public static readonly Color UNIQUE_COLOR = Color.FromRgb(0xCA, 0x55, 0xEF);
        public static readonly Color MOB_COLOR = Color.FromRgb(0x92, 0x92, 0x92);
        public static readonly Color NORMAL_COLOR = Color.FromRgb(0x7C, 0x7C, 0x7C);
        public static readonly Color ELITE_COLOR = Color.FromRgb(0x55, 0x80, 0xED);
        public static readonly Color LEGEND_COLOR = Color.FromRgb(0x69, 0x55, 0xED);
        public static readonly Color HEAVY_COLOR = Color.FromRgb(0xEE, 0x22, 0x0C);
        public static readonly Color SKIRMISHER_COLOR = Color.FromRgb(0xFE, 0xAE, 0x00);
        public static readonly Color ARTILLERY_COLOR = Color.FromRgb(0x00, 0xA2, 0xFF);
        public static readonly Color LEADER_COLOR = Color.FromRgb(0x61, 0xD8, 0x36);

        public static readonly SolidColorBrush UNIQUE_BRUSH = new SolidColorBrush(UNIQUE_COLOR);
        public static readonly SolidColorBrush MOB_BRUSH = new SolidColorBrush(MOB_COLOR);
        public static readonly SolidColorBrush NORMAL_BRUSH = new SolidColorBrush(NORMAL_COLOR);
        public static readonly SolidColorBrush ELITE_BRUSH = new SolidColorBrush(ELITE_COLOR);
        public static readonly SolidColorBrush LEGEND_BRUSH = new SolidColorBrush(LEGEND_COLOR);
        public static readonly SolidColorBrush HEAVY_BRUSH = new SolidColorBrush(HEAVY_COLOR);
        public static readonly SolidColorBrush SKIRMISHER_BRUSH = new SolidColorBrush(SKIRMISHER_COLOR);
        public static readonly SolidColorBrush ARTILLERY_BRUSH = new SolidColorBrush(ARTILLERY_COLOR);
        public static readonly SolidColorBrush LEADER_BRUSH = new SolidColorBrush(LEADER_COLOR);

        private static readonly double GRADIENT_START = 0.8;

        public static readonly LinearGradientBrush UNIQUE_BRUSH_GRADIENT = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection(new List<GradientStop> {
                new GradientStop() { Color = Colors.White, Offset = 0.0 },
                new GradientStop() { Color = Colors.White, Offset = GRADIENT_START },
                new GradientStop() { Color = UNIQUE_COLOR, Offset = 1.0 }
            })
        };
        public static readonly LinearGradientBrush MOB_BRUSH_GRADIENT = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection(new List<GradientStop> {
                new GradientStop() { Color = Colors.White, Offset = 0.0 },
                new GradientStop() { Color = Colors.White, Offset = GRADIENT_START },
                new GradientStop() { Color = MOB_COLOR, Offset = 1.0 }
            })
        };
        public static readonly LinearGradientBrush NORMAL_BRUSH_GRADIENT = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection(new List<GradientStop> {
                new GradientStop() { Color = Colors.White, Offset = 0.0 },
                new GradientStop() { Color = Colors.White, Offset = GRADIENT_START },
                new GradientStop() { Color = NORMAL_COLOR, Offset = 1.0 }
            })
        };
        public static readonly LinearGradientBrush ELITE_BRUSH_GRADIENT = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection(new List<GradientStop> {
                new GradientStop() { Color = Colors.White, Offset = 0.0 },
                new GradientStop() { Color = Colors.White, Offset = GRADIENT_START },
                new GradientStop() { Color = ELITE_COLOR, Offset = 1.0 }
            })
        };
        public static readonly LinearGradientBrush LEGEND_BRUSH_GRADIENT = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection(new List<GradientStop> {
                new GradientStop() { Color = Colors.White, Offset = 0.0 },
                new GradientStop() { Color = Colors.White, Offset = GRADIENT_START },
                new GradientStop() { Color = LEGEND_COLOR, Offset = 1.0 }
            })
        };
        public static readonly LinearGradientBrush HEAVY_BRUSH_GRADIENT = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection(new List<GradientStop> {
                new GradientStop() { Color = Colors.White, Offset = 0.0 },
                new GradientStop() { Color = Colors.White, Offset = GRADIENT_START },
                new GradientStop() { Color = HEAVY_COLOR, Offset = 1.0 }
            })
        };
        public static readonly LinearGradientBrush SKIRMISHER_BRUSH_GRADIENT = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection(new List<GradientStop> {
                new GradientStop() { Color = Colors.White, Offset = 0.0 },
                new GradientStop() { Color = Colors.White, Offset = GRADIENT_START },
                new GradientStop() { Color = SKIRMISHER_COLOR, Offset = 1.0 }
            })
        };
        public static readonly LinearGradientBrush ARTILLERY_BRUSH_GRADIENT = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection(new List<GradientStop> {
                new GradientStop() { Color = Colors.White, Offset = 0.0 },
                new GradientStop() { Color = Colors.White, Offset = GRADIENT_START },
                new GradientStop() { Color = ARTILLERY_COLOR, Offset = 1.0 }
            })
        };
        public static readonly LinearGradientBrush LEADER_BRUSH_GRADIENT = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops = new GradientStopCollection(new List<GradientStop> {
                new GradientStop() { Color = Colors.White, Offset = 0.0 },
                new GradientStop() { Color = Colors.White, Offset = GRADIENT_START },
                new GradientStop() { Color = LEADER_COLOR, Offset = 1.0 }
            })
        };

        public static Brush GetLinearClassColor(string className)
        {
            if (String.IsNullOrEmpty(className))
            {
                return null; 
            }

            string nameLower = className.ToLower();

            if (nameLower == StatisticBuilder.CLASS_HEAVY)
            {
                return ThemeColors.HEAVY_BRUSH;
            }
            else if (nameLower == StatisticBuilder.CLASS_SKIRMISHER)
            {
                return ThemeColors.SKIRMISHER_BRUSH;
            }
            else if (nameLower == StatisticBuilder.CLASS_LEADER)
            {
                return ThemeColors.LEADER_BRUSH;
            }
            else if (nameLower == StatisticBuilder.CLASS_ARTILLERY)
            {
                return ThemeColors.ARTILLERY_BRUSH;
            }
            else if (nameLower == StatisticBuilder.CLASS_UNIQUE)
            {
                return ThemeColors.UNIQUE_BRUSH;
            }
            else if (nameLower == StatisticBuilder.SPECIAL_CLASS_MOB)
            {
                return ThemeColors.MOB_BRUSH;
            }
            else if (nameLower == StatisticBuilder.SPECIAL_CLASS_NORMAL)
            {
                return ThemeColors.NORMAL_BRUSH;
            }
            else if (nameLower == StatisticBuilder.SPECIAL_CLASS_ELITE)
            {
                return ThemeColors.ELITE_BRUSH;
            }
            else if (nameLower == StatisticBuilder.SPECIAL_CLASS_LEGEND)
            {
                return ThemeColors.LEGEND_BRUSH;
            }

            return null;
        }

        public static Brush GetGradientClassColor(string className)
        {
            if (String.IsNullOrEmpty(className))
            {
                return null;
            }

            string nameLower = className.ToLower();

            if (nameLower == StatisticBuilder.CLASS_HEAVY)
            {
                return ThemeColors.HEAVY_BRUSH_GRADIENT;
            }
            else if (nameLower == StatisticBuilder.CLASS_SKIRMISHER)
            {
                return ThemeColors.SKIRMISHER_BRUSH_GRADIENT;
            }
            else if (nameLower == StatisticBuilder.CLASS_LEADER)
            {
                return ThemeColors.LEADER_BRUSH_GRADIENT;
            }
            else if (nameLower == StatisticBuilder.CLASS_ARTILLERY)
            {
                return ThemeColors.ARTILLERY_BRUSH_GRADIENT;
            }
            else if (nameLower == StatisticBuilder.CLASS_UNIQUE)
            {
                return ThemeColors.UNIQUE_BRUSH_GRADIENT;
            }
            else if (nameLower == StatisticBuilder.SPECIAL_CLASS_MOB)
            {
                return ThemeColors.MOB_BRUSH_GRADIENT;
            }
            else if (nameLower == StatisticBuilder.SPECIAL_CLASS_NORMAL)
            {
                return ThemeColors.NORMAL_BRUSH_GRADIENT;
            }
            else if (nameLower == StatisticBuilder.SPECIAL_CLASS_ELITE)
            {
                return ThemeColors.ELITE_BRUSH_GRADIENT;
            }
            else if (nameLower == StatisticBuilder.SPECIAL_CLASS_LEGEND)
            {
                return ThemeColors.LEGEND_BRUSH_GRADIENT;
            }

            return null;
        }
    }
}
