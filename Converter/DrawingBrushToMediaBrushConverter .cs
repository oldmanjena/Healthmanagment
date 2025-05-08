using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Healthmanagment.Converter
{
    public class DrawingBrushToMediaBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Brush drawingBrush)
            {
                // Konvertiere den System.Drawing.Brush in einen System.Windows.Media.Brush
                if (drawingBrush is System.Drawing.SolidBrush solidBrush)
                {
                    // Wenn der Brush ein SolidBrush ist, konvertiere die Farbe
                    return new SolidColorBrush(System.Windows.Media.Color.FromArgb(
                        solidBrush.Color.A, solidBrush.Color.R, solidBrush.Color.G, solidBrush.Color.B));
                }
            }
            return Brushes.Gray; // Standardfarbe, falls keine g?ltige Farbe vorliegt
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Konvertierung zur?ck in System.Drawing.Brush (falls n?tig)
            if (value is SolidColorBrush solidColorBrush)
            {
                var color = solidColorBrush.Color;
                return new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
            }
            return System.Drawing.Brushes.Gray; // Standardwert f?r ung?ltige Eingaben
        }
    }
}

