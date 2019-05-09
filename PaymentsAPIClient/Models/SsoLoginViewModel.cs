using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaymentsAPIClient.Models
{
    public class SsoLoginViewModel
    {
        [Required(ErrorMessage = "Please enter jwt token")]
        [Display(Name = "Jwt Token if not as sample")]
        [DataType(DataType.MultilineText)]
        public string JwtToken { get; set; }
        public string ReturnUrl { get; set; }
        public string SsoLoginBaseUrl { get; set; }

        public string Status { get; set; }
    }
}