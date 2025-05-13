using System;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Healthmanagment;
using Healthmanagment.Klassen;
using Healthmanagment.Converter;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Text.RegularExpressions;

namespace Healthmanagment.ViewModel
{

    public class TrainingsViewModel : ViewModelBase
    {
        public ObservableCollection<TrainingsEintrag> TrainingsDaten { get; set; }

        public ICommand EintragenCommand { get; }

        


        public TrainingsViewModel()
        {
            TrainingsDaten = new ObservableCollection<TrainingsEintrag>();
            EintragenCommand = new RelayCommand(EintragHinzufuegen);
            Start = DateTime.Now.TimeOfDay;
            Debug.WriteLine($"Startzeit gesetzt auf: {Start}");
            _ = LadePlaeneAsync();  // Das "_" bedeutet, dass wir nicht auf das Ergebnis warten, aber es wird trotzdem ausgeführt


        }

        public async Task LadePlaeneAsync()
        {
            await LadePlaene();  // Hier rufst du die Methode weiterhin auf, aber sie muss eine Task zurückgeben.
        }



        // Properties gebunden an UI
        //  public DateTime Datum { get; set; } = DateTime.Now;
        public int KW { get; set; }
      // public DateTime Wochentag { get; set; } = DateTime.Now; // In DB als datetime? -> pr?fen!
      //  public TimeSpan? Start { get; set; } 
      //  public TimeSpan? Dauer { get; set; }

        public int RPM { get; set; }
      //  public double Entfernung { get; set; }
       // public decimal Kcal { get; set; }  // Anpassung
        public int Puls { get; set; }
        public double Aerober { get; set; }
        public double Anaerober { get; set; } // Anearober = Anaerober
     //   public decimal? Regeneration { get; set; }
        public int VO2max { get; set; }
      //  public int PlanNr { get; set; }  // statt int
      //  public string Kommentar { get; set; } = "";

        public int Per_id { get; set; }
        public double Puls_Max { get; set; }

        //public int effekt { get; set; }

