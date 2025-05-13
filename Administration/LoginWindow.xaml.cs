using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient; // oder System.Data.SqlClient, je nach deiner Konfiguration
using System.Configuration;

namespace Healthmanagment.Administration
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string Benutzername => txtBenutzername.Text;
        public string Passwort => txtPasswort.Password;

        public bool Angemeldet { get; private set; } = false; // <- Diese Zeile ist entscheidend

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (PrüfeLogin(Benutzername, Passwort))
            {
                Angemeldet = true;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Login fehlgeschlagen", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Abbrechen_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private bool PrüfeLogin(string benutzername, string passwort)
        {
            // Deine SQL-Login-Prüfung hier
            return true; // Platzhalter
        }
    }
}


