using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

class Program
{
    private const string BrokerUri = "activemq:tcp://localhost:61616?wireFormat.tightEncodingEnabled=true"; // Change this if your ActiveMQ broker is on a different host or port
    private const string QueueName = "genesis.queue";

    static void Main(string[] args)
    {
        // Sending a message
        // SendMessage("Hello, JMS Queue!");

        // Receiving a message
        ReceiveMessages();
    }

    static void SendMessage(string message)
    {
        // Create a connection factory
        IConnectionFactory factory = new ConnectionFactory(BrokerUri);

        // Create a connection
        using IConnection connection = factory.CreateConnection();
        connection.Start();

        // Create a session
        using (ISession session = connection.CreateSession(AcknowledgementMode.ClientAcknowledge))
        {

            // Create the destination (queue)
            IDestination destination = session.GetQueue(QueueName);

            // Create a producer
            using (IMessageProducer producer = session.CreateProducer(destination))
            {
                producer.DeliveryMode = MsgDeliveryMode.Persistent;

                // Create a text message
                ITextMessage textMessage = producer.CreateTextMessage(message);

                // Send the message
                producer.Send(textMessage);
            }
        }


        Console.WriteLine($"Sent message: {message}");
    }

    static string ReceiveMessage()
    {
        // Create a connection factory
        IConnectionFactory factory = new ConnectionFactory(BrokerUri);

        // Create a connection
        using (IConnection connection = factory.CreateConnection())
        {
            connection.Start();

            // Create a session
            using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
            {
                // Create the destination (queue)
                IDestination destination = session.GetQueue(QueueName);

                // Create a consumer
                using IMessageConsumer consumer = session.CreateConsumer(destination);

                // Receive the message
                ITextMessage message = consumer.Receive() as ITextMessage;
                if (message != null)
                {
                    Console.WriteLine($"Received message: {message.Text}");
                }
                else
                {
                    Console.WriteLine("No message received.");
                }
                return message.Text;
            }
        }

    }


    static List<ITextMessage> ReceiveMessages()
    {
        // Create a connection factory
        IConnectionFactory factory = new ConnectionFactory(BrokerUri);
        List<ITextMessage> textMessages = new List<ITextMessage>();

        // Create a connection
        using (IConnection connection = factory.CreateConnection())
        {
            connection.Start();

            // Create a session
            using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
            {
                // Create the destination (queue)
                IDestination destination = session.GetQueue(QueueName);

                // Create a consumer
                using IMessageConsumer consumer = session.CreateConsumer(destination);

                while (true)
                {
                    IMessage message = consumer.Receive();
                    if (message is ITextMessage textMessage)
                    {
                        Console.WriteLine("Received message: " + textMessage.Text);
                        textMessages.Add(textMessage);
                    }
                    else if (message == null)
                    {
                        // No more messages in the queue
                        break;

                    }
                }

                // Receive the message
                // ITextMessage message = consumer.Receive() as ITextMessage;
                // if (message != null)
                // {
                //     Console.WriteLine($"Received message: {message.Text}");
                // }
                // else
                // {
                //     Console.WriteLine("No message received.");
                // }
                return textMessages;
            }
        }

    }
}
