using Microsoft.Win32;
using Municipality_Services_App.Models;
using Municipality_Services_App.Workers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Municipality_Services_App
{
    /// <summary>
    /// Interaction logic for ReportIssuesWindow.xaml
    /// </summary>
    public partial class ReportIssuesWindow: Window
    {
        /// <summary>
        /// Trie is used to store the address strings to make retreivalm of auto suggestions easy 
        /// </summary>
        private readonly Trie trie = new Trie();

        /// <summary>
        /// the filepath where the address info is stored.
        /// </summary>
        private string filePath = "AddressData\\AddressDataSet.json";

        /// <summary>
        /// the list of file paths where any attached images or documents can be accessed
        /// </summary>
        private List<string> uploadedFilePaths = new List<string>();

        /// <summary>
        /// hardcoded categorys that the CurrentUser can select for their service request.
        /// </summary>
        private string[] CategoryOptions = new string[]
        {
           "Road and Pavement Repairs",
           "Water and Sanitation Services",
           "Electricity and Street Lighting",
           "Waste Management",
           "Public Facility Maintenance",
           "Stormwater and Drainage Issues",
           "Environmental Concerns",
           "Public Safety and Security",
           "Transportation Services"
        };

        /// <summary>
        /// constructor
        /// </summary>
        public ReportIssuesWindow()
        {
            InitializeComponent();
            // set the item source of the combobox to the list of ccategorys
            cmbCategory.ItemsSource = CategoryOptions;
            //Start loading in the addresses
            LoadAddressesAsync();
        }

        /// <summary>
        /// asyncronously load the location addresses
        /// </summary>
        private async void LoadAddressesAsync()
        {
            var autocomplete = new AddressAutocomplete();
            try
            {
                // uses Task.Run to run the file loading on a background thread (in theory this wont block the UI but the window still stutters a bit when loading in)
                var addresses = await Task.Run(() => autocomplete.LoadAddressesFromFile(filePath));

                foreach (var address in addresses)
                {
                    string fullAddress = $"{address.Number} {address.Street} {address.City}";
                    //add each address to the trie
                    trie.Insert(fullAddress);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading addresses: {ex.Message}");
            }
        }

        /// <summary>
        /// called when the back button is clicked and returns the CurrentUser to the main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnToMainMenu(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// called when the submit button is click and verifys that all the manditory fields have been filled and saves a service request object to a JSON file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitServiceRequest(object sender, RoutedEventArgs e)
        {
            try
            {
                // retrieve data from the form components
                Address location = Address.ConvertStringToAddress(txtLocation.Text);
                string category = cmbCategory.SelectedItem?.ToString();
                string description = new TextRange(rtxtDescription.Document.ContentStart, rtxtDescription.Document.ContentEnd).Text;
                List<string> filePaths = uploadedFilePaths;

                // validate CurrentUser inputs
                if (location == null)
                {
                    MessageBox.Show("Please enter a valid location.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(category))
                {
                    MessageBox.Show("Please select a category.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Please enter a description.");
                    return;
                }

                // instantiate a ServiceRequest object with the data
                ServiceRequest serviceRequest = new ServiceRequest(location, category, filePaths, description);

                // temporary path for saving the JSON file for testing purposes
                // (IMPORTANT THIS WILL END UP IN THE BIN/DEBUG FOLDER!!!)
                string jsonFilePath = @"ExampleServiceRequests\ServiceRequest.json";

                // serialize the ServiceRequest object to JSON
                string json = JsonConvert.SerializeObject(serviceRequest, Formatting.Indented);

                // save the JSON to a file for now (will implement a database later 
                System.IO.File.WriteAllText(jsonFilePath, json);


                MessageBox.Show("Service request submitted successfully!");

                // exit the window
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while submitting the service request: {ex.Message}");
            }
        }

        /// <summary>
        /// event called when something is dragged over the FileDropBorder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileDropBorder_DragEnter(object sender, DragEventArgs e)
        {
            // if the data format is a droppable file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // copy the file data
                e.Effects = DragDropEffects.Copy;
                FileDropBorder.Background = Brushes.LightBlue;
                DropInfoTextBlock.Text = "Release to drop files";
            }
        }
        /// <summary>
        /// resets the style of the FileDropBorder when the mouse leaves the border
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileDropBorder_DragLeave(object sender, DragEventArgs e)
        {
            FileDropBorder.Background = Brushes.LightGray;
            DropInfoTextBlock.Text = "Drop files here";
        }

        /// <summary>
        /// deals with files being dropped over the FileDropBorder and passes the file paths into a method to be processed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileDropBorder_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                // add the file paths to the list of uploaded files
                uploadedFilePaths.AddRange(droppedFiles);
            }

            ValidateDroppedFiles();

            FileDropBorder.Background = Brushes.LightGray;
            DropInfoTextBlock.Text = "Drop files here";

            //this triggers the LostFocus event to update the progress bar after file drop
            Component_LostFocus(sender, e);

        }

        /// <summary>
        /// handles the border click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttachFile_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true // Allow multiple file selection
            };

            if (openFileDialog.ShowDialog() == true)
            {
                uploadedFilePaths.AddRange(openFileDialog.FileNames);
                ValidateDroppedFiles();
            }

        }

        /// <summary>
        /// ensures the file types are either text or images (docx and pdf are also accepted)
        /// </summary>
        private void ValidateDroppedFiles()
        {
            // checks to see if any of the uploaded file paths are not of a valid type.
            if (!uploadedFilePaths.Any(x => FileHelper.IsValidFileType(x)))
            {
                MessageBox.Show("invalid file type.");
            }
        }

        /// <summary>
        /// this is where the location autosuggestion feature is updated every time the CurrentUser changes the text in the location textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void LocationInput_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            //the prefix used to search for potential addresses
            string input = txtLocation.Text;
            // if the CurrentUser hasnt types anything dont suggest anything
            if (string.IsNullOrWhiteSpace(input))
            {
                txtLocation.ItemsSource = null;
                return;
            }
            
            // use the trie to get a max of 10 suggestions based on the input so far
            var suggestions = trie.GetSuggestions(input, 10);
            
            txtLocation.ItemsSource = suggestions;
        }

        /// <summary>
        /// when a CurrentUser finishes interacting with a component check that they have actually entered something and update the progressbar to match
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Component_LostFocus(object sender, RoutedEventArgs e)
        {
            int progress = 0;
            int totalComponents = 4; // Total number of components being tracked

            // Check if the CurrentUser has entered data in the txtLocation TextBox
            if (!string.IsNullOrWhiteSpace(txtLocation.Text))
                progress++;

            // Check if the CurrentUser has selected an item in the cmbCategory ComboBox
            if (cmbCategory.SelectedItem != null)
                progress++;

            // Check if files have been added via FileDropBorder and the files are of valid types
            if (uploadedFilePaths != null && uploadedFilePaths.Count > 0 && uploadedFilePaths.Any(x => FileHelper.IsValidFileType(x)))
                progress++;

            // Check if the CurrentUser has entered data in the rtxtDescription RichTextBox
            if (!string.IsNullOrWhiteSpace(new TextRange(rtxtDescription.Document.ContentStart, rtxtDescription.Document.ContentEnd).Text.Trim()))
                progress++;

            // Update the ProgressBar value based on the number of completed components
            pbReportCompletionProgress.Value = (progress / (double)totalComponents) * 100;
        }

        /// <summary>
        /// set the proggressbar to be empty when the window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Component_LostFocus(null, null);
        }

        /// <summary>
        /// update progressbar when the CurrentUser enters text into the description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtxtDescription_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // the only time the description box loses focus is when you click submit so it should add to the progressbar instantly
            Component_LostFocus(sender, e);
        }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//

