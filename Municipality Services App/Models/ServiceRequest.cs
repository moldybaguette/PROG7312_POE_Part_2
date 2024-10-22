using Municipality_Services_App.Workers;
using System;
using System.Collections.Generic;

namespace Municipality_Services_App.Models
{
    /// <summary>
    /// stores a service request object including the location, category description and any attached files 
    /// </summary>
    public class ServiceRequest
    {
        // the location of the service request
        public Address Location { get; set; }

        // the category of the service request
        public string Category { get; set; }

        // a description of the service request
        public string Description { get; set; }

        /// <summary>
        /// List of attached files
        /// </summary>
        public List<UploadedFile> Files { get; set; } = new List<UploadedFile>();

        // default constructor
        public ServiceRequest()
        {
        }

        /// <summary>
        /// Constructor to initialize the service request with specific values
        /// </summary>
        /// <param name="location"></param>
        /// <param name="category"></param>
        /// <param name="filePaths"></param>
        /// <param name="description"></param>
        public ServiceRequest(Address location, string category, List<string> filePaths, string description)
        {
            Location = location;
            Category = category;
            foreach (var file in filePaths)
            {
                if (FileHelper.IsValidFileType(file))
                {
                    AddFile(file);
                }
            }
            Description = description;
        }

        /// <summary>
        /// Method to add a new file to the service request
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddFile(string filePath)
        {
            if (FileHelper.IsValidFileType(filePath))
            {
                var uploadedFile = new UploadedFile
                {
                    FileName = System.IO.Path.GetFileName(filePath),
                    FileType = System.IO.Path.GetExtension(filePath).TrimStart('.').ToLower(),
                    Content = FileHelper.EncodeFileToBase64(filePath)
                };
                Files.Add(uploadedFile);
            }
            else
            {
                throw new InvalidOperationException("Invalid file type.");
            }
        }
    }
}//----------------------------------------------------End_of_File----------------------------------------------------//

