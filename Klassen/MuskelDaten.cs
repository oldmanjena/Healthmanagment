using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthmanagment.Klassen
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    namespace Healthmanagment.Klassen
    {
        public static class MuskelDaten
        {
            public static Dictionary<string, List<string>> ZielMuskeln { get; } = new()
        {
            { "Krank", new List<string> { "krank" } },
            { "Rücken", new List<string> { "Latissimus", "Trapezius", "unterer Rücken" } },
            { "Beine", new List<string> { "Quadrizeps", "Harnstrings" } },
            { "Brust", new List<string> { "Obere Brust", "Mittlere Brust" } },
            { "Bizeps", new List<string> { "gesamter Bizeps" } },
            { "Trizeps", new List<string> { "gesamter Trizeps" } },
            { "Schulter", new List<string> { "vordere", "mittlere", "hintere" } },
            { "Bauch", new List<string> { "oberer", "gesamter", "unterer", "schräger" } },
            { "Unterarm", new List<string> { "gesamter" } },
            { "Ganzkörper", new List<string> { "gesamter" } }
        };

            public static ObservableCollection<string> MuskelGruppen { get; } = new(ZielMuskeln.Keys.ToList());

            public static List<string> GetZielMuskelnFuerGruppe(string gruppe)
            {
                return ZielMuskeln.TryGetValue(gruppe, out var muskeln) ? muskeln : new List<string>();
            }
        }
    }
}
