using Syncfusion.SfBusyIndicator.XForms.iOS;
using Syncfusion.XForms.iOS.PopupLayout;
using Syncfusion.XForms.iOS.ComboBox;
using Syncfusion.XForms.iOS.DataForm;
using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.ListView.XForms.iOS;
using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using UserNotifications;
using Google.MobileAds;

namespace AceMobileAppTemplate.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public string PushNotificationsHandle { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            MobileAds.SharedInstance.Start(null);
            SfBusyIndicatorRenderer.Init();
            SfPopupLayoutRenderer.Init();
            SfComboBoxRenderer.Init();
            SfDataFormRenderer.Init();
            SfButtonRenderer.Init();
            SfListViewRenderer.Init();
            RegisterForRemoteNotifications();
            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }

        #region Notifications

        public void RegisterForRemoteNotifications()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert |
                    UNAuthorizationOptions.Sound |
                    UNAuthorizationOptions.Sound,
                    (granted, error) =>
                    {
                        if (granted)
                            InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                    });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");
            if (string.IsNullOrEmpty(oldDeviceToken))
            {

                byte[] bytes = deviceToken.ToArray<byte>();
                string[] hexArray = bytes.Select(b => b.ToString("x2")).ToArray();
                string newDeviceToken = string.Join(string.Empty, hexArray);

                NSUserDefaults.StandardUserDefaults.SetString("PushDeviceToken", newDeviceToken);
                PushNotificationsHandle = newDeviceToken;
            }
            else
            {
                PushNotificationsHandle = oldDeviceToken;
            }
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo, false);
        }

        void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            // make sure we have a payload
            if (options != null && options.ContainsKey(new NSString("aps")))
            {
                // get the APS dictionary and extract message payload. Message JSON will be converted
                // into a NSDictionary so more complex payloads may require more processing
                NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
                string payload = string.Empty;
                NSString payloadKey = new NSString("alert");
                if (aps.ContainsKey(payloadKey))
                {
                    payload = aps[payloadKey].ToString();
                }
            }
            else
            {
                Console.WriteLine($"Received request to process notification but there was no payload.");
            }
        }

        #endregion
    }
}
