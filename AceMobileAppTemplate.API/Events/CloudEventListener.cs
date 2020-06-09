/**
 * Uncomment this class to utilize Azure Event Hubs
 * Add the following NuGet packages to the api project
 * - Azure.Messaging.EventHubs
 * - Azure.Messaging.EventHubs.Processor
 * 
 */

//using Azure.Messaging.EventHubs;
//using Azure.Messaging.EventHubs.Consumer;
//using Azure.Messaging.EventHubs.Processor;
//using Azure.Storage.Blobs;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;

//namespace AceMobileAppTemplate.API.Events
//{
//    public class CloudEventListener
//    {
//        public delegate void CloudEventReceived(string message);
//        public static event CloudEventReceived OnCloudEventReceived;

//        private readonly EventProcessorClient eventProcessor;

//        public CloudEventListener(string eventHubConnectionString,
//                                    string eventHubName,
//                                    string storageContainerName,
//                                    string storageConnectionString)
//        {
//            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
//            BlobContainerClient storageClient = new BlobContainerClient(storageConnectionString, storageContainerName);

//            var processorOptions = new EventProcessorClientOptions
//            {
//                ConnectionOptions = new EventHubConnectionOptions
//                {
//                    TransportType = EventHubsTransportType.AmqpWebSockets,
//                    Proxy = (IWebProxy)null
//                },

//                RetryOptions = new EventHubsRetryOptions
//                {
//                    MaximumRetries = 5,
//                    TryTimeout = TimeSpan.FromMinutes(1)
//                }
//            };

//            eventProcessor = new EventProcessorClient(storageClient, consumerGroup, eventHubConnectionString, eventHubName, processorOptions);

//            eventProcessor.ProcessEventAsync += EventProcessor_ProcessEventAsync; ;
//            eventProcessor.ProcessErrorAsync += EventProcessor_ProcessErrorAsync;
//        }

//        private Task EventProcessor_ProcessErrorAsync(ProcessErrorEventArgs arg)
//        {
//            return Task.CompletedTask;
//        }

//        private async Task<Task> EventProcessor_ProcessEventAsync(ProcessEventArgs arg)
//        {
//            try
//            {
//                var data = arg.Data;
//                string msg = Encoding.UTF8.GetString(data.Body.ToArray());
//                OnCloudEventReceived?.Invoke(msg);
//                await arg.UpdateCheckpointAsync(arg.CancellationToken);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }
//            return Task.CompletedTask;
//        }

//        public async void Stop()
//        {
//            await eventProcessor.StopProcessingAsync();
//        }

//        public async void Start()
//        {
//            await eventProcessor.StartProcessingAsync();
//        }
//    }
//}