using System;
using System.Linq;

namespace Municipality_Services_App.Models
{
    /// <summary>
    /// super simple model to hold addresses when working with JSON
    /// </summary>
    public class Address
    {
        /// <summary>
        /// name of the street
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// house or building number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// name of the city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// parameterised constructor
        /// </summary>
        /// <param name="street"></param>
        /// <param name="number"></param>
        /// <param name="city"></param>
        public Address(string street, string number, string city)
        {
            Street = street;
            Number = number;
            City = city;
        }
        
        /// <summary>
        /// default constructor
        /// </summary>
        public Address() { }

        /// <summary>
        /// Converts a string to an Address object. 
        /// The input should contain a number and a street name separated by a space.
        /// </summary>
        /// <param name="input">The input string to convert.</param>
        /// <returns>An Address object if valid; otherwise, null.</returns>
        public static Address ConvertStringToAddress(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // split the input by spaces
            string[] parts = input.Split(' ');

            // find the first part that is a number
            string number = parts.FirstOrDefault(part => int.TryParse(part, out _));

            if (number == null)
                return null; // No number found

            // join the address chunks after the number as the street name
            int numberIndex = Array.IndexOf(parts, number);
            if (numberIndex == parts.Length - 1)
                return null; // No street name after the number

            string street = string.Join(" ", parts.Skip(numberIndex + 1));

            if (string.IsNullOrWhiteSpace(street))
                return null; // No valid street name found

            // Construct and return the Address object (assuming a fixed city for simplicity)
            return new Address(street.Trim(), number.Trim(), "Cape Town");
        }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//