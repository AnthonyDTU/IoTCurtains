using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Net.Security;
using System.Net.WebSockets.WebSocketFrame;

namespace SmartCurtainsFirmware
{
    public  class WebSocketTest : WebSocket
    {

        private NetworkStream _networkStream;

        //public override WebSocketState State { get; set; } = WebSocketState.Closed;
        public override System.Net.WebSockets.WebSocketFrame.WebSocketState State { get; set; }

        private Socket _tcpSocket;

        /// <summary>
        /// If a secure connection is used.
        /// </summary>
        public bool IsSSL { get; private set; } = false;

        /// <summary>
        /// The remote Host name to connect to.
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// The remote Port to connect to.
        /// </summary>
        public int Port { get; private set; }


        /// <summary>
        /// The remote Prefix to connect to.
        /// </summary>
        public string Prefix { get; private set; }



        public WebSocketTest(string uri, ClientWebSocketHeaders headers = null) 
        {
            State = System.Net.WebSockets.WebSocketFrame.WebSocketState.Connecting;

            var splitUrl = uri.Split(new char[] { ':', '/', '/' }, 4);

            if (splitUrl.Length == 4 && splitUrl[0] == "ws")
            {
                IsSSL = false;
            }
            else if (splitUrl.Length == 4 && splitUrl[0] == "wss")
            {
                IsSSL = true;
            }
            else
            {
                throw new Exception("websocket url should start with 'ws://' or 'wss://'");
            }

            string prefix = "/";

            splitUrl = splitUrl[3].Split(new char[] { '/' }, 2);

            if (splitUrl.Length == 2)
            {
                prefix += splitUrl[1];
            }

            Prefix = prefix;

            Port = IsSSL ? 443 : 80;

            splitUrl = splitUrl[0].Split(new char[] { ':' }, 2);
            Host = splitUrl[0];

            if (splitUrl.Length == 2)
            {
                if (splitUrl[1].Length < 8)
                {
                    try
                    {
                        Port = int.Parse(splitUrl[1]);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something is wrong with the port number of the websocket url");
                    }
                }
            }

            IPHostEntry hostEntry = Dns.GetHostEntry(Host);
            IPEndPoint ep = new IPEndPoint(hostEntry.AddressList[0], Port);

            byte[] buffer = new byte[1024];
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            NetworkStream stream = null;

            try
            {
                _tcpSocket.Connect(ep);

                if (IsSSL)
                {
                    Console.WriteLine("SSL not supported in test");

                    //SslStream sslStream = new SslStream(_tcpSocket);
                    //sslStream.SslVerification = SslVerification;
                    //sslStream.UseStoredDeviceCertificate = UseStoredDeviceCertificate;

                    //if (SslVerification != SslVerification.NoVerification && _certificate != null)
                    //{
                    //    sslStream.AuthenticateAsClient(Host, null, _certificate, SslProtocol);
                    //}
                    //else
                    //{
                    //    sslStream.AuthenticateAsClient(Host, SslProtocol);
                    //}

                    //_networkStream = sslStream;
                }
                else
                {
                    _networkStream = new NetworkStream(_tcpSocket, true);
                }

                WebSocketClientConnect(ep, prefix, Host, headers);
            }
            catch (SocketException ex)
            {
                _tcpSocket.Close();
                State = System.Net.WebSockets.WebSocketFrame.WebSocketState.Closed;
                Debug.WriteLine($"** Socket exception occurred: {ex.Message} error code {ex.ErrorCode}!**");
            }

            ConnectionClosed += WebSocket_ConnectionClosed;



        }

        private void WebSocketClientConnect(IPEndPoint remoteEndPoint, string prefix = "/", string host = null, ClientWebSocketHeaders customHeaders = null)
        {
            string customHeaderString = string.Empty;
            if (customHeaders != null)
            {
                var headerKeys = customHeaders.Keys;
                foreach (string key in headerKeys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        customHeaderString += $"{key}: {customHeaders[key]}\r\n";
                    }
                }
            }
            if (prefix[0] != '/') throw new Exception("websocket prefix has to start with '/'");

            byte[] keyBuf = new byte[16];
            new Random().NextBytes(keyBuf);
            string swk = Convert.ToBase64String(keyBuf);

            byte[] sendBuffer = Encoding.UTF8.GetBytes($"GET {prefix} HTTP/1.1\r\nHost: {(host != null ? host : remoteEndPoint.Address.ToString())}\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Key: {swk}\r\nSec-WebSocket-Version: 13\r\n{customHeaderString}\r\n");
            _networkStream.Write(sendBuffer, 0, sendBuffer.Length);

            string beginHeader = ($"HTTP/1.1 101".ToLower());
            byte[] bufferStart = new byte[beginHeader.Length];
            byte[] buffer = new byte[600];

            int bytesRead = _networkStream.Read(bufferStart, 0, bufferStart.Length);

            bool correctHandshake = false;

            if (bytesRead == bufferStart.Length)
            {
                if (Encoding.UTF8.GetString(bufferStart, 0, bufferStart.Length).ToLower() == beginHeader)
                {
                    //right http request
                    bytesRead = _networkStream.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 20)
                    {
                        var headers = WebSocketHelpers.ParseHeaders(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                        string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                        byte[] swkaSha1 = WebSocketHelpers.ComputeHash(swka);
                        string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                        if (((string)headers["connection"]).ToLower() == "upgrade" && ((string)headers["upgrade"]).ToLower() == "websocket" && (string)headers["sec-websocket-accept"] == swkaSha1Base64)
                        {
                            Debug.WriteLine("WebSocket Client connected");
                            correctHandshake = true;
                        }
                    }
                }
            }

            if (!correctHandshake)
            {
                State = System.Net.WebSockets.WebSocketFrame.WebSocketState.Closed;
                _tcpSocket.Close();

                throw new Exception("WebSocket did not receive right handshake");
            }

            ConnectToStream(_networkStream, false, _tcpSocket);



        }

        private void WebSocket_ConnectionClosed(object sender, EventArgs e)
        {
            _tcpSocket.Close();
        }

    }
}
