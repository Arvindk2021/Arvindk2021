using JWT;
using JWT.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using PaymentsAPIClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace PaymentsAPIClient.Service
{
    public class FanCoreJwtService
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

        static FanCoreJwtService()
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

            if (request.TokenType == JwtTokenType.SportsAllianceV2)
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

                var claims = decoder.Decode(
                    request.Token,
                    urlEncoder.Decode(jwtSecret),
                    !string.IsNullOrWhiteSpace(jwtSecret)
                );

                return new ValidateResponse
                {
                    Claims = claims,
                    JwtToken = jwtToken
                };

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

        public static string GetJwtToken()
        {
            var token = string.Empty;

            return token;
        }


        
        public static string Sign(string payload, string privateKey)
        {
            List<string> segments = new List<string>();
            var header = new { alg = "RS256", typ = "JWT" };

            DateTime issued = DateTime.Now;
            DateTime expire = DateTime.Now.AddHours(10);

            byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None));
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            string stringToSign = string.Join(".", segments.ToArray());

            byte[] bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            //byte[] keyBytes = Convert.FromBase64String(privateKey);
            byte[] keyBytes = FromBase64String(privateKey);
            var privKeyObj = Asn1Object.FromByteArray(keyBytes);
            var privStruct = RsaPrivateKeyStructure.GetInstance((Asn1Sequence)privKeyObj);

            ISigner sig = SignerUtilities.GetSigner("SHA256withRSA");

            sig.Init(true, new RsaKeyParameters(true, privStruct.Modulus, privStruct.PrivateExponent));

            sig.BlockUpdate(bytesToSign, 0, bytesToSign.Length);
            byte[] signature = sig.GenerateSignature();

            segments.Add(Base64UrlEncode(signature));
            return string.Join(".", segments.ToArray());
        }

        private static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                    .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }

        private static byte[] FromBase64String(string inputString)
        {
            string converted = inputString.Replace('-', '+');
            converted = converted.Replace('_', '/');

            return Convert.FromBase64String(converted);
        }


        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }


        // from JWT spec
        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 1: output += "==="; break; // Three pad chars
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }


        public static bool VerifyCognitoJwt(string accessToken)
        {
            string[] parts = accessToken.Split('.');

            string header = parts[0];
            string payload = parts[1];

            string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            JObject headerData = JObject.Parse(headerJson);

            string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            JObject payloadData = JObject.Parse(payloadJson);

            var kid = headerData["kid"];
            var iss = payloadData["iss"];

            var issUrl = iss + "/.well-known/jwks.json";
            var keysJson= string.Empty;

            using (WebClient wc = new WebClient())
            {
                keysJson = wc.DownloadString(issUrl);
            }

            var keyData = GetKeyData(keysJson,kid.ToString());

            if (keyData==null)
                throw new ApplicationException(string.Format("Invalid signature"));

            var modulus = Base64UrlDecode(keyData.Modulus);
            var exponent = Base64UrlDecode(keyData.Exponent);

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();

            var rsaParameters= new RSAParameters();
            rsaParameters.Modulus = new BigInteger(modulus).ToByteArrayUnsigned();
            rsaParameters.Exponent = new BigInteger(exponent).ToByteArrayUnsigned();

            provider.ImportParameters(rsaParameters);

            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts[0] + "." + parts[1]));

            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(provider);
            rsaDeformatter.SetHashAlgorithm(sha256.GetType().FullName);

            if (!rsaDeformatter.VerifySignature(hash, Base64UrlDecode(parts[2])))
                throw new ApplicationException(string.Format("Invalid signature"));

            return true;
        }

        public class KeyData
        {
            public string Modulus { get; set; }
            public string Exponent { get; set; }
        }

        private static KeyData GetKeyData(string keys,string kid)
        {
            var keyData = new KeyData();

            dynamic obj = JObject.Parse(keys);
            var results = obj.keys;
            bool found = false;

            foreach (var key in results)
            {
                if (found)
                    break;

                if (key.kid == kid)
                {
                    keyData.Modulus = key.n;
                    keyData.Exponent = key.e;
                    found = true;
                }
            }

            return keyData;
        }
    }
}