using ClientCustom.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace ClientCustom.UI.Models
{
    public class JWTModel : IAuthModel
    {
        public string SecretKey { get => PaymentSettings.JWTSecretKey; }
        public string SecurityAlgoritm { get => SecurityAlgorithms.HmacSha256Signature; } 
        public int ExpireMinutes { get => 5; } // check this in payment docs or code
        public Claim[] Claims { get; set; }

        public static JWTModel GetJWTModel(string id, string firstname, string lastname, string email)//todo
        {
            JWTModel model = new JWTModel();
            model.Claims = new Claim[]
                {
                    new Claim(PaymentSettings.JWTUniqueIdentifierClaim, id),
                    new Claim(PaymentSettings.JWTFirstNameClaim, firstname),
                    new Claim(PaymentSettings.JWTLastNameClaim, lastname),
                    new Claim(PaymentSettings.JWTEmailAddressClaim, email)
                };
            return model;
        }
    }
}