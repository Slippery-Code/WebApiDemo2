using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApp.Data;
using WebApp.Models;


namespace WebApp.Data
{
    public class WebApiExecuter : IWebApiExecuter
    //theloume na to kanoume dependency injected sto shirts controller sto mvc epishs kaname extract interface
    {
        private const string apiName = "ShirtsApi"; //arxikopoihsei tou string gia na to xrisimopoisoume katw 
        private const string AuthApiName = "AuthorityApi";
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        //xrisimopoioume to HTTP client Find factory gia na dimiourgisoume enan httpClient kai tote xrisimopoioume to HttpClient gia na epikoinwnisoume me to web API
        //tha kanoume dependency inject to HTTP client factory sto web API executer
        //tha prepei na baloume ato sto services collection to opoio einai dependency injection container

        //1on dependency inject sto constructor
        public WebApiExecuter(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) //gia na kanw access kati as poume opws edw to conifiguration prepei na to kanw dependency inject 
            //apo oti exw katalabei oti exeis balei sto program.cs file prepei na to kaneis dependency injection gia na exeis prosbasi se oti allo xreiazesai
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        //meta dimiourgoume diaforetikous methodous to opoio antistoixei se diaforetika http requests

        public async Task<T?> InvokeGet<T>(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);
            // return await httpClient.GetFromJsonAsync<T>(relativeUrl);
            //mporoume na kanoume kai kati diaforetiko
            //dimiourgoume ena request object to opoio tha einai ena httpRequestMessage
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
            var response = await httpClient.SendAsync(request);
            await HandlePotentialError(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T?> InvokePost<T>(string relativeUrl, T obj) //auto pou epistrefei einai type t 
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);
            var response = await httpClient.PostAsJsonAsync(relativeUrl, obj);
            await HandlePotentialError(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }
        public async Task InvokePut<T>(string relativeUrl, T obj) //oti exw edw ston Executer prepei na uparxei sto interface
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);
            var response = await httpClient.PutAsJsonAsync(relativeUrl, obj);
            await HandlePotentialError(response);
        }

        public async Task InvokeDelete(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient); //an sbisoume kapoio tetoia grammh se opoiodipote tha mas pei oti eimaste anauthorized
            var response = await httpClient.DeleteAsync(relativeUrl);
            await HandlePotentialError(response);
        }

        //to ftiaksame san mia kainouria methodo gia na einai pio clean na to baloume se kathe ena apo ta endpoint
        private async Task HandlePotentialError(HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorJson = await httpResponse.Content.ReadAsStringAsync();
                throw new WebApiException(errorJson); //petame to webApiException pou dimiourghsame emeis kai to dinoume san orisma to JsonError to opoio einai o constructor 
                //gia na perasoume to errorResponse sto mvc controller
                //den mporoume na prosthesoume to error response se auto to type T 
            }
        }
        
        //adding the json web token to the HTTP header
        private async Task AddJwtToHeader(HttpClient httpClient)
        {
            // afou exei ginei dependency injection mporw na parw tis ruthmiseis apo to configuration
            JwtToken? token = null;
            string? strToken=httpContextAccessor.HttpContext?.Session.GetString("access_token");
            if(!string.IsNullOrWhiteSpace(strToken))
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strToken);
            }

            if (token ==null || token.ExpiresAt <= DateTime.UtcNow)
            {
                //contacting the authentication endpoint gia na kanoume ontos authenticate kai na paroume token
                var clientId = configuration.GetValue<string>("ClientId");
                var secret = configuration.GetValue<string>("Secret");
                

                //Authenticate against the authority -app credentials class
                var authoClient = httpClientFactory.CreateClient(AuthApiName);
                var response = await authoClient.PostAsJsonAsync("auth", new AppCredential
                {
                    ClientId =clientId,
                    Secret = secret
                });
                response.EnsureSuccessStatusCode(); 




                //get the JWT from the authority
                strToken = await response.Content.ReadAsStringAsync();
                //desiarilizing json
                token = JsonConvert.DeserializeObject<JwtToken>(strToken); //tin klasi JwtToken

                httpContextAccessor.HttpContext?.Session.SetString("access_token", strToken); //saving token into session

            }

            

            //pass the JWT to endpoints through the http headers
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken); //to AccessToken einai auto pou periexei to actuall AccessToken
        
        }
    
    }
}
