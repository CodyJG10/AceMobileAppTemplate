using AceMobileAppTemplate.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static AceMobileAppTemplate.Api.DatabaseManager;

namespace AceMobileAppTemplate.Api
{
    public interface IDatabaseManager
    {
        void SetToken(string token);
        Task<HttpResponseMessage> GetUserData();
        Task<HttpResponseMessage> RefreshToken(string token, string refreshToken);
        Task<HttpResponseMessage> Login(string email, string password);
        Task<HttpResponseMessage> Register(string email, string password, string confirmPassword);
        void ForgotPassword(string email);

        Task<string> RegisterDevice();
        Task<bool> EnablePushNotifications(string id, DeviceRegistration deviceUpdate);
    }
}