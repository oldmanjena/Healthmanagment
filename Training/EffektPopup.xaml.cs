using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Healthmanagment.Training
{
    /// <summary>
    /// Interaktionslogik für EffektPopup.xaml
    /// </summary>
    public partial class EffektPopup : Window
    {
        public string ErgebnisText { get; private set; }
        public EffektPopup()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner; // Sicherstellen, dass es zentriert relativ zum Besitzer angezeigt wird
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Beispielhafte Berechnung
            if (double.TryParse(txtWert1.Text, out double w1) &&
                double.TryParse(txtWert2.Text, out double w2) &&
                double.TryParse(txtWert3.Text, out double w3) &&
                double.TryParse(txtWert4.Text, out double w4))
            {
                double result = w1 * 1 + w2 * 2.0 + w3 * 3.0 + w4 * 4.0;
                ErgebnisText = result.ToString("F1"); // ein Nachkommastelle
                DialogResult = true; // Wichtig, um anzuzeigen, dass das Pop-up erfolgreich geschlossen wurde
                Close(); // Schließe das Pop-up nach erfolgreicher Berechnung
            }
            else
            {
                MessageBox.Show("Bitte gültige Zahlen eingeben.");
            }
        }
    }
}
