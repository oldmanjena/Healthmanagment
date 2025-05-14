using Healthmanagment.Klassen;
using Healthmanagment.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Healthmanagment.Klassen.Healthmanagment.Klassen;
using System.Windows.Controls.Primitives;

namespace Healthmanagment.Training
{
    /// <summary>
    /// Interaktionslogik für MuskelEin.xaml
    /// </summary>
    public partial class MuskelEin : Window, INotifyPropertyChanged
    {
        // Property für die Muskelgruppen, wird aus MuskelDaten bezogen
        public ObservableCollection<string> Muskelgruppen { get; set; }

        // Property für die Trainingseinträge in der DataGrid
        public ObservableCollection<TrainingEntry> TrainingEntries { get; set; } = new ObservableCollection<TrainingEntry>();

        // CollectionViewSource für die Anzeige der letzten Trainingseinheit
        private CollectionViewSource LetzteTrainingData { get; set; }

        // Verbindung zur Datenbank (sollte idealerweise in einer Konfigurationsdatei stehen)
        private readonly string connectionString = "data source=DESKTOP-726MH0T;initial catalog=gesundheit;trusted_connection=true";

        // Variable, um den "krank"-Status zu speichern
        private int kr = 0;

        public MuskelEin()
        {
            InitializeComponent();
            DataContext = new MuskelViewModel(); // Setze das ViewModel als DataContext

            // Beziehe die Muskelgruppen direkt aus der statischen Property
            Muskelgruppen = MuskelDaten.MuskelGruppen;
            cmbGruppe.ItemsSource = Muskelgruppen;

            // Setze die ComboBox auf das erste Element
            if (Muskelgruppen.Any())
            {
                cmbGruppe.SelectedIndex = 0;
            }

            // Initialisiere die ComboBoxen für Art und Technik
            cmbArt.ItemsSource = new[] { "Definition", "Masseaufbau", "Diät", "Normal" };
            cmbArt.SelectedIndex = 3; // Wählt "Normal" aus (Index 3)
            cmbTechnik.ItemsSource = new[] { "Aufwärmen", "Arbeitssatz", "Cool Down" };
            cmbTechnik.SelectedIndex = 0; // Wählt "Aufwärmen" aus (Index 0)

            // Lade die initialen Trainingsdaten
            Daten();

            // Event-Handler für das automatische Generieren von Spalten
            dtgUeber.AutoGeneratingColumn += DtgUeber_AutoGeneratingColumn;

            Debug.WriteLine($"MuskelEin initialisiert, Gruppen: {string.Join(", ", Muskelgruppen)}");
        }

        // Entferne diese redundante Property-Deklaration
        // public ObservableCollection<string> Muskelgruppen { get; set; }

        // Entferne diesen redundanten Konstruktor
        // public MuskelEin() { ... }

        private void cmbGruppe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbZiel.ItemsSource = null; // WICHTIG: ItemsSource aufheben, falls vorher gesetzt
            cmbZiel.Items.Clear();      // Jetzt erlaubt

            if (cmbGruppe.SelectedItem is string selectedGruppe)
            {
                // Verwende die statische Methode aus MuskelDaten, um die Zielmuskeln zu erhalten
                if (MuskelDaten.ZielMuskeln.TryGetValue(selectedGruppe, out var zielMuskeln))
                {
                    foreach (var muskel in zielMuskeln)
                    {
                        cmbZiel.Items.Add(muskel);
                    }
                }

                // Spezifische Logik für die Bauchmuskelgruppe (kann ggf. optimiert werden)
                // Die Sichtbarkeit von UI-Elementen sollte idealerweise über DataBinding im ViewModel gesteuert werden
                // und nicht direkt im Code-Behind.
                if (selectedGruppe == "Bauch")
                {
                    // Steuerung von UI-Elementen (besser über DataBinding)
                }
                else
                {
                    // Steuerung von UI-Elementen (besser über DataBinding)
                }
            }

            cmbZiel.SelectedIndex = 0;
        }

        private async void chkletztes_Checked(object sender, RoutedEventArgs e)
        {
            await LetzteAsync();
        }

