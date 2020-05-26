using AceMobileAppTemplate.Entities;
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

        public struct LoginResult
        {
            [JsonProperty("token")]
            public string Token { get; set; }
            [JsonProperty("refreshToken")]
            public string RefreshToken { get; set; }
        }

        public DatabaseManager(string url)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }

        public async Task<HttpResponseMessage> GetUserData()
        {
            return await _client.GetAsync("auth/userdata");
        }

        public async Task<HttpResponseMessage> Login(string username, string password)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Username", username),
                new KeyValuePair<string, string>("Password", password)
            });

            var response = await _client.PostAsync("auth/token", formContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                LoginResult content = JsonConvert.DeserializeObject<LoginResult>(result);
                string token = content.Token;
                SetToken(token);
            }

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

        public async Task<HttpResponseMessage> RefreshToken(string token, string refreshToken)
        {
            var formContent = new FormUrlEncodedContent(new[]
          {
                new KeyValuePair<string, string>("token", token),
                new KeyValuePair<string, string>("refreshtoken", refreshToken)
            });

            var result = await _client.PostAsync("auth/RefreshToken", formContent);
            return result;
        }

        public void SetToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

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
    }
}