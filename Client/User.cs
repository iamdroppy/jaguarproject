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
        private Socket connection;
        private byte[] bufferData = new byte[1024];

        public User(Socket newClient)
        {
            connection = newClient;
        }

        public void handleServices()
        {
            connection.BeginReceive(bufferData, 0, bufferData.Length, SocketFlags.None, new AsyncCallback(OnArrival), connection);
        }

        public void OnArrival(IAsyncResult ar)
        {
            connection = (Socket)ar.AsyncState;
            string data;
            int bytesRead = connection.EndReceive(ar);
            if (IsConnected(connection))
                connection.BeginReceive(bufferData, 0, bufferData.Length, SocketFlags.None, new AsyncCallback(OnArrival), connection);
            else
                ConsoleLog.LogLine("Client from " + connection.RemoteEndPoint + " has disconnected.");
            if (bytesRead > 0)
            {
                data = System.Text.Encoding.ASCII.GetString(bufferData, 0, bytesRead);
                ConsoleLog.LogLine(connection.RemoteEndPoint + " SENT: " + data);
            }
        }

        static bool IsConnected(Socket connection)
        {
            try
            {
                return !(connection.Poll(1, SelectMode.SelectRead) && connection.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
