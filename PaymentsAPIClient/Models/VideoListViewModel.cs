using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaymentsAPIClient.Models
{
    public class VideoListViewModel
    {
        public List<Video> Videos { get; set; }
        public string ReturnUrl { get; set; }

    }
}