using System;
using System.ComponentModel.DataAnnotations;

namespace SamplesTrial.Models
{
    public class Sample
    {
        /// <summary>
        /// Sample ID
        /// </summary>
        [Key]
        public string SampleID { get; set; }

        /// <summary>
        /// Title of track
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Artist of track
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// URL to the sample of the track
        /// </summary>
        public string SampleMp3URL { get; set; }

        /// <summary>
        /// Date the sample object was created
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Name of full song blob in table
        /// </summary>
        public string Mp3Blob { get; set; }
        /// <summary>
        /// name of sample of the track in table
        /// </summary>
        public string SampleMp3Blob { get; set; }
        /// <summary>
        /// creation of the sample date
        /// </summary>
        public DateTime SampleDate { get; set; }
    }
}