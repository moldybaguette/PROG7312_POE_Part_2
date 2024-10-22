using System;
using System.IO;

namespace Municipality_Services_App.Workers
{
    /// <summary>
    /// methods for working with files uploaded and attached to service requests
    /// </summary>
    public class FileHelper
    {
        // Supported file extensions
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".txt", ".docx", ".pdf" };

        /// <summary>
        /// Validates if the file is an allowed type.
        /// </summary>
        /// <param name="filePath">The full file path.</param>
        /// <returns>True if the file is valid, false otherwise.</returns>
        public static bool IsValidFileType(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            // Get the file extension
            string fileExtension = Path.GetExtension(filePath)?.ToLowerInvariant();

            // Check if the extension is in the allowed list
            return Array.Exists(AllowedExtensions, ext => ext == fileExtension);
        }

        /// <summary>
        /// Encodes a file to a Base64 string.
        /// </summary>
        /// <param name="filePath">The full path of the file to encode.</param>
        /// <returns>A Base64 encoded string.</returns>
        public static string EncodeFileToBase64(string filePath)
        {
            // Read the file into a byte array
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Convert the byte array to a Base64 string
            return Convert.ToBase64String(fileBytes);
        }

        /// <summary>
        /// Decodes a Base64 string back to a file.
        /// </summary>
        /// <param name="base64String">The Base64 encoded string.</param>
        /// <param name="outputFilePath">The path where the decoded file will be saved.</param>
        public static void DecodeBase64ToFile(string base64String, string outputFilePath)
        {
            // Convert the Base64 string to a byte array
            byte[] fileBytes = Convert.FromBase64String(base64String);

            // Write the byte array to a file
            File.WriteAllBytes(outputFilePath, fileBytes);
        }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//