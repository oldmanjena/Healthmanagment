using System;
using System.Globalization;
using System.Windows.Data;

namespace Healthmanagment.Converter
{
    public class MaskedTextToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                // R?ckgabe der Zahl mit 2 Dezimalstellen
                return decimalValue.ToString("0.00", culture);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(text))
                return null;

            // Komma durch Punkt ersetzen, falls im deutschen Kontext Komma verwendet wurde
            text = text.Replace(",", ".");

            // Wenn die Eingabe unvollst?ndig ist, z.?B. "12.", zur?ckgeben
            if (text.EndsWith("."))
                return null;

            // Versuchen, die Eingabe als decimal zu parsen
            if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }

            // Bei ung?ltiger Eingabe null zur?ckgeben
            return null;
        }
    }
}

