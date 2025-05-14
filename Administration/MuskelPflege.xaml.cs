using Healthmanagment.Klassen;
using Healthmanagment.Klassen.Healthmanagment.Klassen;
using System.Collections.ObjectModel;
using System.Windows;

namespace Healthmanagment.Administration
{
    public partial class MuskelPflege : Window
    {
        public ObservableCollection<MuskelEintrag> MuskelEintraegeListe { get; set; }

        public static List<string> VerfügbareGruppen => MuskelDaten.MuskelGruppen.ToList();

        public MuskelPflege()
        {
            InitializeComponent();

            MuskelEintraegeListe = new ObservableCollection<MuskelEintrag>(
                MuskelDaten.ZielMuskeln
                    .SelectMany(kvp => kvp.Value.Select(muskel =>
                        new MuskelEintrag { Muskelgruppe = kvp.Key, Zielmuskel = muskel }))
            );

            DataContext = this;
        }

        private void Speichern()
        {
            MuskelDaten.ZielMuskeln.Clear();

            foreach (var eintrag in MuskelEintraegeListe)
            {
                if (!MuskelDaten.ZielMuskeln.ContainsKey(eintrag.Muskelgruppe))
                {
                    MuskelDaten.ZielMuskeln[eintrag.Muskelgruppe] = new List<string>();
                }

                if (!MuskelDaten.ZielMuskeln[eintrag.Muskelgruppe].Contains(eintrag.Zielmuskel))
                {
                    MuskelDaten.ZielMuskeln[eintrag.Muskelgruppe].Add(eintrag.Zielmuskel);
                }
            }

            MuskelDaten.MuskelGruppen.Clear();
            foreach (var gruppe in MuskelDaten.ZielMuskeln.Keys)
                MuskelDaten.MuskelGruppen.Add(gruppe);
        }

    }
}