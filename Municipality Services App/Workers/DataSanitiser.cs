using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace Municipality_Services_App.Workers
{
    public class DataSanitiser
    {

        private static readonly List<string> ValidMonths = new List<string>
        {
            "january", "february", "march", "april", "may", "june",
            "july", "august", "september", "october", "november", "december"
        };


        /// <summary>
        /// used to parse incomplete or malformed dates that might be retuned by the Scraper.
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        public static DateTime ParseDate(string dateString)
        {
            // assume the event happens on the current year if no year is given
            int currentYear = DateTime.Now.Year;

            // remove spaces 
            dateString = dateString.Trim().ToLower();

            // check for a date range
            if (dateString.Contains("-"))
            {
                // if there is a range take the first date
                string[] dates = dateString.Split('-');
                dateString = dates[0].Trim();
            }


            string[] dateParts = dateString.Split(' ');

            // extract day and month
            int day;
            string monthString;

            try
            {
                if (dateParts.Length == 2)
                {
                    // Format: "7 September" or "18 October"

                    day = int.Parse(dateParts[0]);
                    monthString = dateParts[1];
                }
                else
                {
                    // Format: "11" or "26"
                    day = int.Parse(dateParts[0]);
                    monthString = dateParts[1]; // Assume the month is in the second part if present
                }

                // Verify if the month is valid
                if (!ValidMonths.Contains(monthString))
                {
                    throw new ArgumentException($"Invalid month: {monthString}");
                }

                // Parse the month name to an integer (1-12)
                int month = DateTime.ParseExact(monthString, "MMMM", CultureInfo.InvariantCulture).Month;

                var temp = new DateTime(currentYear, month, day);

                if (temp < DateTime.Now)
                {
                    //since the site only has upcoming events if an event is in a previous month then it must be an event for next year
                    currentYear++;
                }

                // Return the DateTime for the given day, month, and year
                return new DateTime(currentYear, month, day);
            }
            catch (FormatException ex)
            {
                Debug.WriteLine($"Error parsing date '{dateString}': {ex.Message}");
                return DateTime.MinValue; // Return a default value or handle as necessary
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return DateTime.MinValue; // Return a default value or handle as necessary
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error while parsing date '{dateString}': {ex.Message}");
                return DateTime.MinValue; // Return a default value or handle as necessary
            }

        }

        public static BitmapImage SetUpEventImage(string src)
        {
            try
            {
                if (string.IsNullOrEmpty(src))
                {
                    Debug.WriteLine("Image source is empty.");
                    return null;
                }

                // Ensure the URL contains a valid protocol
                if (!src.StartsWith("http://") && !src.StartsWith("https://"))
                {
                    src = "https://" + src.TrimStart('/');
                }

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(src, UriKind.Absolute);
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load image: {ex.Message}");
                return null;  // Return null if the image failed to load
            }
        }



    }
}
