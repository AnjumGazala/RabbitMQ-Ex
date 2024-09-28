using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQConsumer
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HELLO");
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("test-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            //Fair Dispatch
            channel.BasicQos(prefetchSize:0, prefetchCount:1, global:false);
            Console.WriteLine("Waiting for message");
            var consumer = new EventingBasicConsumer(channel);

            //Event Handler
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
               
                if (message.Contains("exception"))
                {
                    Console.WriteLine("Error");
                    channel.BasicReject(e.DeliveryTag, false);
                  
                    throw new Exception("Error in processing");
                }

                if (int.TryParse(message, out int delayTime))
                {
                    Thread.Sleep(delayTime * 1000);

                }
                Console.WriteLine($"Processed message {message}");
                channel.BasicAck(e.DeliveryTag, false);
            };
            channel.BasicConsume("test-queue", false, consumer);

            Console.WriteLine("Enter to Exit");
            Console.ReadLine();
        }
    }
}