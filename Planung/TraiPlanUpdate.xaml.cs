using Healthmanagment.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaktionslogik f?r TraiPlanUpdate.xaml
    /// </summary>
    public partial class TraiPlanUpdate : Window
    {
        private TrainPlanUpdateViewModel viewModel;
        public TraiPlanUpdate()
        {            

            InitializeComponent();

            viewModel = new TrainPlanUpdateViewModel();
            this.DataContext = viewModel;

            
        }
    }
}

