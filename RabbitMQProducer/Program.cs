using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitMQProducer
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HELLO");
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("test-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            //var message = new { Name = "Producer", Message = "Hello!" };

            string? message = null;
            do 
            {
                Console.WriteLine("Please enter your message");
                message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                    SendMessage(channel, message);
            } while (!string.IsNullOrEmpty(message));

           
        }

        static void SendMessage(IModel channel, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", "test-queue", null, body);
            Console.WriteLine($"Send Message {message}");
        }
    }
    
}