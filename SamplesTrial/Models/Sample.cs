using System;
using System.ComponentModel.DataAnnotations;

namespace SamplesTrial.Models
{
    public class Sample
    {
        /// <summary>
        /// Product ID
        /// </summary>
        [Key]
        public string SampleID { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Price of the product
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// Category of the product
        /// </summary>
        public string SampleMp3URL { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Mp3Blob { get; set; }

        public string SampleMp3Blob { get; set; }

        public DateTime SampleDate { get; set; }
    }
}