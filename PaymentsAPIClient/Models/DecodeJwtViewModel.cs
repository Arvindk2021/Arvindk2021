using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaymentsAPIClient.Models
{

    public enum JwtTokenType
    {
        Standard = 1,
        SportsAlliance = 2,
        SportsAllianceV2 = 3
    }

    public class DecodeJwtViewModel
    {
        [Required(ErrorMessage = "Please enter jwt token")]
        [Display(Name = "Secret keys")]
        public string SecretKeys { get; set; }

        public JwtTokenType TokenType { get; set; }

        [Required(ErrorMessage = "Please enter jwt token")]
        [Display(Name = "Jwt Token")]
        [DataType(DataType.MultilineText)]
        public string InputJwtToken { get; set; }

        [Display(Name = "Standard Jwt Token")]
        public string StandardToken { get; set; }
        public string Claims { get; set; }
        public string Message { get; set; }
    }
}