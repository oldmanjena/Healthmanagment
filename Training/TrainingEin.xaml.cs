using Healthmanagment.Converter;
using Healthmanagment.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
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
using WPFMaskedTextBox;
using Calendar = System.Globalization.Calendar;

namespace Healthmanagment.Training
{
    /// <summary>
    /// Interaktionslogik f?r TrainingEin.xaml
    /// </summary>
    public partial class TrainingEin : Window
    {
        public TrainingEin()
        {
            var combinedVM = new CombinedViewModel();

            // ?? Verbindung zwischen TimePickerVM und TrainingsVM herstellen
            combinedVM.TimePickerVM.TrainingsVM = combinedVM.TrainingsVM;

            // ?? ViewModel an View binden
            DataContext = combinedVM;

            InitializeComponent();
           // DataContext = new CombinedViewModel();
           
            //UpdateKalenderwoche(DateTime.Today);

            var timeSpanConverter = new TimeSpanToStringConverter();
            Resources.Add("TimeSpanToStringConverter", timeSpanConverter);        



        }

        public class CombinedViewModel
        {
            public TrainingsViewModel TrainingsVM { get; set; }
            public TimePickerViewModel TimePickerVM { get; set; }

            public CombinedViewModel()
            {
                TrainingsVM = new TrainingsViewModel();
                TimePickerVM = new TimePickerViewModel();

                TimePickerVM.TrainingsVM = TrainingsVM; // Verbindung setzen

                TrainingsVM.LadePlaene(); // <--- WICHTIG: Jetzt werden die Pl?ne geladen
            }
        }

        private void txtEffekt_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var popup = new EffektPopup();
                popup.Owner = this; // setzt den Owner auf das Hauptfenster

                if (popup.ShowDialog() == true)
                {
                    txtEffekt.Text = popup.ErgebnisText;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Öffnen des Popups: {ex.Message}");
            }
        }





        private void UpdateKalenderwoche(DateTime datum)
        {
            // Kultur verwenden, z.?B. Deutsch (ISO 8601)
            CultureInfo ci = CultureInfo.CurrentCulture;
            Calendar cal = ci.Calendar;

            // Kalenderwoche nach ISO 8601 (erste Woche mit mindestens 4 Tagen)
            CalendarWeekRule rule = CalendarWeekRule.FirstFourDayWeek;
            DayOfWeek firstDay = DayOfWeek.Monday;

            int kw = cal.GetWeekOfYear(datum, rule, firstDay);
            txtKw.Text = $"KW {kw}";
        }
       

        private void txtEntfernung_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as MaskedTextBox;
            if (textBox != null)
            {
                // Pr?fen, ob der Text ein Komma enth?lt und durch einen Punkt ersetzen
                string inputText = textBox.Text;

                // Ersetze Komma durch Punkt
                if (inputText.Contains(","))
                {
                    inputText = inputText.Replace(",", ".");
                }

                // Wenn der Text ge?ndert wurde, die Bindung mit dem neuen Text aktualisieren
                if (inputText != textBox.Text)
                {
                    textBox.Text = inputText;
                    // Setze den Cursor ans Ende des Textes, damit der Benutzer weiter eingeben kann
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }

     

      

        private void txtEffekt_LostFocus(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show("Lost Focus Event wurde ausgelöst!");
        }

        private void TestPopupButton_Click(object sender, RoutedEventArgs e)
        {
            EffektPopup effektPopUp = new EffektPopup();
            effektPopUp.Owner = this;
            if (effektPopUp.ShowDialog() == true)
            {
                txtEffekt.Text = effektPopUp.ErgebnisText;
            }
        }

        private void txtEffekt_GotFocus(object sender, RoutedEventArgs e)
        {
            EffektPopup effektPopUp = new EffektPopup();
            effektPopUp.Owner = this;
            if (effektPopUp.ShowDialog() == true)
            {
                txtEffekt.Text = effektPopUp.ErgebnisText;
            }
        }
    }
}

