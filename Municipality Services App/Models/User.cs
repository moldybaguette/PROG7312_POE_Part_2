using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Municipality_Services_App.Models
{
    public class User
    {

        public string Address { get; set; }
        public Stack<EventDataModel> EventsInterestedIn { get; set; }

        public HashSet<string> CategoriesInterestedIn = new HashSet<string>();

        public List<string> SearchHistory { get; set; }


        public Queue<EventDataModel> GlobalEvents { get; set; }
        public ObservableCollection<EventDataModel> DisplayedEvents { get; set; }

        public User()
        {
            EventsInterestedIn = new Stack<EventDataModel>();
            GlobalEvents = new Queue<EventDataModel>();
            DisplayedEvents = new ObservableCollection<EventDataModel>();
            SearchHistory = new List<string>();
        }

        public void UserIsInterestedIn(EventDataModel eventData)
        {
            EventsInterestedIn.Push(eventData);
            CategoriesInterestedIn.Add(eventData.Category);
        }
        public void ResetDisplayedEvents()
        {
            DisplayedEvents.Clear();
            foreach (var ev in GlobalEvents)
            {
                DisplayedEvents.Add(ev);
            }
        }
    }
}