        private DateTime? _datum = DateTime.Today;
        public DateTime? Datum
        {
            get => _datum;
            set
            {
                if (_datum != value)
                {
                    _datum = value;
                    KW = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(_datum ?? DateTime.Today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    OnPropertyChanged(nameof(Datum));
                    OnPropertyChanged(nameof(Wochentag));
                    OnPropertyChanged(nameof(KW));  // Stelle sicher, dass KW auch aktualisiert wird
                }
            }
        }

        public string Wochentag => Datum?.ToString("dddd", new CultureInfo("de-DE")) ?? string.Empty;





        private void EintragHinzufuegen(object obj)
        {
            try
            {
                // Testcode zur Überprüfung der TimeSpan-Formatierung
               //TimeSpan testTimeSpan = new TimeSpan(1, 8, 34);
                //string testString = testTimeSpan.ToString("HH:mm:ss");
                //Debug.WriteLine($"Test TimeSpan format: {testString}");

                string connectionString = ConfigurationManager.ConnectionStrings["managment"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Training(Datum, KW, Wochentag, Start, Dauer, RPM, Entfernung, kcal, Puls, Aerober, Anearober, Regenaration, VO2max, Plan_Nr, Kommentar, effekt) " +
                        "VALUES (@Datum, @KW, @Wochentag,CONVERT(time, @Start, 108), CONVERT(time, @Dauer, 108), @RPM, @Entfernung, @kcal, @Puls, @Aerober, @Anearober, @Regeneration, @VO2max, @Plan_Nr, @Kommentar, @effekt)", con))
                    {
                        TimeSpanHHMMSSFormatter formatter = new TimeSpanHHMMSSFormatter();
                        cmd.Parameters.Add("@Datum", SqlDbType.Date).Value = Datum.HasValue ? Datum.Value.Date : DBNull.Value;
                        cmd.Parameters.Add("@KW", SqlDbType.Int).Value = KW;
                        cmd.Parameters.Add("@Wochentag", SqlDbType.VarChar, 20).Value = Wochentag ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Start", SqlDbType.Time).Value = new TimeSpan(20, 20, 41);  // Testwert
                        cmd.Parameters.Add("@Dauer", SqlDbType.Time).Value = new TimeSpan(3, 0, 0);  // Testwert

                        cmd.Parameters.Add("@RPM", SqlDbType.Int).Value = RPM;
                        cmd.Parameters.Add("@Entfernung", SqlDbType.Decimal).Value = Entfernung;
                        cmd.Parameters.Add("@kcal", SqlDbType.Decimal).Value = Kcal;
                        cmd.Parameters.Add("@Puls", SqlDbType.Int).Value = Puls;
                        cmd.Parameters.Add("@Aerober", SqlDbType.Float).Value = Aerober;
                        cmd.Parameters.Add("@Anearober", SqlDbType.Float).Value = Anaerober;
                        cmd.Parameters.Add("@Regeneration", SqlDbType.Decimal).Value = Regeneration;

                        cmd.Parameters.Add("@VO2max", SqlDbType.Int).Value = VO2max;
                        cmd.Parameters.Add("@Plan_Nr", SqlDbType.Int).Value = PlanNr;
                        cmd.Parameters.Add("@Kommentar", SqlDbType.VarChar, 200).Value = Kommentar ?? (object)DBNull.Value;
                          
                        cmd.Parameters.Add("@effekt", SqlDbType.Float).Value = effekt;


                        foreach (SqlParameter p in cmd.Parameters)
                            Debug.WriteLine($"{p.ParameterName} = {p.Value} ({p.Value?.GetType()})");

                        cmd.ExecuteNonQuery();
                    }
                }

                TrainingsDaten.Add(new TrainingsEintrag
                {
                    Datum = Datum ?? DateTime.MinValue,
                    KW = KW,
                    Wochentag = Wochentag,
                    Start = Start,
                    Dauer = Dauer,
                    RPM = RPM,
                    Entfernung = (double)Entfernung,
                    Kcal = (decimal)Kcal,
                    Puls = Puls,
                    Aerober = Aerober,
                    Anaerober = Anaerober,
                    Regeneration = (decimal)Regeneration,
                    VO2max = VO2max,
                    PlanNr = PlanNr,
                    Kommentar = Kommentar,
                    effekt = effekt
                });

                MessageBox.Show("Eintrag erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Datenbankfehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SetDatumExtern(DateTime datum)
        {
            Datum = datum;
            KW = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(datum, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
           // Wochentag = datum.ToString("dddd", new CultureInfo("de-DE"));
        }

        private TrainingsEintrag _neuerEintrag = new TrainingsEintrag();
        public TrainingsEintrag NeuerEintrag
        {
            get => _neuerEintrag;
            set
            {
                _neuerEintrag = value;
                OnPropertyChanged(nameof(NeuerEintrag));
            }
        }

        private DateTime? _dauerZeit;
        public DateTime? DauerZeit
        {
            get => _dauerZeit;
            set
            {
                _dauerZeit = value;
                OnPropertyChanged(nameof(DauerZeit));

                // Konvertiere zu TimeSpan?
                Dauer = value?.TimeOfDay;
                Debug.WriteLine($"[DEBUG] Dauer berechnet aus DauerZeit: {Dauer?.ToString() ?? "NULL"}");
            }
        }

        private TimeSpan? _dauer;
        public TimeSpan? Dauer
        {
            get => _dauer;
            set
            {
                _dauer = value;
                OnPropertyChanged(nameof(Dauer));
            }
        }



        private TimeSpan? _start;
        public TimeSpan? Start
        {
            get => _start;
            set
            {
                if (_start != value)
                {
                    _start = value;
                    OnPropertyChanged(nameof(Start));
                    Debug.WriteLine($"Start wurde auf {_start} gesetzt.");
                    // KW berechnen
                    if (_start.HasValue)
                    {
                        var dateTime = DateTime.Today + _start.Value;
                        KW = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            dateTime,
                            CalendarWeekRule.FirstFourDayWeek,
                            DayOfWeek.Monday
                        );
                    }
                    else
                    {
                        KW = 0;
                    }
                }
            }
        }





        private string _kommentar;
        public string Kommentar
        {
            get => _kommentar;
            set
            {
                if (_kommentar != value)
                {
                    _kommentar = value;
                    OnPropertyChanged(); // OnPropertyChanged f?r die Kommentar-Property aufrufen
                    Debug.WriteLine($"Kommentar (Property Setter): {_kommentar}"); // F?ge diese Debug-Ausgabe hinzu
                }
            }
        }

        private decimal _entfernung;
        public decimal Entfernung
        {
            get => _entfernung;
            set
            {
                if (_entfernung != value)
                {
                    _entfernung = value;
                    OnPropertyChanged(nameof(Entfernung));
                }
            }
        }


        private int _planNr;
        public int PlanNr
        {
            get => _planNr;
            set
            {
                if (_planNr != value)
                {
                    _planNr = value;
                    OnPropertyChanged(nameof(PlanNr));
                }
            }
        }

        private double _effekt;
        public double effekt
        {
            get => _effekt;
            set
            {
                if (_effekt != value)
                {
                    _effekt = value;
                    OnPropertyChanged(nameof(effekt));
                }
            }
        }


        private decimal _regeneration;
        public decimal Regeneration
        {
            get => _regeneration;
            set
            {
                if (_regeneration != value)
                {
                    _regeneration = value;
                    OnPropertyChanged(nameof(Regeneration));
                    Debug.WriteLine($"?? Regeneration gesetzt: {_regeneration}");
                }
            }
        }


        public ObservableCollection<string> PlanNrList { get; set; } = new();

        public async Task LadePlaene()
        {
            string connectionString = "data source=DESKTOP-726MH0T;initial catalog=managment;trusted_connection=true";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Planung WHERE erledigt < 1";
                SqlCommand cmd = new SqlCommand(query, con);

                int spalten_nr = 4; // z.?B. "PlanNummer"

                try
                {
                    await con.OpenAsync();
                    SqlDataReader dr = await cmd.ExecuteReaderAsync();

                    PlanNrList.Clear();
                    while (await dr.ReadAsync())
                    {
                        PlanNrList.Add(dr.GetValue(spalten_nr).ToString());
                    }

                    dr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden der Daten: " + ex.Message);
                }
            }
        }


        private decimal _kcal;
        public decimal Kcal
        {
            get => _kcal;
            set
            {
                if (_kcal != value)
                {
                    _kcal = value;
                    Debug.WriteLine($"?? Kcal gesetzt: {_kcal}");
                    OnPropertyChanged(nameof(Kcal));
                }
            }
        }



    }
}


