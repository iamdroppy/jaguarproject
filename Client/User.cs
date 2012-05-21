using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Globalization;
using System.Threading;
using JaguarProject.IO;

namespace JaguarProject.Client
{
    class User
    {
        #region Registration Variables
        public string registration_username;
        public string registration_password;
        public string registration_email;
        public string registration_DOB;
        public string registration_figure;
        #endregion

        private Socket connection;
        private byte[] bufferData = new byte[1024];
        public DBConnection db;
        public bool finishIt = false;
        public User(Socket newClient)
        {
            connection = newClient;
        }
        public void handleServices()
        {
            sendData("@@\u0001");
            db = new DBConnection();
            db.connect();
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
                processPacket(data);
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
        public void sendData(string data)
        {
            byte[] encData = System.Text.Encoding.ASCII.GetBytes(data);
            connection.Send(encData);
        }
        public void processPacket(string data)
        {
            string b64head = null, packet = null, header = null;
            int packetLen;
            try
            {
                b64head = data.Substring(1, 2);
                packetLen = Data.decodeB64(b64head);
                packet = data.Substring(3, packetLen);
                header = packet.Substring(0, 2);
            }
            catch { }
            switch (header)
            {
                #region Registration Packets
                case "CN": //Common welcome message
                    sendData("DUIH\u0001");
                    break;

                case "CJ": //Crypto initialization
                    sendData("DARAHIIIKHJIPAIQAdd-MM-yyyy\u0002\u0001");
                    sendData("@H[100,105,110,115,120,125,130,135,140,145,150,155,160,165,170,175,176,177,178,180,185,190,195,200,205,206,207,210,215,220,225,230,235,240,245,250,255,260,265,266,267,270,275,280,281,285,290,295,300,305,500,505,510,515,520,525,530,535,540,545,550,555,565,570,575,580,585,590,595,596,600,605,610,615,620,625,626,627,630,635,640,645,650,655,660,665,667,669,670,675,680,685,690,695,696,700,705,710,715,720,725,730,735,740]\u0001");
                    break;

                case "@q": //Registration request
                    sendData("Bc01-01-2012\u0001");
                    break;

                case "@j": //Client sends desired username, and a hefty process to check if it is used is started.
                    int nameLen = Data.decodeB64(packet.Substring(2, 2));
                    string userName = packet.Substring(4, nameLen);
                    if (DBFunctions.userTaken(db.connection, userName) == 1)
                        sendData("@dPA\u0001");
                    else if (DBFunctions.userTaken(db.connection, userName) == 0)
                        sendData("@dH\u0001");
                    else
                        ConsoleLog.LogErrorLine("Unable to perform usercheck.");
                        finishIt = true;
                    break;

                case "CK": //When user sends the username and password of their choice
                    int userNameLen = Data.decodeB64(packet.Substring(2, 2));
                    string name = packet.Substring(4, userNameLen);
                    int passLen = Data.decodeB64(packet.Substring((4 + userNameLen), 2));
                    string pass = packet.Substring((6 + userNameLen), passLen);
                    ConsoleLog.LogErrorLine("Desired username: " + name + " and desired pass: " + pass);
                    if (DBFunctions.userTaken(db.connection, name) == 1)
                        sendData("@dPA\u0001");
                    else if (DBFunctions.userTaken(db.connection, name) == 0)
                    {
                        registration_username = name;
                        registration_password = pass;
                        sendData("DZH\u0001");
                    }
                    else
                        ConsoleLog.LogErrorLine("Unable to perform usercheck.");
                        finishIt = true;
                    break;

                case "CE": //User sends email packet
                    int emailLen = Data.decodeB64(packet.Substring(2,2));
                    string email = packet.Substring(4, emailLen);
                    registration_email = email;
                    sendData("DO\u0001");
                    break;

                case "BR": //User sends date of birth with username again
                    int dobLen = Data.decodeB64(packet.Substring(2, 2));
                    string dob = packet.Substring(4, dobLen);
                    registration_DOB = dob;
                    int userLen = Data.decodeB64(packet.Substring((4 + dobLen), 2));
                    string unam = packet.Substring((6 + dobLen), userLen);
                    if (unam == registration_username)
                        sendData("CY1\u0001");
                    else
                    {
                        sendData("BKYou are sending invalid registration data.\u0001");
                        sendData("DARAHIIIKHJIPAIQAdd-MM-yyyy\u0002\u0001");
                        sendData("@H[100,105,110,115,120,125,130,135,140,145,150,155,160,165,170,175,176,177,178,180,185,190,195,200,205,206,207,210,215,220,225,230,235,240,245,250,255,260,265,266,267,270,275,280,281,285,290,295,300,305,500,505,510,515,520,525,530,535,540,545,550,555,565,570,575,580,585,590,595,596,600,605,610,615,620,625,626,627,630,635,640,645,650,655,660,665,667,669,670,675,680,685,690,695,696,700,705,710,715,720,725,730,735,740]\u0001");
                    }
                    break;

                case "@n": //User confirms registration information
                    sendData("DO\u0001");
                    break;

                case "@k": //Huge registration finalization packet
                    int namelenlen = Data.decodeB64(packet.Substring(2, 2));
                    int namelen = Data.decodeB64(packet.Substring(4, namelenlen));
                    string finalName = packet.Substring(5, namelen);
                    int applen = Data.decodeB64(packet.Substring((7+namelen), 2));
                    //string app 
                    break;

                #endregion

                default:
                    break;
            }
        }
    }
}
