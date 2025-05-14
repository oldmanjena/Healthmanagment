using Healthmanagment.Klassen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Healthmanagment.Klassen.Healthmanagment.Klassen;

namespace Healthmanagment.ViewModel
{
    public class MuskelViewModel : INotifyPropertyChanged
    {
        // #region Private Felder

        private ObservableCollection<string> _muskelgruppenListe; // Um Verwechslungen zu vermeiden
        private ObservableCollection<string> _zielmuskelnListe = new ObservableCollection<string>();
        private DateTime? _selectedDate = DateTime.Now;
        private string _ausgewaehlteMuskelgruppe;
        private string _uebung;
        private int _satz;
        private int _wiederholungen;
        private decimal _gewicht;
        private decimal _veraenderung;
        private string _art;
        private string _technik;
        private int _trainNr;
        private ObservableCollection<string> _trainingsnummernListe;
        private bool _istKrank;
        private bool _istNichtKrank = true; // Standardwert
        private ICollectionView _letzteTrainingDataView;
        private ObservableCollection<string> _zieleListe = new ObservableCollection<string>();
        private bool _istDtgUeberSichtbar = true;
        private bool _istDtgAndereDatenSichtbar;
        private ObservableCollection<AufwaermSatz> _aufwaermDatenListe = new ObservableCollection<AufwaermSatz>();
        private ICollectionView _aufwaermDatenView;

        // #endregion

        // #region Öffentliche Properties

        public TimePickerViewModel TimePickerVM { get; } = new TimePickerViewModel(); // Direkt initialisieren

