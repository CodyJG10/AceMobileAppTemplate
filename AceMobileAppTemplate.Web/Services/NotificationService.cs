using AceMobileAppTemplate.Entities;
using AceMobileAppTemplate.Entities.Constants;
using AceMobileAppTemplate.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.NotificationHubs;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AceMobileAppTemplate.Web.Services
{
    public class NotificationService
    {
        private NotificationHubClient _hub;

        public NotificationService(string hubName, string connectionString)
        {
            _hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubName);
        }

        public async Task<bool> RegisterForPushNotifications(string id, DeviceRegistration deviceUpdate, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            RegistrationDescription registrationDescription = null;
            int deviceType = 0;

            switch (deviceUpdate.Platform)
            {
                case "apns":
                    registrationDescription = new AppleRegistrationDescription(deviceUpdate.Handle, deviceUpdate.Tags);
                    deviceType = DeviceType.IOS;
                    break;
                case "fcm":
                    registrationDescription = new FcmRegistrationDescription(deviceUpdate.Handle, deviceUpdate.Tags);
                    deviceType = DeviceType.ANDROID;
                    break;
            }

            registrationDescription.RegistrationId = id;
            if (deviceUpdate.Tags != null)
                registrationDescription.Tags = new HashSet<string>(deviceUpdate.Tags);

            try
            {
                var user = await userManager.FindByNameAsync(deviceUpdate.Tags[0].Split(":")[1]);

                if (!context.UserClaims.Any(x => x.UserId == user.Id && x.ClaimType == "PushNotificationsProvider"))
                {
                    IdentityUserClaim<string> claim = new IdentityUserClaim<string>()
                    {
                        UserId = user.Id,
                        ClaimType = "PushNotificationsProvider",
                        ClaimValue = deviceType.ToString()
                    };
                    context.Add(claim);
                }
                else 
                {
                    var existingClaim = context.UserClaims.Single(x => x.UserId == user.Id && x.ClaimType == "PushNotificationsProvider");
                    existingClaim.ClaimValue = deviceType.ToString();
                    context.Update(existingClaim);
                }

                context.SaveChanges();

                await _hub.CreateOrUpdateRegistrationAsync(registrationDescription);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> CreateRegistrationId()
        {
            return await _hub.CreateRegistrationIdAsync();
        }

        #region Pushing Notifications

        public void SendNotification(string message, ApplicationDbContext context, IdentityUser user)
        {
            var notificationProvider = context.UserClaims.Single(x => x.ClaimType == "PushNotificationsProvider" && x.UserId == user.Id);
            if (notificationProvider != null && notificationProvider.ClaimValue != null)
            {
                switch (int.Parse(notificationProvider.ClaimValue))
                {
                    case DeviceType.IOS:
                        SendNotificationToApple(message, "username:" + user.Email);
                        break;
                    case DeviceType.ANDROID:
                        SendNotificationToAndroid(message, "username:" + user.Email);
                        break;
                }
            }
        }

        private async void SendNotificationToApple(string message, string toUsername)
        {
            string payload = "{\"aps\":{\"alert\":\"" + message + "\"}}";
            await _hub.SendAppleNativeNotificationAsync(payload, toUsername);
        }

        private async void SendNotificationToAndroid(string message, string toUsername)
        {
            string payload = "{ \"data\":{ \"message\":\"" + message + "\"} }";
            await _hub.SendFcmNativeNotificationAsync(payload, toUsername);
        }

        #endregion
    }
}
