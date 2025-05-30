using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthmanagment.Klassen
{
    public class TrainingsEintrag
    {
        public DateTime Datum { get; set; }
        public int KW { get; set; }
        public string Wochentag { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? Dauer { get; set; }
        public int RPM { get; set; }
        public double Entfernung { get; set; }
        public decimal Kcal { get; set; }
        public int Puls { get; set; }
        public double Aerober { get; set; }
        public double Anaerober { get; set; }
        public decimal Regeneration { get; set; }
        public int VO2max { get; set; }
        public int PlanNr { get; set; }
        public string Kommentar { get; set; }
        public double effekt { get; set; }
    }
}

