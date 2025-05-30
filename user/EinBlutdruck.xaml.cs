using System.Configuration;
using System.Data;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Windows;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;
using SystemColors = System.Windows.SystemColors;

using Healthmanagment.ViewModel;


namespace Healthmanagment
{
    /// <summary>
    /// Interaktionslogik f?r EinBlutdruck.xaml
    /// </summary>
    public partial class EinBlutdruck : UserControl
    {
        public DateTime? SelectedTime { get; set; }
        public EinBlutdruck()
        {
            InitializeComponent();

            txtZeit.Text = DateTime.Now.ToString("HH:mm");
            DataContext = this; // Stelle sicher, dass der DataContext gesetzt wird
           // MessageBox.Show($"SelectedTime: {SelectedTime}"); // Debug-Ausgabe

        }

       
        private string BerechneTageszeit(DateTime zeit)
        {
            int stunde = zeit.Hour;
            return stunde switch
            {
                >= 5 and < 12 => "Morgen",
                >= 12 and < 14 => "Mittag",
                >= 14 and < 18 => "Nachmittag",
                >= 18 and < 22 => "Abend",
                _ => "Nacht"
            };
        }

     
      

        private void Eintragen()
        {
            


            if (!int.TryParse(txtSys.Text, out int sys) ||
                !int.TryParse(txtDia.Text, out int dia) ||
                !int.TryParse(txtPuls.Text, out int puls))
            {
                MessageBox.Show("Bitte g?ltige Werte f?r Blutdruck und Puls eingeben.");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["gesundheit"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Blutdruck(Datum, Uhrzeit, Systole, Diastole, Puls, Tageszeit, Bemerkung) VALUES (@wann, @zeit, @sys, @dia, @puls, @tag, @bemer)", con))
                {
                    cmd.Parameters.AddWithValue("@wann", dtpDatum.SelectedDate ?? DateTime.Now);
                    DateTime zeit;
                    if (DateTime.TryParseExact(txtZeit.Text, "HH:mm", null, System.Globalization.DateTimeStyles.None, out zeit))
                    {
                        // Der Benutzer hat eine g?ltige Uhrzeit eingegeben
                        cmd.Parameters.AddWithValue("@zeit", zeit);
                    }
                    else
                    {
                        // Ung?ltige Eingabe oder leeres Textfeld, daher aktuelle Zeit verwenden
                        cmd.Parameters.AddWithValue("@zeit", DateTime.Now);
                    }
                    cmd.Parameters.AddWithValue("@sys", sys);
                    cmd.Parameters.AddWithValue("@dia", dia);
                    cmd.Parameters.AddWithValue("@puls", puls);
                    cmd.Parameters.AddWithValue("@tag", txtTageszeit.Text);
                    cmd.Parameters.AddWithValue("@bemer", txtBemerkung.Text);
                    cmd.ExecuteNonQuery();
                }
            }
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["gesundheit"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT Datum, Uhrzeit, Systole, Diastole, Puls, Tageszeit, Bemerkung FROM Blutdruck ORDER BY Datum DESC, Uhrzeit DESC";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Die Daten an das DataGrid binden
                dtgDaten.ItemsSource = dt.DefaultView;
            }
        }





        private void BtnEnde_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

       

        private void txtZeit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DateTime.TryParseExact(txtZeit.Text, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedTime))
            {
                SelectedTime = parsedTime;
            }
            else
            {
                // Fehlerbehandlung: Ung?ltige Eingabe
                MessageBox.Show("Bitte geben Sie eine g?ltige Uhrzeit im Format HH:mm ein.");
            }
        }

        private void btnEintragen_Click_1(object sender, RoutedEventArgs e)
        {
            Eintragen();           
            MessageBox.Show("Daten wurden aktualisiert!");
        }


        private void CheckSystolicBloodPressure()
        {
            if (int.TryParse(txtSys.Text, out int systolic))
            {
                if (systolic >= 140) // Zu hoch nach WHO
                {
                    txtSys.Background = new SolidColorBrush(Colors.Red);
                    txtSys.Foreground = new SolidColorBrush(Colors.White);
                    UpdateTextBox("Systole zu hoch.");
                }
                else if (systolic >= 90 && systolic < 140) // Normal nach WHO
                {
                    txtSys.Background = new SolidColorBrush(Colors.Green);
                    txtSys.Foreground = new SolidColorBrush(Colors.White);
                    txtSys.FontWeight = FontWeights.Bold;  // Verwende WPF spezifische Schriftstile
                    UpdateTextBox("Systole normal.");
                }
                else if (systolic < 90) // Zu niedrig nach WHO
                {
                    txtSys.Background = new SolidColorBrush(Colors.Yellow);
                    txtSys.Foreground = new SolidColorBrush(Colors.Black);
                    UpdateTextBox("Systole zu niedrig.");
                }
            }
            else
            {
                // Falls die Eingabe ung?ltig ist
                txtSys.Background = SystemColors.WindowBrush;
                txtSys.Foreground = SystemColors.WindowTextBrush;
                UpdateTextBox("Ung?ltige Eingabe.");
            }
        }

