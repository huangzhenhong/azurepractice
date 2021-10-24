using Azure.Messaging.ServiceBus;
using AzurePractice.Model;
using System;
using System.Collections.Generic;

namespace AzurePractice.ConsoleApp.ServiceBus
{
    // Test Azure Service Bus Queue
    class Program
    {
        // Get this connection string from "Shared access policies"
        private static string _connectionString = "Endpoint=sb://appnamespace1000s.servicebus.windows.net/;SharedAccessKeyName=SendPolicy;SharedAccessKey=f+fy6oVuSTJTF+vuzy0eAnicSPh00IQsn8gNBFcWfoM=;EntityPath=myorderedqueue";
        private static string _queueName = "myorderedqueue";
        private static string _deadLetterQueueName = "myorderedqueue/$DeadLetterQueue";
        static void Main(string[] args)
        {
            ServiceBusClient _client = new ServiceBusClient(_connectionString);

            //SendMessageToServiceBusQueue(_client);

            //PeekMessages(_client);

            //PeekMessagesWithAck(_client);

            PeekMessagesInDeadLetteringQueue();

            Console.ReadLine();
        }

        private static void PeekMessages(ServiceBusClient _client)
        {
            //ServiceBusReceiveMode.PeekLock
            //ServiceBusReceiveMode.ReceiveAndDelete, this will delete the message
            ServiceBusReceiver _receiver = _client.CreateReceiver(_queueName,
                            new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

            //Single message
            //ServiceBusReceivedMessage _message = _receiver.ReceiveMessageAsync().GetAwaiter().GetResult();
            //Console.WriteLine(_message.Body.ToString());

            // Multiple messges
            var messages = _receiver.ReceiveMessagesAsync(3).GetAwaiter().GetResult();
            foreach (var m in messages)
            {
                Console.WriteLine(m.Body.ToString());
            }
        }

        private static void PeekMessagesWithAck(ServiceBusClient _client)
        {
            //ServiceBusReceiveMode.PeekLock
            //ServiceBusReceiveMode.ReceiveAndDelete, this will delete the message
            ServiceBusReceiver _receiver = _client.CreateReceiver(_queueName,
                            new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

            //Single message
            ServiceBusReceivedMessage _message = _receiver.ReceiveMessageAsync().GetAwaiter().GetResult();
            Console.WriteLine(_message.Body.ToString());

            // Need to delete the message in the code
            _receiver.CompleteMessageAsync(_message);
        }

        private static void SendMessageToServiceBusQueue(ServiceBusClient _client)
        {
            List<Order> orders = new List<Order>() {
                new Order() { OrderId = "01", Quantity = 10, UnitPrice = 3.4m },
                new Order() { OrderId = "02", Quantity = 12, UnitPrice = 13.4m },
                new Order() { OrderId = "03", Quantity = 15, UnitPrice = 2.0m },
                new Order() { OrderId = "04", Quantity = 20, UnitPrice = 5.2m },
                new Order() { OrderId = "05", Quantity = 11, UnitPrice = 6.8m },
            };

            ServiceBusSender _sender = _client.CreateSender(_queueName);

            foreach (Order o in orders)
            {
                ServiceBusMessage mesage = new ServiceBusMessage(o.ToString());
                mesage.ContentType = "application/json";
                // Specify the time to live time
                mesage.TimeToLive = TimeSpan.FromSeconds(30);
                _sender.SendMessageAsync(mesage).GetAwaiter().GetResult();
            }

            Console.WriteLine("All of the message were sent!");

        }

        private static void PeekMessagesInDeadLetteringQueue()
        {
            var deadLetterQueue = "$DeadLetterQueue";
            var connStr = _connectionString + "/"+ deadLetterQueue;
            var queueName = _queueName + "/" + deadLetterQueue;

            ServiceBusClient _client = new ServiceBusClient(connStr);

            ServiceBusReceiver _receiver = _client.CreateReceiver(queueName,
                            new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

            //Single message
            ServiceBusReceivedMessage _message = _receiver.ReceiveMessageAsync().GetAwaiter().GetResult();
            Console.WriteLine(_message.DeadLetterReason);
            Console.WriteLine(_message.DeadLetterErrorDescription);
            Console.WriteLine(_message.Body.ToString());

        }
    }
}
