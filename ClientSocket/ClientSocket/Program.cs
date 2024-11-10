using System;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            string username;
            do
            {
                Console.WriteLine("Please enter a username: ");
                username = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(username));
            // Set the TcpClient
            string server = "127.0.0.1";
            int port = 13000;

            TcpClient client = new TcpClient(server, port);
            NetworkStream stream = client.GetStream();
            Byte[] displayUsername = Encoding.ASCII.GetBytes(username);

            stream.Write(displayUsername, 0, displayUsername.Length);


            // Translate the passed message into ASCII and store it as a Byte array
            while (true)
            {
                Console.WriteLine("Type A message to send to the server");
                String userMessage = Console.ReadLine();
                if (userMessage == "end")
                {
                    break;
                }

                string usernameAndMessage = $"{username} {userMessage}";
                Byte[] data = Encoding.ASCII.GetBytes(usernameAndMessage);
                

                // Send the message to the connected TcpServer
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", usernameAndMessage
                    );

                // Buffer to store the response bytes
                data = new Byte[256];

                // String to store the response ASCII representation
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

            }
            Console.WriteLine("Connection ended!");
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
    }
}