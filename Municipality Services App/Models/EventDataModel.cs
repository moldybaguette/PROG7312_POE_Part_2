using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Municipality_Services_App.Models
{
    public class EventDataModel
    {
        public BitmapImage Image { get; set; }
        public string Title { get; set; } // Event title
        public string DateInfo { get; set; } // Event date 
        public DateTime ParsedDate { get; set; }
        public string TimeInfo { get; set; } // time of the event 

        public string Venue { get; set; } // List of venues
        public string Category { get; set; } // Event category
        public string DistanceFromUserKm { get; set; } // Sanitiser from CurrentUser in kilometers
        public double DistanceValue { get; set; }
        public bool InterestedButtonEnabled { get; set; } // Indicates if the CurrentUser is interested in attending

        public EventDataModel()
        {
            InterestedButtonEnabled = true;
        }
    }
}