        private async Task LetzteAsync()
        {
            string selectedUebung = txtUebung.Text;
            string CmdString = "SELECT TOP 1 CAST(Wann AS DATE) AS Wann, Uebung, Satz, Wh, Gewicht " +
                               "FROM Muskel " +
                               "WHERE Krank = 0 AND Uebung LIKE '%' + @uebung + '%' " +
                               "ORDER BY Wann DESC;";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(CmdString, con))
                    {
                        cmd.Parameters.AddWithValue("@uebung", selectedUebung);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            DataTable dt = new DataTable("letzteTraining");
                            dt.Load(reader);
                            LetzteTrainingData = new CollectionViewSource { Source = dt.DefaultView };
                            dtgUeber.ItemsSource = LetzteTrainingData.View;

                            if (dt.Rows.Count > 0)
                            {
                                if (decimal.TryParse(dt.Rows[0]["Gewicht"].ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal letztesGewicht))
                                {
                                    txtletztGewicht.Text = letztesGewicht.ToString("0.00", CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    txtletztGewicht.Text = string.Empty;
                                }
                                txtLetztWh.Text = dt.Rows[0]["Wh"].ToString();
                            }
                            else
                            {
                                txtletztGewicht.Text = string.Empty;
                                txtLetztWh.Text = string.Empty;
                            }
                        }
                    }
                }
                dtgUeber.Visibility = Visibility.Visible;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Datenbankfehler beim Abrufen der letzten Trainingsdaten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Allgemeiner Fehler beim Abrufen der letzten Trainingsdaten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ver_Click(object sender, RoutedEventArgs e)
        {
            string selectedGruppe = cmbGruppe.Text;
            string selectedUebung = txtUebung.Text;
            string selectedArt = cmbArt.Text;

            string query = @"
                SELECT TOP 1 Wann, art, Uebung, Zielmuskel, Satz, Wh, Gewicht
                FROM Muskel
                WHERE art = @art
                  AND muskelgruppe = @muskelgruppe
                  AND Uebung LIKE '%' + @uebung + '%'";

            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@art", selectedArt);
                    command.Parameters.AddWithValue("@muskelgruppe", selectedGruppe);
                    command.Parameters.AddWithValue("@uebung", selectedUebung);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }

                dtgUeber.ItemsSource = dataTable.DefaultView;
                dtgUeber.Visibility = Visibility.Visible;

                // Anpassen der Spaltenbreite und Ausrichtung (kann auch im XAML erfolgen)
                foreach (var column in dtgUeber.Columns)
                {
                    column.Width = DataGridLength.SizeToHeader;
                    column.HeaderStyle = new Style(typeof(DataGridColumnHeader))
                    {
                        Setters = { new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center) }
                    };
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Datenbankfehler beim Anzeigen der Vergleichsdaten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Allgemeiner Fehler beim Anzeigen der Vergleichsdaten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Daten()
        {
            string query = "SELECT Uebung, Wann, Wh, Gewicht FROM Muskel WHERE Art = 'Arbeitssatz' AND Gewicht > 0 ORDER BY Uebung, Wann";
            TrainingEntries.Clear(); // Stelle sicher, dass die Collection leer ist, bevor neue Daten hinzugefügt werden

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TrainingEntries.Add(new TrainingEntry
                            {
                                Uebung = reader["Uebung"].ToString(),
                                Wann = DateTime.Parse(reader["Wann"].ToString()),
                                Wh = int.Parse(reader["Wh"].ToString()),
                                Gewicht = double.Parse(reader["Gewicht"].ToString())
                            });
                        }
                    }
                }
                dtgUeber.ItemsSource = TrainingEntries;
                dtgUeber.Visibility = Visibility.Visible;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Datenbankfehler beim Laden der Trainingsdaten: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Allgemeiner Fehler beim Laden der Trainingsdaten: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class TrainingEntry
        {
            public string Uebung { get; set; }
            public DateTime Wann { get; set; }
            public int Wh { get; set; }
            public double Gewicht { get; set; }

            // Beispiel für eine formatierte Ausgabe im XAML
            // public string WannFormatted => Wann.ToString("dd.MM.yyyy");
        }

        private void DtgUeber_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Wann")
            {
                if (e.Column is DataGridTextColumn col)
                {
                    col.Binding = new Binding("Wann") { StringFormat = "dd.MM.yyyy" };
                }
            }
        }

        private void chkKrank_Checked(object sender, RoutedEventArgs e)
        {
            if (chkKrank.IsChecked == true)
            {
                chkNein.IsChecked = false;
                kr = 1;
                cmbGruppe.SelectedIndex = Muskelgruppen.IndexOf("Krank"); // Verwende IndexOf, um den richtigen Index zu finden
                Debug.WriteLine($"Krank-Status: {kr}");
            }
        }

        private void chkNein_Checked(object sender, RoutedEventArgs e)
        {
            if (chkNein.IsChecked == true)
            {
                chkKrank.IsChecked = false;
                kr = 0;
                Debug.WriteLine($"Krank-Status: {kr}");
            }
        }

        private void btn_Ende_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void ver()
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            decimal.TryParse(txtGewicht.Text, NumberStyles.Any, invariantCulture, out decimal aktuellesGewicht);
            decimal.TryParse(txtletztGewicht.Text, NumberStyles.Any, invariantCulture, out decimal letztesGewicht);
            decimal.TryParse(txtWh.Text, NumberStyles.Any, invariantCulture, out decimal aktuelleWh);
            decimal.TryParse(txtLetztWh.Text, NumberStyles.Any, invariantCulture, out decimal letzteWh);

            decimal gewichtVeraenderung = aktuellesGewicht - letztesGewicht;
            decimal whVeraenderung = aktuelleWh - letzteWh;

            txtVer.Text = gewichtVeraenderung.ToString("0.00");
            txtWHVer.Text = whVeraenderung.ToString("0.00");

            txtVerPro.Text = letztesGewicht != 0 ? ((gewichtVeraenderung / letztesGewicht) * 100).ToString("0.00") + "%" : "";
            txtWhProz.Text = letzteWh != 0 ? ((whVeraenderung / letzteWh) * 100).ToString("0.00") + "%" : "";

            // Die Veraenderung-Property im ViewModel wird hier nicht mehr direkt gesetzt,
            // da diese Logik primär die UI-Anzeige betrifft. Wenn das ViewModel diese
            // Information benötigt, sollte sie über DataBinding oder eine andere
            // geeignete Methode bereitgestellt werden.
        }

        private void txtGewicht_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Die ver()-Methode sollte idealerweise aufgerufen werden, wenn der Fokus
            // das Textfeld verlässt oder nach einer bestimmten Verzögerung, um
            // die Performance nicht unnötig zu belasten.
            // ver(); // Aktiviere dies, wenn die Berechnung bei jeder Textänderung erfolgen soll
        }

        private async void txtUebung_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtUebung.Text) && txtUebung.Text.Length > 2)
            {
                await LetzteAsync();
            }
        }

        

        private void txtGewicht_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ver(); // Deine Methode zur Berechnung der Veränderung
            // ValueChanged ist ein Event für NumberInput-Steuerelemente, nicht für TextBoxen.
            // Für TextBoxen ist TextChanged das relevante Event.
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}