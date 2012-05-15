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
        public static TcpListener listener;
        public static XmlReader config;
        public static string hotelName;
        public static int serverPort;

        public static void Main(string[] args)
        {
            Console.WriteLine("JaguarProject Sulake Emulator");
            Console.WriteLine("Development Version 0.0.1\n");
            ConsoleLog.LogLine("Opening configuration file (Config.xml)");
            readXml();
            startServer();
            mainLoop();
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
        public static void startServer()
        {
            ConsoleLog.LogLine("Starting server on port " + serverPort);
            try
            {
                listener = new TcpListener(serverPort);
                listener.Start();
                ConsoleLog.LogLine("Successfully started. Now listening for clients.");
            }
            catch
            {
                ConsoleLog.LogError("Unable to start on port " + serverPort + " due to port/permissions conflicts.");
            }
        }
        public static void mainLoop()
        {
            while (true)
            {
                User userClient = new User(listener.AcceptSocket());
                Thread userThread = new Thread(new ThreadStart(userClient.handleClient));
                userThread.Start();
            }
        }
    }
}
