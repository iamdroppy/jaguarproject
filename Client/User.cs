using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using JaguarProject.IO;

namespace JaguarProject.Client
{
    class User
    {
        public Socket clientSocket;
        public Stream clientStream;
        public StreamReader downStream;
        public StreamWriter upStream;

        public User(object connection)
        {
            clientSocket = (Socket)connection;
            clientStream = new NetworkStream(clientSocket);
            downStream = new StreamReader(clientStream);
            upStream = new StreamWriter(clientStream);
            upStream.AutoFlush = true;
            ConsoleLog.LogLine("Client connected and bound from " + clientSocket.RemoteEndPoint.ToString());
        }

        public void handleClient()
        {
            while (true)
            {
                upStream.Write("HI THERE!");
            }
        }
    }
}
