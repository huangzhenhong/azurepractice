using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzurePractice.FunctionApp.ServiceBusQueue
{
    public static class GetMessages
    {
        // This Azure Function will get messages from Service Bus Queue
        [FunctionName("GetMessages")]
        public static void Run([ServiceBusTrigger("myorderedqueue", Connection = "servicebus-connection")]Message myQueueItem, ILogger log)
        {
            log.LogInformation($"Body of the message: {Encoding.UTF8.GetString(myQueueItem.Body)}");
            log.LogInformation($"Content Type: {myQueueItem.ContentType.ToString()}");
        }
    }
}
