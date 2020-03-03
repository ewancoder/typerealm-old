namespace TypeRealm.ConsoleApp.Networking
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using Messages;

    internal sealed class Connection : IDisposable
    {
        private readonly string _server;
        private readonly int _port;
        private readonly AuthInformation _authInformation;
        private TcpClient _client;

        public Connection(string server, int port, AuthInformation authInformation)
        {
            _server = server;
            _port = port;
            _authInformation = authInformation;
            _client = new TcpClient();
        }

        public Stream Stream { get; private set; }

        public void ReconnectAndAuthorize()
        {
            DisposeConnection();

            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    _client = new TcpClient();
                    _client.Connect(_server, _port);
                    Stream = _client.GetStream();

                    Send(new Authorize
                    {
                        PlayerId = _authInformation.PlayerName
                    });

                    break;
                }
                catch
                {
                    DisposeConnection();
                }
            }
        }

        public object ReceiveMessage()
        {
            // TODO: Add pinging and reconnecting if connection is lost.
            return MessageSerializer.Deserialize(Stream);
        }

        public void Send(object message)
        {
            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    MessageSerializer.Serialize(Stream, message);
                    return;
                }
                catch
                {
                    if (i == 5)
                        throw;

                    ReconnectAndAuthorize();
                }
            }
        }

        public void Dispose()
        {
            DisposeConnection();
        }

        private void DisposeConnection()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }

            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
    }
}
