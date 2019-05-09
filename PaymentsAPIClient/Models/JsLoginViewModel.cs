using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaymentsAPIClient.Models
{
    public class JsLoginViewModel
    {

        [Required(ErrorMessage = "You must enter your Email address")]
        [StringLength(150, ErrorMessage = "The username cannot be longer than 150 characters")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "You must enter your password")]
        [StringLength(20, ErrorMessage = "The password cannot be longer than 20 characters")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        public string Status { get; set; }
    }
}