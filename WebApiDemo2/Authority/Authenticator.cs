using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Claims;

namespace WebApiDemo2.Authority
{
    public static class Authenticator
    {
        //help the authority to authenticate the application
        public static bool Authenticate(string clientId, string secret)
        {
            var app = AppRepository.GetApplicationByClientId(clientId);
            if (app == null) return false;

            return (app.ClientId == clientId && app.Secret == secret);

        }
            //briskoume poios exei to idio clientId kai Secret auto shmainei oti to authentication exei ginei epitixhmena


            //createToken method

        public static string CreateToken(string clientId, DateTime expiresAt, string strSecretKey)
        {
            //1.security Algorithm
            //2.Payload contains the claims contains the application information
            //3.generated signature

            //payload goes through the alg applying the signing key 
            //to opoio epistrefei ena signature

            //gia na paraxthei to token prepei na broume ton algorithmo

            //json web token handler

            //to token descriptor basika simperilambanei tis 3 aparetites plirofories pou exw arithmisei panw  kai kapoies epiprosthetes plirofories

            //Algorithm
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(strSecretKey)),
                SecurityAlgorithms.HmacSha256Signature);


            //Payload(claims) ta claims einai ena dictionary
            var app = AppRepository.GetApplicationByClientId(clientId);



            var claimsDictionary = new Dictionary<string, object>
            {
                {"AppName", app?.ApplicationName??string.Empty},
               /* { "Read",(app?.Scopes??string.Empty).Contains("read")? "true":"false"}, //an simperilambanei to read einai true alliws false
                { "Write",(app?.Scopes??string.Empty).Contains("write")? "true":"false"}
               hardcoded the permissions to opoio den bgazei kai poli noima */
            };

            //edw tha orisoume ta permissions apo panw pou ta balame se comment
            var scopes = app?.Scopes?.Split(',') ?? Array.Empty<string>();
            if (scopes.Length>0)
            {
                foreach(var scope in scopes)
                {
                    claimsDictionary.Add(scope.Trim().ToLower(), "true");
                }
            } //adding all of the permissions


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Claims = claimsDictionary,
                Expires = expiresAt,
                NotBefore = DateTime.UtcNow
            };

            var tokenHandler = new JsonWebTokenHandler();
            //gia na dimiourghsw ena token prepei na dimiourghsw ena security token descriptor edw 
            return tokenHandler.CreateToken(tokenDescriptor);


        }

        public static async Task<IEnumerable<Claim>?> VerifyTokenAsync(string tokenString, string securityKey)
        {
            if (string.IsNullOrWhiteSpace(tokenString) || string.IsNullOrWhiteSpace(securityKey))
            {
                return null;
            }

            var keyBytes = System.Text.Encoding.UTF8.GetBytes(securityKey);
            var tokenHandler = new JsonWebTokenHandler();

            //preparing to apo katw wste na to paroume sto tokenHandler katw
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, //most important pragma
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false, //an theloun na to xrisimopoisoun kai alla atoma prepei na einai true opws kai gia to apo panw gia na baleis kati edw prepei na enhmerwseis kai to tokenDescriptor
                ValidateLifetime = true, //check if the signing key is still valid
                ClockSkew = TimeSpan.Zero // No clock skew
            };

            try
            {
                var result = await tokenHandler.ValidateTokenAsync(tokenString, validationParameters);
                if(result.SecurityToken !=null)
                {
                    var tokenObject = tokenHandler.ReadJsonWebToken(tokenString);
                    return tokenObject.Claims ?? Enumerable.Empty<Claim>();
                }
                else
                {
                    //token is not valid
                    return null;
                }
                
            }
            catch(SecurityTokenMalformedException)
            {
                return null;
            }
            catch(SecurityTokenExpiredException)
            {
                return null;
            }
            catch(SecurityTokenInvalidSignatureException)
            {
                return null;
            }
            catch(Exception)
            {
                //other exception
                throw;
            }
            //genika kali praktiki einai otan kapoia methodo petaei exception na kanoume try catch
        }

    }
}
