using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HuntAndPeck.Views
{
    // Builds the hint label brushes from the colours chosen in Options. Shared by
    // the overlay and the Options preview so the preview is the real thing.
    internal static class HintBrushes
    {
        private const string DefaultActive = "#F9CD33";
        private const string DefaultInactive = "#E6C030";

        private static readonly Dictionary<string, Brush> Cache = new Dictionary<string, Brush>();

        public static Brush Background(string color)
        {
            switch (color)
            {
                // Hand-tuned gradients for the stock colours, kept byte-for-byte.
                case DefaultActive:
                    return Gradient("#FFFEF5", "#FFE44D", "#F5D033", "#E8C028");
                case DefaultInactive:
                    return Gradient("#F5F3E8", "#EDDA3F", "#E0C230", "#D4B022");
            }

            var c = Parse(color, DefaultActive);
            return Gradient(Mix(c, Colors.White, 0.9), Mix(c, Colors.White, 0.25), c, Mix(c, Colors.Black, 0.08));
        }

        public static Brush Border(string color)
        {
            switch (color)
            {
                case DefaultActive: return Solid("#C9A825");
                case DefaultInactive: return Solid("#B89920");
            }
            return Frozen(new SolidColorBrush(Mix(Parse(color, DefaultActive), Colors.Black, 0.2)));
        }

        public static Brush Solid(string color)
        {
            if (!Cache.TryGetValue(color ?? "", out var brush))
            {
                brush = Cache[color ?? ""] = Frozen(new SolidColorBrush(Parse(color, "Black")));
            }
            return brush;
        }

        private static Brush Gradient(string a, string b, string c, string d)
        {
            return Gradient(Parse(a, a), Parse(b, b), Parse(c, c), Parse(d, d));
        }

        private static Brush Gradient(Color a, Color b, Color c, Color d)
        {
            return Frozen(new LinearGradientBrush(new GradientStopCollection
            {
                new GradientStop(a, 0), new GradientStop(b, 0.3), new GradientStop(c, 0.7), new GradientStop(d, 1)
            }, 90));
        }

        private static Brush Frozen(Brush brush)
        {
            brush.Freeze();
            return brush;
        }

        private static Color Parse(string color, string fallback)
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(color);
            }
            catch (Exception)
            {
                return (Color)ColorConverter.ConvertFromString(fallback);
            }
        }

        private static Color Mix(Color a, Color b, double t)
        {
            return Color.FromRgb(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t));
        }
    }

    // XAML access for the Options preview. ConverterParameter "border" gives the edge colour.
    public class HintBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as string;
            return (parameter as string) == "border" ? HintBrushes.Border(color) : HintBrushes.Background(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
