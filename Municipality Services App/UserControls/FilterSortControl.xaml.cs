using Municipality_Services_App.Models;
using Municipality_Services_App.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Municipality_Services_App.UserControls
{
    /// <summary>
    /// Interaction logic for FilterSortControl.xaml
    /// </summary>
    public partial class FilterSortControl: System.Windows.Controls.UserControl
    {
        //--------------------------------------------------//
        //                   *Properties*                   //
        //--------------------------------------------------//

        /// <summary>
        /// boolean value exposed to be used for UI binding
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(FilterSortControl),
                new PropertyMetadata(false, OnIsExpandedChanged));

        /// <summary>
        /// exposed version of the DependencyProperty
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// CurrentUser singleton instance
        /// </summary>
        private User CurrentUser = UserSingleton.Instance.CurrentUser;

        /// <summary>
        /// worker used to compute the filters and sort events as well as implement the recomendation algorithm
        /// </summary>
        private FilterSortLogic FilterSortLogic { get; set; }

        /// <summary>
        /// default sorting options, CurrentUser can sort by distance once they add their address
        /// </summary>
        private ObservableCollection<string> SortByOptions = new ObservableCollection<string>
        {
            "Soonest Date", "Latest Date", "Recommended"
        };

        //--------------------------------------------------//

        /// <summary>
        /// constructor
        /// </summary>
        public FilterSortControl()
        {
            InitializeComponent();
            FilterSortLogic = new FilterSortLogic();
            ComboBoxSortBy.ItemsSource = SortByOptions;
        }

        //--------------------------------------------------//
        //                     *METHODS*                    //
        //--------------------------------------------------//

        /// <summary>
        /// Applies the selected filters to the events based on the CurrentUser's choices.
        /// Creates a hashset of all the categories with selected checkboxes and applies the filtering.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ButtonApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            //create a hashset of all the categories with selected checkboxes
            var CategoryFilter = CreateCategoryFilter();

            // if no dates have been selected set filters to min and max to not effect results
            var selectedMinDate = DatePickerMinDate.SelectedDate ?? DateTime.MinValue;
            var selectedMaxDate = DatePickerMaxDate.SelectedDate ?? DateTime.MaxValue;

            //if the users hasnt set their address set to 0 and maxValue respectivly to not effect the filtering at all
            var MinDistance = string.IsNullOrEmpty(CurrentUser.Address) ? 0 : MinDistanceSlider.Value;
            var MaxDistance = string.IsNullOrEmpty(CurrentUser.Address) ? double.MaxValue : MaxDistanceSlider.Value;
            var sortBy = ComboBoxSortBy.SelectedItem.ToString() ?? "Soonest Date";

            //apply the filtering
            FilterSortLogic.ApplyFilters(sortBy, CategoryFilter, MinDistance, MaxDistance, selectedMinDate, selectedMaxDate);
        }

        /// <summary>
        /// Clears all the selected filters and resets the filter controls to their default values.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ButtonClearFilters_Click(object sender, RoutedEventArgs e)
        {
            FilterSortLogic.ClearFilters();
            // reset everything to default values
            DatePickerMinDate.SelectedDate = null;
            DatePickerMaxDate.SelectedDate = null;

            MinDistanceSlider.Value = MinDistanceSlider.Minimum;
            MaxDistanceSlider.Value = MaxDistanceSlider.Maximum;

            CheckBoxConferenceExpoCategory.IsChecked = false;
            CheckBoxFestivalCategory.IsChecked = false;
            CheckBoxFilmCategory.IsChecked = false;
            CheckBoxLifestyleCategory.IsChecked = false;
            CheckBoxArtsCategory.IsChecked = false;
            CheckBoxSportCategory.IsChecked = false;
            CheckBoxOtherCategory.IsChecked = false;
        }

        // <summary>
        /// Callback invoked when the IsExpanded property changes.
        /// This method may be used to change the icon of the dropdown button.
        /// </summary>
        /// <param name="d">The dependency object that changed.</param>
        /// <param name="e">The event data containing the old and new values.</param>
        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //TODO: change the icon of the drop down button
        }

        /// <summary>
        /// Expands or contracts the filter control when the button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ExpandContractButton_Click(object sender, RoutedEventArgs e)
        {

            var expandStoryboard = (Storyboard)FindResource("ExpandStoryboard");
            var collapseStoryboard = (Storyboard)FindResource("CollapseStoryboard");

            if (!IsExpanded)
            {
                IsExpanded = !IsExpanded;
                expandStoryboard.Begin();
            }
            else
            {
                // the IsExpanded is only changed after the collapse storyboard is completed so that the filter componenet is out of the way when the itemview expands 
                collapseStoryboard.Begin();
            }
        }

        /// <summary>
        /// Creates a hashset of selected categories based on checked checkboxes.
        /// </summary>
        /// <returns>A hashset containing the selected category strings.</returns>
        private HashSet<string> CreateCategoryFilter()
        {
            HashSet<string> selectedCategories = new HashSet<string>();

            // im so sorry. please just pretend this doesnt exist...
            AddCheckBoxContentIfChecked(CheckBoxConferenceExpoCategory, selectedCategories);

            AddCheckBoxContentIfChecked(CheckBoxFestivalCategory, selectedCategories);
            AddCheckBoxContentIfChecked(CheckBoxFilmCategory, selectedCategories);
            AddCheckBoxContentIfChecked(CheckBoxLifestyleCategory, selectedCategories);
            AddCheckBoxContentIfChecked(CheckBoxArtsCategory, selectedCategories);
            AddCheckBoxContentIfChecked(CheckBoxSportCategory, selectedCategories);
            AddCheckBoxContentIfChecked(CheckBoxOtherCategory, selectedCategories);

            return selectedCategories;
        }

        /// <summary>
        /// adds the comtemt of the checkbox to a hashset. (literally the only reason to use a hashset her is the rubric.) 
        /// </summary>
        /// <param name="categogoryCheckBox"></param>
        /// <param name="selectedCategories"></param>
        private void AddCheckBoxContentIfChecked(CheckBox categogoryCheckBox, HashSet<string> selectedCategories)
        {
            if (categogoryCheckBox.IsChecked == true)
            {
                var categoryString = categogoryCheckBox.Content.ToString();
                selectedCategories.Add(categoryString);
                CurrentUser.CategoriesInterestedIn.Add(categoryString);
            }
        }


        /// <summary>
        /// Updates the Max Distance label and ensures the Min Distance slider does not exceed the Max Distance value.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the new value.</param>
        private void MaxDistanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MaxDistanceValue != null)
            {
                MaxDistanceValue.Text = $"{(int)e.NewValue} km";
            }

            // Ensure Min slider doesn't exceed Max slider
            if (MinDistanceSlider != null && MinDistanceSlider.Value > e.NewValue)
            {
                MinDistanceSlider.Value = e.NewValue;
            }

        }

        /// <summary>
        /// Updates the Min Distance label and ensures the Max Distance slider does not go below the Min Distance value.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the new value.</param>
        private void MinDistanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MinDistanceValue != null)
            {
                MinDistanceValue.Text = $"{(int)e.NewValue} km";
            }

            // Ensure Max slider doesn't go below Min slider
            if (MaxDistanceSlider != null && MaxDistanceSlider.Value < e.NewValue)
            {
                MaxDistanceSlider.Value = e.NewValue;
            }
        }


        /// <summary>
        /// Completes the storyboard animation and toggles the IsExpanded property.
        /// This ensures the item view does not appear underneath the FilterSortControl.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            // finish the animation before setting IsExpanded so that the item view doesnt apear underneither the filterSortControl
            IsExpanded = !IsExpanded;
        }


        /// <summary>
        /// Updates the SortBy options in the ComboBox based on the current filter selections.
        /// </summary>
        private void ComboBoxSortBy_DropDownOpened(object sender, EventArgs e)
        {
            // if the user has entered their address
            if (!string.IsNullOrEmpty(CurrentUser.Address))
            {
                TryAddSortByDistanceOptions();
            }
        }

        /// <summary>
        /// was having issues where it would keep adding the distance sort options this is the fix.
        /// </summary>
        private void TryAddSortByDistanceOptions()
        {
            var DistanceAcending = "Distance (acending)";
            var DistanceDecending = "Distance (decending)";

            var DistanceAlreadyAdded = SortByOptions.Contains(DistanceAcending) || SortByOptions.Contains(DistanceDecending);
            if (!DistanceAlreadyAdded)
            {
                SortByOptions.Add(DistanceAcending);
                SortByOptions.Add(DistanceDecending);
            }
        }

    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//