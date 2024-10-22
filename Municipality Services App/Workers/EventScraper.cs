using HtmlAgilityPack;
using Municipality_Services_App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Municipality_Services_App.Workers
{
    /// <summary>
    /// Represents a class responsible for scraping event data from a website
    /// and adding the events to the user's collection.
    /// (all the method summaries have been done by chat but implementation was done by hand with some help from chat when learing how to use Xpaths)
    /// </summary>
    public class EventScraper
    {
        /// <summary>
        /// Queue that stores event data models.
        /// </summary>
        public Queue<EventDataModel> Events { get; set; }

        /// <summary>
        /// Instance of the current user from the singleton.
        /// </summary>
        private User CurrentUser = UserSingleton.Instance.CurrentUser;

        /// <summary>
        /// Flag indicating whether the user has set a location.
        /// </summary>
        private bool UserHasSetLocationFlag = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventScraper"/> class
        /// and sets the event queue to the current user's global events.
        /// </summary>
        public EventScraper()
        {
            Events = CurrentUser.GlobalEvents;
        }

        /// <summary>
        /// Asynchronously scrapes event data from a predefined website and
        /// processes the scraped event details.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ScrapeEventsAsync()
        {
            if (!string.IsNullOrEmpty(CurrentUser.Address))
            {
                UserHasSetLocationFlag = true;
            }

            var url = "https://eventsincapetown.com/all-events/";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);


            var eventNodes = doc.DocumentNode.SelectNodes("//article[contains(@class, 'mec-event-article') and contains(@class, 'mec-clear')]");

            // fully clear all events stored
            Events.Clear();
            CurrentUser.DisplayedEvents.Clear();

            if (eventNodes != null)
            {
                foreach (var eventNode in eventNodes)
                {
                    // extract event details URL
                    var eventDetailsURL = eventNode.SelectSingleNode(".//h4[@class='mec-event-title']/a")?.GetAttributeValue("href", string.Empty);
                    if (string.IsNullOrEmpty(eventDetailsURL))
                        // skip if URL is missing
                        continue;
                    // load the event details page and get the root node
                    var eventDetailsDoc = await web.LoadFromWebAsync(eventDetailsURL);
                    var eventDetailsNode = eventDetailsDoc.DocumentNode;

                    await FetchEventDetailsAsync(eventNode, eventDetailsNode);
                }
            }
        }

        /// <summary>
        /// Asynchronously fetches and processes detailed information for an event
        /// from its main and detail page nodes.
        /// </summary>
        /// <param name="mainPageNode">The HTML node containing the main event details.</param>
        /// <param name="detailsPageNode">The HTML node containing the detailed event information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task FetchEventDetailsAsync(HtmlNode mainPageNode, HtmlNode detailsPageNode)
        {
            var newEvent = new EventDataModel();
            var distanceAPI = new EventDistanceAPI();

            // Fetch venue information from the details page
            var venueTask = Task.Run(() =>
            {
                var venueNode = detailsPageNode.SelectNodes(".//li")
                    ?.FirstOrDefault(li => li.InnerText.Contains("Venue:"));
                return HttpUtility.HtmlDecode(venueNode?.InnerText.Replace("Venue:", "").Trim() ?? string.Empty);
            });

            var timesTask = Task.Run(() =>
            {
                var venueNode = detailsPageNode.SelectNodes(".//li")
                   ?.FirstOrDefault(li => li.InnerText.Contains("Time:") | li.InnerText.Contains("Event Times:"));
                return HttpUtility.HtmlDecode(venueNode?.InnerText.Replace("Time:", "").Trim() ?? string.Empty);
            });

            // Fetch title, image, category, and date from the main page
            var titleTask = Task.Run(() =>
                HttpUtility.HtmlDecode(mainPageNode.SelectSingleNode(".//h4[@class='mec-event-title']/a")?.InnerText.Trim() ?? string.Empty)
            );

            var imageTask = Task.Run(() =>
                mainPageNode.SelectSingleNode(".//img[@src]")?.GetAttributeValue("data-lazy-src", string.Empty) ??
                               mainPageNode.SelectSingleNode(".//img[@src]")?.GetAttributeValue("src", string.Empty)
            );

            var categoryTask = Task.Run(() =>
                HttpUtility.HtmlDecode(mainPageNode.SelectSingleNode(".//span[@class='mec-category']")?.InnerText.Trim() ?? string.Empty)
            );

            var datesTask = Task.Run(() =>
                HttpUtility.HtmlDecode(mainPageNode.SelectSingleNode(".//div[contains(@class, 'mec-event-date')]")?.InnerText.Trim() ?? string.Empty)
            );


            // Wait for all tasks to complete
            await Task.WhenAll(titleTask, imageTask, categoryTask, datesTask, venueTask, timesTask);

            // Populate the event object
            newEvent.Image = DataSanitiser.SetUpEventImage(await imageTask);
            newEvent.Title = await titleTask;
            newEvent.DateInfo = await datesTask;
            // this is kinda cheating but the site im scraping had a few events that were incorrectly formatted so this handles those specific edge cases
            newEvent.TimeInfo = (await timesTask).Replace("Event", "").Replace("Times:", "");
            newEvent.Venue = await venueTask;
            newEvent.Category = await categoryTask;

            // add the event to the collection on the UI thread
            if (newEvent.Venue.Length <= 100)
            {
                // if the CurrentUser has given their address then check the distance between the users address and the event
                if (!string.IsNullOrEmpty(newEvent.Venue) && UserHasSetLocationFlag)
                {
                    newEvent.DistanceFromUserKm = await distanceAPI.GetDistanceAsync(CurrentUser.Address, newEvent.Venue);
                }
                else
                {
                    newEvent.DistanceFromUserKm = "N/A";
                }

                newEvent.ParsedDate = DataSanitiser.ParseDate(newEvent.DateInfo);


                Debug.WriteLine(newEvent.ParsedDate.ToShortDateString());
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Events.Enqueue(newEvent);
                    CurrentUser.DisplayedEvents.Add(newEvent);
                });
            }
        }

        
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//


