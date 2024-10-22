using Municipality_Services_App.Models;
using Municipality_Services_App.UserControls;
using Municipality_Services_App.Workers;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Municipality_Services_App
{
    /// <summary>
    /// Interaction logic for LocalEventsWindow.xaml
    /// </summary>
    public partial class LocalEventsWindow: Window
    {
        //--------------------------------------------------//
        //                   *Properties*                   //
        //--------------------------------------------------//

        /// <summary>
        /// local collection that points to the users DisplayedEvents
        /// </summary>
        public ObservableCollection<EventDataModel> EventData { get; set; }

        /// <summary>
        /// Instance of the EventScraper for fetching events
        /// </summary>
        private readonly EventScraper Scraper;

        /// <summary>
        /// Current user from the singleton
        /// </summary>
        private User CurrentUser = UserSingleton.Instance.CurrentUser;

        //--------------------------------------------------//

        /// <summary>
        /// constructor
        /// </summary>
        public LocalEventsWindow()
        {
            InitializeComponent();

            // instantiate a new event Scraper object
            Scraper = new EventScraper();
            //make event data point to the events collection in the EventScraper
            EventData = CurrentUser.DisplayedEvents;

            DataContext = this;

            // manually set the items control src to the event data collection 
            eventItemsControl.ItemsSource = EventData;

        }

        //--------------------------------------------------//
        //                     *METHODS*                    //
        //--------------------------------------------------//

        /// <summary>
        /// window Loaded event to asynchronously start scraping events
        /// called after the window has loaded for a responsive UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // i do the scraping after the window has loaded because i think it looks
            // cool to see all the event cards load in while the UI remains responsive
            await Scraper.ScrapeEventsAsync();

        }

        /// <summary>
        /// Closes the current window and returns to the main menu.
        /// </summary>
        private void ReturnToMainMenu(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Opens the user personalization dialog to allow the CurrentUser to set their address.
        /// This is awaited to ensure distance calculations are done after the dialog is closed.
        /// </summary>
        private async void btnUserSettings_Click(object sender, RoutedEventArgs e)
        {
            // this dialog allows the CurrentUser to set their address.
            // the distance for each event is calculated when the CurrentUser exists the dialog which is why its awaited
            await OpenUserPersonalisationDialog();
        }

        /// <summary>
        /// Handles the event when the user clicks the interested button for an event.
        /// Disables the button for the current event and updates the user's interests.
        /// </summary>
        private void ButtonInterested_Click(object sender, RoutedEventArgs e)
        {
            // get the clicked button
            Button clickedButton = sender as Button;

            // get the event data passed with the CommandParameter
            var eventData = clickedButton.CommandParameter as EventDataModel;

            if (eventData != null)
            {
                // directly set the button to be disabled
                clickedButton.IsEnabled = false;

                // this is so that when the page reloads the buttons of interested events stays disabled
                eventData.InterestedButtonEnabled = false;
                CurrentUser.UserIsInterestedIn(eventData);
            }
        }

        /// <summary>
        /// Handles the search functionality when the user presses the Enter key.
        /// </summary>
        private void TextBoxSearch_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // CurrentUser clicks enter to search
            {
                // get the result of the search
                var searchResult = SearchLogic.GetSearchResult(TextBoxSearch.Text);

                // clear displayed events before adding the results of the search
                CurrentUser.DisplayedEvents.Clear();

                // re-add the displayed events
                foreach (var item in searchResult)
                {
                    CurrentUser.DisplayedEvents.Add(item);
                }
            }
        }

        /// <summary>
        /// Resets the search results if the search text box is empty.
        /// This will restore the displayed events to the default state.
        /// </summary>
        private void TextBoxSearch_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            // reset search if text is empty
            if (string.IsNullOrEmpty(TextBoxSearch.Text))
            {
                CurrentUser.ResetDisplayedEvents();
            }
        }


        /// <summary>
        /// Opens the user personalization dialog and updates the user's address.
        /// If the address changes, recalculates distances for displayed events.
        /// </summary>
        private async Task OpenUserPersonalisationDialog()
        {
            UserPersonalisationDialog dialog = new UserPersonalisationDialog();
            // record the users address value before the dialog opens
            var currentUserAddress = CurrentUser.Address;

            bool? result = dialog.ShowDialog();
            var addressChanged = currentUserAddress != CurrentUser.Address;
            // if the CurrentUsers address changed recalculate all the distance values for all the events
            if (result == true && addressChanged)
            {
                var distanceAPI = new EventDistanceAPI();
                CurrentUser.DisplayedEvents.Clear();
                foreach (var item in CurrentUser.GlobalEvents.ToList())
                {
                    item.DistanceFromUserKm = await distanceAPI.GetDistanceAsync(CurrentUser.Address, item.Venue);
                    CurrentUser.DisplayedEvents.Add(item);
                }
            }
        }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//