using Municipality_Services_App.Data_Structures;
using Municipality_Services_App.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Municipality_Services_App.Workers
{
    public class FilterSortLogic
    {
        //--------------------------------------------------//
        //                   *PROPERTIES*                   //
        //--------------------------------------------------//

        private User CurrentUser = UserSingleton.Instance.CurrentUser;

        public FilterSortLogic()
        {

        }

        /// <summary>
        /// Applies filtering and sorting logic to the events based on the provided parameters.
        /// </summary>
        /// <param name="sortBy">The sorting method to apply (e.g., "Soonest Date", "Distance").</param>
        /// <param name="categoryFilters">A set of category names to filter by.</param>
        /// <param name="minDistance">The minimum distance value to filter events by (default is 0).</param>
        /// <param name="maxDistance">The maximum distance value to filter events by (default is double.MaxValue).</param>
        /// <param name="minDate">The minimum date value to filter events by (default is DateTime.MinValue).</param>
        /// <param name="maxDate">The maximum date value to filter events by (default is DateTime.MaxValue).</param>
        public void ApplyFilters(string sortBy, HashSet<string> categoryFilters, double minDistance = 0, double maxDistance = double.MaxValue, DateTime? minDate = null, DateTime? maxDate = null)
        {
            // set default dates if not provided
            minDate = minDate ?? DateTime.MinValue;
            maxDate = maxDate ?? DateTime.MaxValue;

            Debug.WriteLine(CurrentUser.GlobalEvents.Count);
            ProcessDistanceValues(CurrentUser.GlobalEvents);

            var filteredEvents = CurrentUser.GlobalEvents.Where(e =>
                 (categoryFilters.Count == 0 || categoryFilters.Contains(e.Category)) && // filter by category (if no categories are selected show all categories)
                e.DistanceValue >= minDistance &&                                     // filter by distance range
                e.DistanceValue <= maxDistance &&
                e.ParsedDate >= minDate &&                                   // filter by date range
                e.ParsedDate <= maxDate
            ).ToList();
            Debug.WriteLine(filteredEvents.Count);

            var sortedAndFilteredEventData = SortEventData(sortBy, filteredEvents);

            CurrentUser.DisplayedEvents.Clear();
            foreach (var ev in sortedAndFilteredEventData)
            {
                CurrentUser.DisplayedEvents.Add(ev);
            }
        }

        /// <summary>
        /// Resets any filters applied and shows all events again.
        /// </summary>
        public void ClearFilters()
        {
            CurrentUser.ResetDisplayedEvents();
        }


        private static void ProcessDistanceValues(Queue<EventDataModel> eventList)
        {
            foreach (var eventModel in eventList)
            {
                // Check if DistanceFromUserKm contains digits or a '.'
                string sanitizedDistance = Regex.Replace(eventModel.DistanceFromUserKm, @"[^0-9.]", "");

                // If the sanitizedDistance is empty, assign a large value to DistanceValue
                if (string.IsNullOrEmpty(sanitizedDistance) || !sanitizedDistance.Any(char.IsDigit))
                {
                    eventModel.DistanceValue = 100; // still show the events with unkown distances but at the bottom
                }
                else
                {
                    // Try to parse the sanitized string into a double
                    if (double.TryParse(sanitizedDistance, out double distance))
                    {
                        eventModel.DistanceValue = distance;
                    }
                    else
                    {
                        eventModel.DistanceValue = double.MaxValue; // Handle parsing failure with a large number
                    }
                }
            }
        }

        private List<EventDataModel> SortEventData(string sortBy, List<EventDataModel> filteredEvents)
        {
            IOrderedEnumerable<EventDataModel> sortedEvents;

            switch (sortBy)
            {
                case "Soonest Date":
                    // Sort by ascending date
                    sortedEvents = filteredEvents.OrderBy(item => item.ParsedDate);
                    break;

                case "Latest Date":
                    // Sort by descending date
                    sortedEvents = filteredEvents.OrderByDescending(item => item.ParsedDate);
                    break;

                case "Distance (acending)":
                    // Sort by ascending distance
                    sortedEvents = filteredEvents.OrderBy(item => item.DistanceValue);
                    break;

                case "Distance (decending)":
                    // Sort by descending distance
                    sortedEvents = filteredEvents.OrderByDescending(item => item.DistanceValue);
                    break;

                case "Recommended":
                    // Sort based on previous CurrentUser activity
                    return CalculatePersonalisedEventPriorities(filteredEvents);

                default:
                    // Default to sorting by soonest date
                    sortedEvents = filteredEvents.OrderBy(item => item.ParsedDate);
                    break;
            }

            return sortedEvents.ToList();
        }

        public List<EventDataModel> CalculatePersonalisedEventPriorities(List<EventDataModel> unsortedEvents)
        {
            var priorityQueue = new PriorityQueue<EventDataModel>();
            foreach (var eventModel in unsortedEvents)
            {
                // each event gets a low priority initially
                int priority = 1;

                // increase the priority of the event based on how much it matches the users prefernces
                if (CurrentUser.CategoriesInterestedIn.Contains(eventModel.Category))
                {
                    priority += 5;
                }


                // if the CurrentUser has directly expressed interest in a specific event title then increase the priority by alot!
                if (CurrentUser.EventsInterestedIn.Any(ev => ev.Title == eventModel.Title))
                {
                    priority += 50;
                }


                // count how many previous search terms match
                int SearchHistoryRelevance = CurrentUser.SearchHistory.Count(term =>
                    eventModel.Venue.ToLower().Contains(term) ||
                    eventModel.Title.ToLower().Contains(term) ||
                    eventModel.DateInfo.ToLower().Contains(term));

                int SearchPriorityScore = SearchHistoryRelevance * 10;

                priority += SearchPriorityScore;


                // Add the event to the priority queue
                priorityQueue.Enqueue(eventModel, priority);
            }
            var toReturn = new List<EventDataModel>();
            while (!priorityQueue.IsEmpty())
            {
                // add the events in order of their lowest priority value
                toReturn.Add(priorityQueue.Dequeue());
            }
            // fix the order
            toReturn.Reverse();
            return toReturn;
        }

    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//