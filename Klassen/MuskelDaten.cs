using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthmanagment.Klassen
{
    public static class MuskelDaten
    {
        public static Dictionary<string, List<string>> ZielDict { get; } = new()
        {
            { "Krank", new List<string> { "krank" } },
            { "Ruecken", new List<string> { "Latissimus", "Trapezius", "unterer Rücken" } },
            { "Beine", new List<string> { "Quadrizeps", "Hamstrings" } },
            { "Brust", new List<string> { "Obere Brust", "Mittlere Brust" } },
            { "Bizeps", new List<string> { "gesamter Bizeps" } },
            { "Trizeps", new List<string> { "gesamter Trizeps" } },
            { "Schulter", new List<string> { "vordere", "mittlere", "hintere" } },
            { "Bauch", new List<string> { "oberer", "gesamter", "unterer", "schräger" } },
            { "Unterarm", new List<string> { "gesamter" } },
            { "Ganzkoerper", new List<string> { "gesamter" } }
        };
    }
}
