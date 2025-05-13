using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Healthmanagment.Converter
{
    public class TimeSpanHHMMSSFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is TimeSpan ts)
            {
                if (format == "HH:mm:ss")
                {
                    return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
                }
            }
            return arg?.ToString();
        }
    }
}