        private void CheckDiastolicBloodPressure()
        {
            if (int.TryParse(txtDia.Text, out int diastolic))
            {
                if (diastolic >= 90) // Zu hoch nach WHO
                {
                    txtDia.Background = new SolidColorBrush(Colors.Red);
                    txtDia.Foreground = new SolidColorBrush(Colors.White);
                    UpdateTextBox("Diastole zu hoch.");
                }
                else if (diastolic >= 60 && diastolic < 90) // Normal nach WHO
                {
                    txtDia.Background = new SolidColorBrush(Colors.Green);
                    txtDia.Foreground = new SolidColorBrush(Colors.White);
                    txtDia.FontWeight = FontWeights.Bold;  // Verwende WPF spezifische Schriftstile
                    UpdateTextBox("Diastole normal.");
                }
                else if (diastolic < 60) // Zu niedrig nach WHO
                {
                    txtDia.Background = new SolidColorBrush(Colors.Yellow);
                    txtDia.Foreground = new SolidColorBrush(Colors.Black);
                    UpdateTextBox("Diastole zu niedrig.");
                }
            }
            else
            {
                txtDia.Background = SystemColors.WindowBrush;
                txtDia.Foreground = SystemColors.WindowTextBrush;
                UpdateTextBox("Ung?ltige Eingabe.");
            }
        }

        private void CheckPulse()
        {
            if (int.TryParse(txtPuls.Text, out int pulse))
            {
                if (pulse > 100) // Zu hoch nach Standard
                {
                    txtPuls.Background = new SolidColorBrush(Colors.Red);
                    txtPuls.Foreground = new SolidColorBrush(Colors.White);
                    UpdateTextBox("Puls zu hoch.");
                }
                else if (pulse >= 60 && pulse <= 100) // Normaler Puls
                {
                    txtPuls.Background = new SolidColorBrush(Colors.Green);
                    txtPuls.Foreground = new SolidColorBrush(Colors.White);
                    txtPuls.FontWeight = FontWeights.Bold;  // Verwende WPF spezifische Schriftstile
                    UpdateTextBox("Puls normal.");
                }
                else if (pulse < 60) // Zu niedrig
                {
                    txtPuls.Background = new SolidColorBrush(Colors.Yellow);
                    txtPuls.Foreground = new SolidColorBrush(Colors.Black);
                    UpdateTextBox("Puls zu niedrig.");
                }
            }
            else
            {
                txtPuls.Background = SystemColors.WindowBrush;
                txtPuls.Foreground = SystemColors.WindowTextBrush;
                UpdateTextBox("Ung?ltige Eingabe.");
            }
        }

        private void txtSys_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckSystolicBloodPressure();
        }

        private void txtDia_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckDiastolicBloodPressure();
        }

        private void txtPuls_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckPulse();
        }

        private void UpdateTextBox(string message)
        {
            // F?ge die Nachricht in einer neuen Zeile hinzu
            txtBemerkung.Text += Environment.NewLine + message;

            // Optional: Zum Ende des Textes scrollen
            txtBemerkung.ScrollToEnd();
        }

        private void btnPruefen_Click(object sender, RoutedEventArgs e)
        {
            //AuswertungWindow auswertung = new AuswertungWindow();
           // auswertung.ShowDialog(); // Jetzt funktioniert ShowDialog()
        }

        private void txtZeit_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (DateTime.TryParse(txtZeit.Text, out DateTime zeit)) // ?berpr?fung auf g?ltige Zeit
            {
                txtTageszeit.Text = BerechneTageszeit(zeit);
            }
            else
            {
                txtTageszeit.Text = "Ung?ltige Zeit"; // Fehlerbehandlung
            }
        }

        private void txtBemerkung_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FillDataGrid()

        {

            string ConString = "data source=DESKTOP-726MH0T;initial catalog=gesundheit;trusted_connection=true";

            string CmdString = string.Empty;

            using (SqlConnection con = new SqlConnection(ConString))
            {

                CmdString = "select * from Blutdruck ORDER BY Datum DESC ";

                SqlCommand cmd = new SqlCommand(CmdString, con);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("maximal");

                sda.Fill(dt);
                //dtgGewicht.AutoGeneratingColumn += Dtg_Gewicht_AutoGeneratedColumns;
                dtgDaten.ItemsSource = dt.DefaultView;

            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }

        private void dtgDaten_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}


