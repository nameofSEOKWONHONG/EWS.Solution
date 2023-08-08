// using Azure.Messaging.ServiceBus;
//
// namespace EWS.Domain.Infra.ServiceBus;
//
// public class ServiceHubProducer
// {
//     string connectionString = "<connection_string>";
//     string queueName = "<queue_name>";
//     
//     public ServiceHubProducer()
//     {
//         
//     }
//
//     public async Task Producer()
//     {
//         await using var client = new ServiceBusClient(connectionString);
//         
//         // create the sender
//         ServiceBusSender sender = client.CreateSender(queueName);
//         
//         // create a message that we can send. UTF-8 encoding is used when providing a string.
//         ServiceBusMessage message = new ServiceBusMessage("Hello world!");
//
//         // send the message
//         await sender.SendMessageAsync(message);
//     }
// }
//
// public class ServiceHubConsumer
// {
//     string connectionString = "<connection_string>";
//     string queueName = "<queue_name>";
//     
//     public ServiceHubConsumer()
//     {
//         var client = new ServiceBusClient(connectionString);
//         var options = new ServiceBusProcessorOptions 
//         {
//
//             AutoCompleteMessages = false,
//             MaxConcurrentCalls = 20
//         };
//     }
//
//     public async Task Consumer()
//     {
//         await using var client = new ServiceBusClient(connectionString);
//         
//         // create a receiver that we can use to receive the message
//         ServiceBusReceiver receiver = client.CreateReceiver(queueName);
//
//         // the received message is a different type as it contains some service set properties
//         ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
//         
//         // get the message body as a string
//         string body = receivedMessage.Body.ToString();
//         
//         //TODO : PROCESS LOGIC
//         
//         Console.WriteLine(body);
//     }
// }