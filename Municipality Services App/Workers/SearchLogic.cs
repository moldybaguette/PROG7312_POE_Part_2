using Municipality_Services_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Municipality_Services_App.Workers
{
    public static class SearchLogic
    {
        private static User CurrentUser = UserSingleton.Instance.CurrentUser;

        /// <summary>
        /// Allows the user to search events by venue, title, or date using multiple terms.
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static List<EventDataModel> GetSearchResult(string searchText)
        {
            // split up the search to allow the CurrentUser to search multiple properties of an event
            // at once, for instance: "durban october" 
            var searchTerms = searchText.Trim().ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // add the search terms to the users search history to help with their recomendation algorithm
            CurrentUser.SearchHistory.AddRange(searchTerms.ToList());

            // set the result of the search to all the events if
            // any of the search terms are present within the venue, title or date
            var searchResult = CurrentUser.GlobalEvents.Where(ev =>
                searchTerms.Any(term => ev.Venue.ToLower().Contains(term) ||
                                        ev.Title.ToLower().Contains(term) ||
                                        ev.DateInfo.ToLower().Contains(term)))
                .ToList();

            return searchResult;
        }

    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//