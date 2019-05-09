using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaymentsAPIClient.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        public string Password { get; set; }
    }
}