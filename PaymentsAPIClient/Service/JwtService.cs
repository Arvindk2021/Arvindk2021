using System;
using System.Text;
using JWT;
using JWT.Serializers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PaymentsAPIClient.Models;

namespace PaymentsAPIClient.Service
{
    public class JwtService
    {


        public class ValidateRequest
        {

            public string Token { get; set; }
            public bool ValidateAllFields { get; set; }
            public string JwtSecret { get; set; }
            public JwtTokenType TokenType { get; set; }
        }
        public class ValidateResponse
        {
            //public DateTime ExpireDate { get; set; }
            //public string UniqueId { get; set; }
            //public string EmailAddress { get; set; }
            //public string FirstName { get; set; }
            //public string LastName { get; set; }
            public string Claims { get; set; }
            public string JwtToken { get; set; }
        }


        private static Dictionary<char, char> SportsAlianceCharacterMap;

        static JwtService()
        {
            SportsAlianceCharacterMap = new Dictionary<char, char>
            {
                { 'a','p' },
                { 'b','q' },
                { 'c','r' },
                { 'd','s' },
                { 'e','t' },
                { 'f','u' },
                { 'g','v' },
                { 'h','w' },
                { 'i','x' },
                { 'j','y' },
                { 'k','z' },
                { 'l','a' },
                { 'm','b' },
                { 'n','c' },
                { 'o','d' },
                { 'p','e' },
                { 'q','f' },
                { 'r','g' },
                { 's','h' },
                { 't','i' },
                { 'u','j' },
                { 'v','k' },
                { 'w','l' },
                { 'x','m' },
                { 'y','n' },
                { 'z','o' },
                { 'A','P' },
                { 'B','Q' },
                { 'C','R' },
                { 'D','S' },
                { 'E','T' },
                { 'F','U' },
                { 'G','V' },
                { 'H','W' },
                { 'I','X' },
                { 'J','Y' },
                { 'K','Z' },
                { 'L','A' },
                { 'M','B' },
                { 'N','C' },
                { 'O','D' },
                { 'P','E' },
                { 'Q','F' },
                { 'R','G' },
                { 'S','H' },
                { 'T','I' },
                { 'U','J' },
                { 'V','K' },
                { 'W','L' },
                { 'X','M' },
                { 'Y','N' },
                { 'Z','O' }
            };


        }
        private static string Caesar(string value)
        {
            var result = "";
            foreach (var c in value)
            {
                if (SportsAlianceCharacterMap.ContainsKey(c))
                {
                    result += SportsAlianceCharacterMap[c];
                }
                else
                {
                    result += c;
                }
            }
            return result;
        }


        public static ValidateResponse Validate(ValidateRequest request)
        {
            var jwtToken = "";
            var jwtSecret = request.JwtSecret;
            if (string.IsNullOrWhiteSpace(request.TokenType.ToString()))
            {
                throw new Exception("Jwt authentication not enabled");
            }

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                throw new Exception("No authentication token has been provided");
            }

            // Decrypt the mess that is sports alliance            
            if (request.TokenType == JwtTokenType.SportsAlliance)
            {
                request.Token = request.Token.Substring(0, 32) + request.Token.Substring(64);

                // oooh yes we need to pad!
                if (request.Token.Length % 4 > 0)
                    request.Token = request.Token.PadRight(request.Token.Length + 4 - request.Token.Length % 4, '=');
                request.Token = Encoding.UTF8.GetString(Convert.FromBase64String(request.Token));
            }

