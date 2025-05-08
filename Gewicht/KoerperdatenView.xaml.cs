using Healthmanagment.ViewModel;
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

namespace Healthmanagment.Gewicht
{
    /// <summary>
    /// Interaktionslogik f?r KoerperdatenView.xaml
    /// </summary>
    public partial class KoerperdatenView : Window
    {
        public KoerperdatenView()
        {
            InitializeComponent();
            this.DataContext = new KoerperdatenViewModel(); // oder was du wirklich verwendest
        }

        private void Control_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(request);
                }

                e.Handled = true;
            }
        }

        private void OnFinishButtonClicked(object sender, RoutedEventArgs e)
        {
            // Daten aus dem ViewModel speichern
            if (DataContext is KoerperdatenViewModel vm)
            {
                if (vm.SpeichernCommand.CanExecute(null))
                {
                    vm.SpeichernCommand.Execute(null);
                }
            }

            // Optional: Fenster schlie?en nach Abschluss
            this.Close(); // Dies schlie?t das Fenster, nachdem der Button geklickt wurde
        }
    }
}

