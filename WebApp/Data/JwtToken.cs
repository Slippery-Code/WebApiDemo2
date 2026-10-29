using Newtonsoft.Json;

namespace WebApp.Data
{
    public class JwtToken
    {

        [JsonProperty("access_token")] //auto blepei sto access_token ston authorityController 
        public string? AccessToken { get; set; }

        [JsonProperty("expires_at")] //opws kai to apo panw
        public DateTime ExpiresAt { get; set; }
    }
}
