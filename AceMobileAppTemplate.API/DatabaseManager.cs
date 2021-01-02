using AceMobileAppTemplate.Entities;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AceMobileAppTemplate.Api
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly HttpClient _client;

        public DatabaseManager(string url)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }

        public async Task<HttpResponseMessage> GetUserData(string id = null, string email = null)
        {
            string callParams;
            if (id != null)
            {
                callParams = "?id=" + id;
            }
            else
            {
                callParams = "?email=" + email;
            }
            var response = await _client.GetAsync("api/UserData" + callParams);
            return response;
        }

        #region Account
        public async Task<HttpResponseMessage> Authenticate()
        {
            var disco = await _client.GetDiscoveryDocumentAsync(_client.BaseAddress.AbsoluteUri);
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }

            // request token
            var tokenResponse = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }

            _client.SetBearerToken(tokenResponse.AccessToken);
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
        public async Task<HttpResponseMessage> Login(string username, string password)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
            });

            var response = await _client.PostAsync("api/login", formContent);
            return response;
        }
        public async Task<HttpResponseMessage> Register(string email, string password, string confirmPassword)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", email),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("confirmPassword", confirmPassword)
            });

            var response = await _client.PostAsync("auth/register", formContent);
            return response;
        }
        public async void ForgotPassword(string email)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email)
            });

            await _client.PostAsync("auth/ForgotPassword", formContent);
        }
        #endregion

        #region Push Notifications
        public async Task<string> RegisterDevice()
        {
            var response = await _client.GetAsync("notifications/Register");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<bool> EnablePushNotifications(string id, DeviceRegistration deviceUpdate)
        {
            string json = JsonConvert.SerializeObject(deviceUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("notifications/enable/" + id, content);
            return response.IsSuccessStatusCode;
        }
        #endregion
    }
}