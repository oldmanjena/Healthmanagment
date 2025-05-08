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

namespace Healthmanagment.Planung
{
    /// <summary>
    /// Interaktionslogik f?r TraiPlanung.xaml
    /// </summary>
    public partial class TraiPlanung : Window
    {
        public TraiPlanung()
        {
            InitializeComponent();
            this.DataContext = new PlanFreiesViewModel();
        }
    }
}

