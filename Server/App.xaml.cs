using System;
using System.Threading;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace CHATAPP
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e); // Call the base OnStartup to properly start the WPF application

            // Start the server on a separate thread
            Thread serverThread = new Thread(StartServer);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void addMessageToUI(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.TextMessagesLines.Items.Add(message);
                }
            });
        }

        private string[] seperateUsernameAndMessage(string item)
        {
            string[] splittedString = item.Split(' ');
            string username = splittedString[0];
            string messageToShow = string.Join(' ', splittedString.Skip(1));
            string[] usernameAndMessageSeperated = { messageToShow, username };
            return usernameAndMessageSeperated;
        }

        private void handleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            string username = null;
            NetworkStream stream = client.GetStream();
            Byte[] bytes = new Byte[256];
            string data = null;
            int i;

            try
            {
                while (true)
                {
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to an ASCII string
                        data = Encoding.ASCII.GetString(bytes, 0, i);

                        // Send back a response to the client
                        byte[] msg = Encoding.ASCII.GetBytes(data);
                        stream.Write(msg, 0, msg.Length);
                        if (data.Split(' ').Length > 1)
                        {
                            string[] usernameAndMessageSeperated = seperateUsernameAndMessage(data);
                            addMessageToUI($"{usernameAndMessageSeperated[0]}: {usernameAndMessageSeperated[1]}");
                        }
                        else
                        {
                            username = data;
                            addMessageToUI($"{username} has connected to the server!");
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                addMessageToUI("SocketException");
            }
            catch (IOException ex)
            {
                addMessageToUI($"{username} have disconnected!");
            }
            finally
            {
                client.Close();
            }

        }

        private void StartServer()
        {
            int port = 13000;
            TcpListener server = null;

            try
            {
                // Initialize TcpListener
                server = new TcpListener(IPAddress.Any, port);
                server.Start();


                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Thread handlingClientThread = new Thread(handleClient);
                    handlingClientThread.IsBackground = true;
                    handlingClientThread.Start(client);
                }
            }
            catch (SocketException ex)
            {
                addMessageToUI("SocketException");
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
