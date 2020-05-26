using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AceMobileAppTemplate.Api;
using AceMobileAppTemplate.Dependencies;
using AceMobileAppTemplate.Entities;
using AceMobileAppTemplate.iOS.Dependencies;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationRegistration))]
namespace AceMobileAppTemplate.iOS.Dependencies
{
    public class NotificationRegistration : INotificationRegistration
    {
        public async void RegisterForRemoteNotifications(string username, IDatabaseManager db)
        {
            var app = (AppDelegate)UIApplication.SharedApplication.Delegate;
            if (app.PushNotificationsHandle != null)
            {
                var id = await db.RegisterDevice();
                var handle = app.PushNotificationsHandle;
                DeviceRegistration deviceUpdate = new DeviceRegistration()
                {
                    Handle = handle,
                    Platform = "apns",
                    Tags = new string[] { "username:" + username }
                };

                var result = await db.EnablePushNotifications(id, deviceUpdate);
                Console.WriteLine(result);
            }
        }
    }
}