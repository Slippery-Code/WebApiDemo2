using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApp.Data;
using WebApp.Models;


namespace WebApp.Data
{
    public class WebApiExecuter : IWebApiExecuter
    //theloume na to kanoume dependency injected sto shirts controller sto mvc epishs kaname extract interface
    {
        private const string apiName = "ShirtsApi"; //arxikopoihsei tou string gia na to xrisimopoisoume katw 
        private readonly IHttpClientFactory httpClientFactory;

        //xrisimopoioume to HTTP client Find factory gia na dimiourgisoume enan httpClient kai tote xrisimopoioume to HttpClient gia na epikoinwnisoume me to web API
        //tha kanoume dependency inject to HTTP client factory sto web API executer
        //tha prepei na baloume ato sto services collection to opoio einai dependency injection container

        //1on dependency inject sto constructor
        public WebApiExecuter(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        //meta dimiourgoume diaforetikous methodous to opoio antistoixei se diaforetika http requests

        public async Task<T?> InvokeGet<T>(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
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
            var response = await httpClient.PostAsJsonAsync(relativeUrl, obj);
            await HandlePotentialError(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }
        public async Task InvokePut<T>(string relativeUrl, T obj) //oti exw edw ston Executer prepei na uparxei sto interface
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            var response = await httpClient.PutAsJsonAsync(relativeUrl, obj);
            await HandlePotentialError(response);
        }

        public async Task InvokeDelete(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
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
    
    }
}
