using Azure.Messaging.ServiceBus;
using AzurePractice.Model;
using System;
using System.Collections.Generic;

namespace AzurePractice.ConsoleApp.Topics
{
    class Program
    {
        // Get this connection string from "Shared access policies" of the topic
        private static string _connectionString = "Endpoint=sb://appnamespace1000s.servicebus.windows.net/;SharedAccessKeyName=managePolicy;SharedAccessKey=zPZpi6z12cOqbollkiHbe3IL7v36XADpVNGlHKo4PJY=;EntityPath=breakingnews";
        private static string _topicName = "breakingnews";
        static void Main(string[] args)
        {
            ServiceBusClient _client = new ServiceBusClient(_connectionString);

            SendMessageTopic(_client);

            //PeekMessages(_client);

            //PeekMessagesWithAck(_client);

            //PeekMessagesInDeadLetteringQueue();

            Console.ReadLine();
        }

        private static void PeekMessages(ServiceBusClient _client)
        {
            var receiveOptions = new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock };
            ServiceBusReceiver _receiver = _client.CreateReceiver(_topicName, "subscriptionA", receiveOptions);

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
            ServiceBusReceiver _receiver = _client.CreateReceiver(_topicName,
                            new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

            //Single message
            ServiceBusReceivedMessage _message = _receiver.ReceiveMessageAsync().GetAwaiter().GetResult();
            Console.WriteLine(_message.Body.ToString());

            // Need to delete the message in the code
            _receiver.CompleteMessageAsync(_message);
        }

        private static void SendMessageTopic(ServiceBusClient _client)
        {
            List<Order> orders = new List<Order>() {
                new Order() { OrderId = "01", Quantity = 10, UnitPrice = 3.4m },
                new Order() { OrderId = "02", Quantity = 12, UnitPrice = 13.4m },
                new Order() { OrderId = "03", Quantity = 15, UnitPrice = 2.0m },
                new Order() { OrderId = "04", Quantity = 20, UnitPrice = 5.2m },
                new Order() { OrderId = "05", Quantity = 11, UnitPrice = 6.8m },
            };

            ServiceBusSender _sender = _client.CreateSender(_topicName);

            int i = 1;
            foreach (Order o in orders)
            {
                ServiceBusMessage message = new ServiceBusMessage(o.ToString());
                message.ContentType = "application/json";
                message.MessageId = i.ToString();
                // Specify the time to live time
                //mesage.TimeToLive = TimeSpan.FromSeconds(30);
                _sender.SendMessageAsync(message).GetAwaiter().GetResult();
                i++;
            }

            Console.WriteLine("All of the message were sent!");

        }

        private static void PeekMessagesInDeadLetteringQueue()
        {
            var deadLetterQueue = "$DeadLetterQueue";
            var connStr = _connectionString + "/" + deadLetterQueue;
            var queueName = _topicName + "/" + deadLetterQueue;

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
