using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaymentsAPIClient.Models
{
    public class JsonLoginViewModel
    {
        [Required(ErrorMessage = "You must enter your username")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "You must enter your password")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}