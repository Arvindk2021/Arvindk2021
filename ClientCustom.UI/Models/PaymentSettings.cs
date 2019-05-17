using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ClientCustom.UI.Models
{
    public static class PaymentSettings
    {
        public static string PaymentSSOUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["PaymentSSOURL"];
            }
        }
        public static string JWTSecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings["JWTSecretKey"];
            }
        }
        public static string JWTUniqueIdentifierClaim
        {
            get
            {
                return ConfigurationManager.AppSettings["JWTUniqueIdentifierClaim"];
            }
        }
        public static string JWTFirstNameClaim
        {
            get
            {
                return ConfigurationManager.AppSettings["JWTFirstNameClaim"];
            }
        }
        public static string JWTLastNameClaim
        {
            get
            {
                return ConfigurationManager.AppSettings["JWTLastNameClaim"];
            }
        }
        public static string JWTEmailAddressClaim
        {
            get
            {
                return ConfigurationManager.AppSettings["JWTEmailAddressClaim"];
            }
        }
    }
}