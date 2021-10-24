## Azure Service Bus - Best Practices - Notes

#### Azure Service Bus - Best Practices

- When you have clients that send or receive messages from the Azure Service Bus, the clients normally of the type IQueueClient.
- In the background these objects make use of the MessagingFactory object. This provides the internal management of connections.
- Don’t close the connections directly after sending or receiving messages, since establishing a connection is an expensive operation.
- Hence use the same client object for multiple operation

**Client batching** can be done with the prior version of the SDK – WindowsAzure.ServiceBus.SDK. Client batching delays the sending of messages and instead sends pending messages as a batch.

**Batched store access** is when the queue itself batches multiple messages before it is written to the internal store. This helps in better throughput.

**Enable Prefetch** – Here the receiver quietly acquires more messages from the queue or topic subscription. This is up to a value defined by the PrefetchCount limit.

When a receiver wants to receive messages, the messages are received from the buffer based on the number of prefetched messages. Then additional messages are prefetched again in the background.

The issues with Prefetch. Here in ReceiveAndDelete mode , remember when the messages are prefetched, they are removed from the queue. If the application crashes before the messages are processed, the messages are lost.

You can set the Prefetch count for the clients. QueueClient.PrefetchCount or SubscriptionClient.PrefetchCount



If you need to implement a high throughput queue with a small number of senders and receivers

- Use multiple message factories to create senders

- Use asynchronous operations



If you want to decrease the latency of sending or receiving messages.

- You can disable client batching of messages and batched store access.

- If you have a single client, consider using a prefetch count up to 20 times the processing rate of the receiver.



If you need to implement a high throughput queue with a large number of senders and small number of receivers

- For the sender that resides in a different process, use only one single factory per process.

- Use asynchronous operations and take advantage of client-side batching.

- Leave the setting of batched store access enabled.

- Set the prefetch count to 20 times that of the maximum processing rates of all receivers of a factory.



If you need to implement a high throughput queue with a small number of senders and large number of receivers

- If each receiver is in a different process , use only a single factory per process.

- Leave batched store access enabled.

- Set the prefetch count to a lower value. This is because you have multiple receivers.



If you need to implement a high throughput topic with a small number of senders and small number of subscriptions

- Increase the overall send rate by using multiple message factories to create senders

- Increase the overall receive rate by using multiple message factories to create receivers

- Use asynchronous operations and client-side batching.

- Leave batched store access enabled.

- Set the prefetch count to 20 times the maximum processing rates of all receivers of a factory