        public ObservableCollection<string> MuskelgruppenListe
        {
            get => _muskelgruppenListe;
            set
            {
                if (_muskelgruppenListe != value)
                {
                    _muskelgruppenListe = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> ZielmuskelnListe
        {
            get => _zielmuskelnListe;
            set
            {
                if (_zielmuskelnListe != value)
                {
                    _zielmuskelnListe = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AusgewaehlteMuskelgruppe // Klarerer Name
        {
            get => _ausgewaehlteMuskelgruppe;
            set
            {
                if (_ausgewaehlteMuskelgruppe != value)
                {
                    _ausgewaehlteMuskelgruppe = value;
                    OnPropertyChanged();
                    LadeZieleFuerGruppe(value); // Logik zum Laden der Ziele direkt hier
                }
            }
        }

        public string Uebung
        {
            get => _uebung;
            set
            {
                if (_uebung != value)
                {
                    _uebung = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Satz
        {
            get => _satz;
            set
            {
                if (_satz != value)
                {
                    _satz = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Satz wurde gesetzt: {_satz}");
                }
            }
        }

        public int Wiederholungen
        {
            get => _wiederholungen;
            set
            {
                if (_wiederholungen != value)
                {
                    _wiederholungen = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Wiederholungen wurde gesetzt: {_wiederholungen}");
                }
            }
        }

        public decimal Gewicht
        {
            get => _gewicht;
            set
            {
                if (_gewicht != value)
                {
                    _gewicht = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Gewicht wurde gesetzt: {_gewicht}");
                }
            }
        }

        public decimal Veraenderung
        {
            get => _veraenderung;
            set
            {
                if (_veraenderung != value)
                {
                    _veraenderung = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Art
        {
            get => _art;
            set
            {
                if (_art != value)
                {
                    _art = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Technik
        {
            get => _technik;
            set
            {
                if (_technik != value)
                {
                    _technik = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TrainNr
        {
            get => _trainNr;
            set
            {
                if (_trainNr != value)
                {
                    _trainNr = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> TrainingsnummernListe
        {
            get => _trainingsnummernListe;
            set
            {
                _trainingsnummernListe = value;
                OnPropertyChanged();
            }
        }

        public bool IstKrank
        {
            get => _istKrank;
            set
            {
                if (_istKrank != value)
                {
                    _istKrank = value;
                    OnPropertyChanged();
                    if (value) IstNichtKrank = false;
                }
            }
        }

        public bool IstNichtKrank
        {
            get => _istNichtKrank;
            set
            {
                if (_istNichtKrank != value)
                {
                    _istNichtKrank = value;
                    OnPropertyChanged();
                    if (value) IstKrank = false;
                }
            }
        }

        public ICollectionView LetzteTrainingDataView
        {
            get => _letzteTrainingDataView;
            set
            {
                _letzteTrainingDataView = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ZieleListe
        {
            get => _zieleListe;
            set
            {
                _zieleListe = value;
                OnPropertyChanged();
                Debug.WriteLine($"[VM] Neue Ziele gesetzt: {string.Join(", ", _zieleListe)}");
            }
        }

        public bool IstDtgUeberSichtbar
        {
            get => _istDtgUeberSichtbar;
            set
            {
                if (_istDtgUeberSichtbar != value)
                {
                    _istDtgUeberSichtbar = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedZielmuskel;

        public string SelectedZielmuskel
        {
            get => _selectedZielmuskel;
            set
            {
                if (_selectedZielmuskel != value)
                {
                    _selectedZielmuskel = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IstDtgAndereDatenSichtbar
        {
            get => _istDtgAndereDatenSichtbar;
            set
            {
                if (_istDtgAndereDatenSichtbar != value)
                {
                    _istDtgAndereDatenSichtbar = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<AufwaermSatz> AufwaermDatenListe
        {
            get => _aufwaermDatenListe;
            set
            {
                _aufwaermDatenListe = value;
                OnPropertyChanged();
            }
        }


        public ICollectionView AufwaermDatenView
        {
            get => _aufwaermDatenView;
            set
            {
                _aufwaermDatenView = value;
                OnPropertyChanged();
            }
        }

        public List<string> Arten { get; } = new() { "Definition", "Masseaufbau", "Diät", "Normal" };
        public List<string> Techniken { get; } = new() { "Aufwärmen", "Arbeitssatz", "Cool Down" };

        // #endregion

        // #region Commands

        public ICommand EintragenCommand { get; }
        public ICommand AnzeigenAufwaermenCommand { get; }
        public ICommand ZeigeAndereDatenCommand { get; }

        // #endregion

        // #region Konstruktor

        public MuskelViewModel()
        {
            MuskelgruppenListe = MuskelDaten.MuskelGruppen; // Zugriff auf die statische Property
            EintragenCommand = new RelayCommand(EintragHinzufuegen, KannEintragen);
            AnzeigenAufwaermenCommand = new RelayCommand(AusfuehrenAnzeigenAufwaermen);
            ZeigeAndereDatenCommand = new RelayCommand(AusfuehrenZeigeAndereDaten);

            Art = Arten[0];
            Technik = Techniken[0];
            AusgewaehlteMuskelgruppe = MuskelgruppenListe.FirstOrDefault(); // Standardauswahl

            LadeTrainingsnummern(); // <<<<< NEU
        }

        // #endregion

        // #region Methoden

        private void LadeZieleFuerGruppe(string gruppe)
        {
            if (string.IsNullOrEmpty(gruppe))
            {
                ZieleListe.Clear();
                return;
            }

            var ziele = MuskelDaten.GetZielMuskelnFuerGruppe(gruppe);
            ZieleListe.Clear();
            foreach (var ziel in ziele)
            {
                ZieleListe.Add(ziel);
            }
            Debug.WriteLine($"[VM] Ziele für {gruppe} geladen: {string.Join(", ", ZieleListe)}");
        }

        private bool KannEintragen(object obj)
        {
            Debug.WriteLine("KannEintragen wurde aufgerufen");
            return true; // Füge hier deine Logik hinzu, wann ein Eintrag gespeichert werden kann
        }

        private void EintragHinzufuegen(object obj)
        {
            Debug.WriteLine("EintragHinzufuegen wurde aufgerufen");
            try
            {
                MessageBox.Show("Eintrag wird ausgeführt.", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);

                string connectionString = ConfigurationManager.ConnectionStrings["managment"].ConnectionString;
                using SqlConnection con = new(connectionString);
                con.Open();

                using SqlCommand cmd = new("INSERT INTO Muskel(wann, Muskelgruppe, Uebung, Zielmuskel, Satz, Wh, gewicht, veraenderung, art, krank, Technik, Trainnr) " +
                                            "VALUES (@wann, @Muskelgruppe, @Uebung, @Zielmuskel, @Satz, @Wh, @gewicht, @veraenderung, @art, @krank, @Technik, @Trainnr)", con);
                cmd.Parameters.AddWithValue("@wann", SelectedDate); // Verwende SelectedDate
                cmd.Parameters.AddWithValue("@Muskelgruppe", AusgewaehlteMuskelgruppe); // Verwende AusgewaehlteMuskelgruppe
                cmd.Parameters.AddWithValue("@Uebung", Uebung);
                if (!string.IsNullOrWhiteSpace(SelectedZielmuskel))
                {
                    cmd.Parameters.AddWithValue("@Zielmuskel", SelectedZielmuskel);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Zielmuskel", DBNull.Value);
                }
                cmd.Parameters.AddWithValue("@Satz", Satz);
                cmd.Parameters.AddWithValue("@Wh", Wiederholungen);
                cmd.Parameters.AddWithValue("@gewicht", Gewicht);
                cmd.Parameters.AddWithValue("@veraenderung", Veraenderung);
                cmd.Parameters.AddWithValue("@art", Art);
                cmd.Parameters.AddWithValue("@krank", IstKrank ? 1 : 0);
                cmd.Parameters.AddWithValue("@Technik", Technik);
                cmd.Parameters.AddWithValue("@Trainnr", TrainNr);

                cmd.ExecuteNonQuery();

                // Hier könntest du optional die lokale Collection aktualisieren,
                // wenn du die Daten auch im ViewModel halten möchtest.

                MessageBox.Show("Muskeleintrag erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Datenbankfehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AusfuehrenZeigeAndereDaten(object obj)
        {
            IstDtgUeberSichtbar = false;
            IstDtgAndereDatenSichtbar = true;
            // Hier ggf. Logik zum Laden von Daten für das zweite DataGrid
        }

        private void AusfuehrenAnzeigenAufwaermen(object obj)
        {
            string selectedGruppe = AusgewaehlteMuskelgruppe;
            string selectedUebung = Uebung;
            string selectedArt = Art;

            string conString = ConfigurationManager.ConnectionStrings["managment"].ConnectionString;
            string cmdString;

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    cmdString = "SELECT Uebung, MaxGewicht, " +
                                "MaxGewicht * 0.4 AS Satz1, " +
                                "MaxGewicht * 0.5 AS Satz2, " +
                                "MaxGewicht * 0.6 AS Satz3 " +
                                "FROM (SELECT Uebung, MAX(training) AS MaxGewicht " +
                                "FROM Max " +
                                "WHERE Muskel = @muskelgruppe " +
                                "AND Uebung LIKE '%' + @uebung + '%' " +
                                "GROUP BY Uebung) AS MaxGewichtSubquery " +
                                "ORDER BY Uebung DESC;";

                    SqlCommand cmd = new SqlCommand(cmdString, con);
                    cmd.Parameters.AddWithValue("@muskelgruppe", selectedGruppe);
                    cmd.Parameters.AddWithValue("@uebung", selectedUebung);

                    SqlDataReader reader = cmd.ExecuteReader();
                    AufwaermDatenListe.Clear();

                    while (reader.Read())
                    {
                        AufwaermDatenListe.Add(new AufwaermSatz
                        {
                            Uebung = reader["Uebung"].ToString(),
                            MaxGewicht = Convert.ToDecimal(reader["MaxGewicht"]),
                            Satz1 = Convert.ToDecimal(reader["Satz1"]),
                            Satz2 = Convert.ToDecimal(reader["Satz2"]),
                            Satz3 = Convert.ToDecimal(reader["Satz3"])
                        });
                    }

                    AufwaermDatenView = CollectionViewSource.GetDefaultView(AufwaermDatenListe);
                    IstDtgUeberSichtbar = false;
                    IstDtgAndereDatenSichtbar = true; // Zeige das DataGrid für Aufwärmen
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Datenbankfehler beim Abrufen der Aufwärmdaten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // #endregion

        // #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // #endregion

        // #region Hilfsklasse

        public class AufwaermSatz
        {
            public string Uebung { get; set; }
            public decimal MaxGewicht { get; set; }
            public decimal Satz1 { get; set; }
            public decimal Satz2 { get; set; }
            public decimal Satz3 { get; set; }
        }

        // #endregion
    

    private void LadeTrainingsnummern()
        {
            TrainingsnummernListe = new ObservableCollection<string>();

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["managment"].ConnectionString;
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string query = "SELECT Plan_Nr FROM Planung WHERE erledigt < 1";
                using SqlCommand command = new(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TrainingsnummernListe.Add(reader["Plan_Nr"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Trainingsnummern: " + ex.Message);
            }
        }
    }
}