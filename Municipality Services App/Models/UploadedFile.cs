﻿namespace Municipality_Services_App.Models
{
    /// <summary>
    /// Class to represent an uploaded file (this was generated by chat)
    /// </summary>
    public class UploadedFile
    {
        /// <summary>
        /// The name of the file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The type/extension of the file (e.g., txt, pdf, jpg)
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// The base64 encoded content of the file
        /// </summary>
        public string Content { get; set; }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//