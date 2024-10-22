using Municipality_Services_App.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Municipality_Services_App
{
    /// <summary>
    /// this class was made with lots of help from chatGPT because im not as comfortable with working with files as i probably should be.
    /// it simply handles loading in the address data so that the data can be loaded into the Trie structure
    /// </summary>
    public class AddressAutocomplete
    {
        public IEnumerable<Address> LoadAddressesFromFile(string filePath)
        {
            // Open the file stream for reading the JSON file
            using (var fileStream = File.OpenRead(filePath))
            // Create a StreamReader to read from the file stream
            using (var streamReader = new StreamReader(fileStream))
            // Initialize a JsonTextReader to parse the JSON data
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                // Create a JsonSerializer to deserialize JSON objects into Address objects
                var serializer = new JsonSerializer();

                // Read the first token from the JSON file to check its format
                jsonReader.Read();
                if (jsonReader.TokenType != JsonToken.StartArray)
                {
                    // If the first token is not the start of an array, throw an exception
                    throw new JsonException("JSON file should start with an array.");
                }

                // Continue reading through the JSON file
                while (jsonReader.Read())
                {
                    // If the current token represents the start of an object (address), deserialize it
                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        // Deserialize the JSON object into an Address object and yield it to the caller
                        yield return serializer.Deserialize<Address>(jsonReader);
                    }
                }
            }
        }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//