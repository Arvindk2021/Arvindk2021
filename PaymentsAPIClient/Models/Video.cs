using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaymentsAPIClient.Models
{
    public class Video
    {
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EntriId { get; set; }
        public string PlayerId { get; set; }
        public string ImageUrl { get; set; }
    }
}