using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using JaguarProject.IO;
using JaguarProject.Client;

namespace JaguarProject
{
    class Program
    {
        public static Socket server;
        public static Socket client;
        public static XmlReader config;
        public static string hotelName;
        public static int serverPort;

        //MySQL Connection Information

        public static void Main(string[] args)
        {
            Console.WriteLine("JaguarProject Sulake Emulator");
            Console.WriteLine("Development Version 0.0.1\n");
            ConsoleLog.LogLine("Opening configuration file (Config.xml)");
            readXml();
            MySQLConnect();
            startServer();
            while (true) { }
        }

        public static void readXml()
        {
            string currentNode = null;
            try
            {
                config = new XmlTextReader("Config.xml");
                while (config.Read())
                {
                    switch (config.NodeType)
                    {
                        case XmlNodeType.Element:
                            currentNode = config.Name;
                            break;
                        case XmlNodeType.Text:
                            switch (currentNode)
                            {
                                case "ServerPort":
                                    serverPort = int.Parse(config.Value);
                                    ConsoleLog.LogLine("Read configuration port as " + serverPort);
                                    break;
                                case "HotelName":
                                    hotelName = config.Value;
                                    ConsoleLog.LogLine("Read configuration 'Hotel Name' as " + hotelName);
                                    break;
                                case "Host":
                                    DBConnection.mysql_host = config.Value;
                                    break;
                                case "DBName":
                                    DBConnection.mysql_db = config.Value;
                                    break;
                                case "UserName":
                                    DBConnection.mysql_user = config.Value;
                                    break;
                                case "Password":
                                    DBConnection.mysql_user_pass = config.Value;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            break;
                    }
                }
            }
            catch
            {
                ConsoleLog.LogError("Config.xml was either not found or contained invalid data.");
                Console.Read();
                Environment.Exit(-1);
            }
        }
        public static void MySQLConnect()
        {
            DBConnection mainConnection = new DBConnection();
            if (mainConnection.connect())
                ConsoleLog.LogLine("Successfully connected to MySQL server.");
            else
            {
                Console.Read();
                Environment.Exit(-1);
            }
        }
        public static void startServer()
        {
            ConsoleLog.LogLine("Starting server on port " + serverPort);
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, serverPort));
                server.Listen(5);
                server.BeginAccept(new AsyncCallback(AcceptClient), null);
                ConsoleLog.LogLine("Successfully started. Now listening for clients.");
            }
            catch
            {
                ConsoleLog.LogError("Unable to start on port " + serverPort + " due to port/permissions conflicts.");
            }
        }
        public static void AcceptClient(IAsyncResult ar)
        {
            client = server.EndAccept(ar);
            server.BeginAccept(new AsyncCallback(AcceptClient), null);
            ConsoleLog.LogLine("Client connected from " + client.RemoteEndPoint);
            User newUser = new User(client);
            Thread userThread = new Thread(new ThreadStart(newUser.handleServices));
            userThread.Start();
        }
    }
}
