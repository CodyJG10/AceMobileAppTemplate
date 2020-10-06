/**
 * Uncomment this class to utilize Azure Event Hubs
 * Add the following NuGet packages to the api project
 * - Azure.Messaging.EventHubs
 * - Azure.Messaging.EventHubs.Processor
 * 
 */

//using AceMobileAppTemplate.Api;
//using AceMobileAppTemplate.Models;
//using Autofac;
//using Newtonsoft.Json;
//using Plugin.Settings;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AceMobileAppTemplate.Mobile.Util
//{
//    public class AutoUpdater
//    {
//        private readonly IDatabaseManager _db;
//        private readonly UserData _userData;
//        private readonly CloudEventListener eventListener;

//        public AutoUpdater()
//        {
//            _db = App.Container.Resolve<IDatabaseManager>();
//            _userData = JsonConvert.DeserializeObject<UserData>(CrossSettings.Current.GetValueOrDefault("UserData", ""));

//            string eventHubConnectionString;
//            string eventHubName;
//            string storageConnectionString;
//            string storageContainerName;

//            eventListener = new CloudEventListener(eventHubConnectionString, eventHubName, storageContainerName, storageConnectionString);
//        }

//        public void Start()
//        {
//            CloudEventListener.OnCloudEventReceived += CloudEventListener_CloudEventReceivedEvent;
//            eventListener.Start();
//        }

//        public void Stop()
//        {
//            CloudEventListener.OnCloudEventReceived -= CloudEventListener_CloudEventReceivedEvent;
//            eventListener.Stop();
//        }

//        private void CloudEventListener_CloudEventReceivedEvent(string msg)
//        {
//            try
//            {
//                var updateEvent = JsonConvert.DeserializeObject<UpdateEvent>(msg);
//                if (updateEvent.User == _userData.UserName)
//                {

//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }
//        }
//    }
//}