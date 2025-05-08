using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthmanagment.ViewModel
{
    public class TrainartViewModel
    {
        public List<string> Trainingsarten { get; set; }

        public TrainartViewModel()
        {
            Trainingsarten = new List<string> { "Di?t", "Masseaufbau", "Definition" };
        }
    }
}