            if (request.TokenType==JwtTokenType.SportsAllianceV2)
            {
                // if the token is already a jwt 
                if (!Regex.IsMatch(request.Token, "^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.?[A-Za-z0-9-_.+/=]*$"))
                {
                    var lenBefore = request.Token.Length;
                    request.Token = request.Token.Substring(0, 22) + request.Token.Substring(44);
                    var lenAfter = request.Token.Length;

                    request.Token = Caesar(request.Token);

                    // oooh yes we need to pad!
                    if (request.Token.Length % 4 > 0)
                        request.Token = request.Token.PadRight(request.Token.Length + 4 - request.Token.Length % 4, '=');

                    var base64Token = Convert.FromBase64String(request.Token);
                    request.Token = System.Text.Encoding.UTF8.GetString(base64Token);

                    // Now we have some querystring nonsense and need to get the JWT!
                    request.Token = System.Web.HttpUtility.ParseQueryString(request.Token)["access_token"];

                }

                jwtToken = request.Token;

                //if its sports alliance jwt then there can be multiple secrets so we cycle through and try to find a valid one if not then use the first
                if (!string.IsNullOrWhiteSpace(request.JwtSecret))
                {
                    var possibleSecrets = request.JwtSecret.Split(',');
                    jwtSecret = possibleSecrets[0];
                    foreach (var possibleSecret in possibleSecrets)
                    {
                        try
                        {
                            var serializer = new JsonNetSerializer();
                            var provider = new UtcDateTimeProvider();
                            var validator = new JwtValidator(serializer, provider);
                            var urlEncoder = new JwtBase64UrlEncoder();
                            var decoder = new JwtDecoder(serializer, validator, urlEncoder);

                            jwtSecret = possibleSecret;

                            var claims = JObject.Parse(
                                decoder.Decode(
                                    request.Token,
                                    urlEncoder.Decode(possibleSecret),
                                    true
                                )
                            );
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

            try
            {
                var serializer = new JsonNetSerializer();
                var provider = new UtcDateTimeProvider();
                var validator = new JwtValidator(serializer, provider);
                var urlEncoder = new JwtBase64UrlEncoder();
                var decoder = new JwtDecoder(serializer, validator, urlEncoder);

                //var claims = JObject.Parse(
                //    decoder.Decode(
                //        request.Token,
                //        urlEncoder.Decode(jwtSecret),
                //        !string.IsNullOrWhiteSpace(jwtSecret)
                //    )
                //);

                var claims= decoder.Decode(
                    request.Token,
                    urlEncoder.Decode(jwtSecret),
                    !string.IsNullOrWhiteSpace(jwtSecret)
                );

                return new ValidateResponse
                {
                    Claims = claims,
                    JwtToken = jwtToken
                };

                //if (claims[SiteContext.Current.Configuration.JwtAuthenticationIdClaim] == null || string.IsNullOrWhiteSpace(claims[SiteContext.Current.Configuration.JwtAuthenticationIdClaim].Value<string>()))
                //    throw new Exception($"Token does not contain '{SiteContext.Current.Configuration.JwtAuthenticationIdClaim}' claim.");

                //if (claims[SiteContext.Current.Configuration.JwtAuthenticationFirstNameClaim] == null || string.IsNullOrWhiteSpace(claims[SiteContext.Current.Configuration.JwtAuthenticationFirstNameClaim].Value<string>()))
                //    throw new Exception($"Token does not contain '{SiteContext.Current.Configuration.JwtAuthenticationFirstNameClaim}' claim.");

                //if (claims[SiteContext.Current.Configuration.JwtAuthenticationLastNameClaim] == null || string.IsNullOrWhiteSpace(claims[SiteContext.Current.Configuration.JwtAuthenticationLastNameClaim].Value<string>()))
                //    throw new Exception($"Token does not contain '{SiteContext.Current.Configuration.JwtAuthenticationLastNameClaim}' claim.");

                //if (claims[SiteContext.Current.Configuration.JwtAuthenticationEmailAddressClaim] == null || string.IsNullOrWhiteSpace(claims[SiteContext.Current.Configuration.JwtAuthenticationEmailAddressClaim].Value<string>()))
                //    throw new Exception($"Token does not contain '{SiteContext.Current.Configuration.JwtAuthenticationEmailAddressClaim}' claim.");

                //if (request.ValidateAllFields)
                //{
                //    if (claims[SiteContext.Current.Configuration.JwtAuthenticationEmailAddressClaim] == null || string.IsNullOrWhiteSpace(claims[SiteContext.Current.Configuration.JwtAuthenticationEmailAddressClaim].Value<string>()))
                //    {
                //        throw new Exception($"Token does not contain '{SiteContext.Current.Configuration.JwtAuthenticationEmailAddressClaim}' claim.");
                //    }

                //    var emailClaim = claims[SiteContext.Current.Configuration.JwtAuthenticationEmailAddressClaim].Value<string>();
                //    if (!emailClaim.Contains("@"))
                //    {
                //        throw new Exception($"Email address '{emailClaim}' is invalid, it does not contain '@'.");
                //    }

                //    if (!emailClaim.Contains("."))
                //    {
                //        throw new Exception($"Email address '{emailClaim}' is invalid, it does not contain '.'.");
                //    }
                //}
                //return new ValidateResponse
                //{
                //    ExpireDate = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(claims["exp"].Value<long>()),
                //    UniqueId = claims[SiteContext.Current.Configuration.JwtAuthenticationIdClaim].Value<string>(),
                //    FirstName =
                //        claims[SiteContext.Current.Configuration.JwtAuthenticationFirstNameClaim].Value<string>(),
                //    LastName = claims[SiteContext.Current.Configuration.JwtAuthenticationLastNameClaim].Value<string>(),
                //    EmailAddress =
                //        claims[SiteContext.Current.Configuration.JwtAuthenticationEmailAddressClaim].Value<string>()
                //};

            }
            catch (TokenExpiredException)
            {
                throw new Exception("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                throw new Exception("Token has invalid signature");
            }

        }

    }
}