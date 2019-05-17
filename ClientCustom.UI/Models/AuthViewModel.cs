using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientCustom.UI.Models
{
    public class LoginViewModel
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }

        public string Token { get; set; }
    }
}