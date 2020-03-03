/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using TypeRealm.Domain;
    using TypeRealm.Messages;
    using TypeRealm.Server.Messaging;

    internal sealed class Server : IDisposable
    {
        private const int Port = 30100;
        private readonly object _lock = new object();
        private readonly TcpListener _listener;
        private readonly Logger _logger = new Logger();
        private readonly IPlayerRepository _playerRepository;
        private readonly BattleRepository _battleRepository;
        private readonly MessageDispatcher _dispatcher;
        private readonly List<ConnectedClient> _connectedClients
            = new List<ConnectedClient>();

        public Server(IPlayerRepository playerRepository, BattleRepository battleRepository)
        {
            _playerRepository = playerRepository;
            _battleRepository = battleRepository;

            _listener = new TcpListener(IPAddress.Parse("0.0.0.0"), Port);
            _listener.Start();
            _listener.BeginAcceptTcpClient(HandleConnection, _listener);

            _dispatcher = new MessageDispatcher(new InMemoryMessageHandlerFactory(
                _connectedClients, playerRepository, battleRepository));

            _logger.Log($"Listening on {Port}...");
        }

        private void HandleConnection(IAsyncResult result)
        {
            // Start waiting for another client as soon as some client has connected.
            _listener.BeginAcceptTcpClient(HandleConnection, _listener);

            try
            {
                using (var tcpClient = _listener.EndAcceptTcpClient(result))
                using (var stream = tcpClient.GetStream())
                {
                    var authorizeMessage = MessageSerializer.Deserialize(stream) as Authorize;
                    var playerId = authorizeMessage.PlayerId;

                    var player = _playerRepository.Find(playerId);
                    if (player == null)
                    {
                        player = new Player(playerId, playerId, 1, null);
                        _playerRepository.Save(player);
                    }

                    var client = new ConnectedClient(playerId, stream);

                    // TODO: Maybe refactor locks to thread-safe collection.
                    // Or maybe leave as it is. More clear, and log is binded to the actual order of things.
                    lock (_lock)
                    {
                        _connectedClients.Add(client);
                        _logger.Log($"{player.Name} has connected.");
                    }

                    try
                    {
                        // Send initial status after connect.
                        Update(playerId);

                        while (true)
                        {
                            var message = MessageSerializer.Deserialize(stream);

                            lock (_lock)
                            {
                                _logger.Log($"Received message: {message}");
                                _dispatcher.Dispatch(client, message);
                                Update(playerId);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        lock (_lock)
                        {
                            _logger.Log($"{player.Name} unexpectedly lost connection. {exception.Message}");
                            _connectedClients.Remove(client);
                            Update(playerId);
                        }

                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Log($"A client tried and failed to connect. {exception.Message}");
            }
        }

        public void Dispose()
        {
            if (_listener != null)
                _listener.Stop();
        }

        /// <summary>
        /// Updates all relevant players.
        /// </summary>
        /// <param name="playerId">Player that has changed.</param>
        private void Update(string playerId)
        {
            // TODO: Update only clients that are correlating with changed player.

            foreach (var client in _connectedClients)
            {
                var status = GetStatus(client.PlayerId);
                MessageSerializer.Serialize(client.Stream, status);
            }
        }

        private Status GetStatus(string playerId)
        {
            var player = _playerRepository.Find(playerId);
            var neighbors = _playerRepository.FindNeighborsOf(player);
            var onlineNeighbors = neighbors.Where(n => _connectedClients.Any(c => c.PlayerId == n.PlayerId));

            var playerStatus = player.GetPlayerStatus();
            var zoneStatus = new ZoneStatus
            {
                Neighbors = onlineNeighbors
                    .Select(n => new PlayerMessage
                    {
                        PlayerId = n.PlayerId,
                        Name = n.Name,
                        IsInBattle = _battleRepository.FindActiveFor(n.PlayerId) != null,
                        IsDead = n.IsDead()
                    }).ToList()
            };

            PassZoneStatus passZoneStatus = null;
            if (player.IsMoving())
            {
                passZoneStatus = new PassZoneStatus
                {
                    Me = new PassPlayerMessage
                    {
                        PlayerId = player.PlayerId,
                        Name = player.Name,
                        Direction = player.MovingInformation.Direction,
                        ProgressPercentage = player.MovingInformation.CalculateProgressPercentage()
                    },
                    Neighbors = onlineNeighbors.Select(n => new PassPlayerMessage
                    {
                        PlayerId = n.PlayerId,
                        Name = n.Name,
                        Direction = n.MovingInformation.Direction,
                        ProgressPercentage = n.MovingInformation.CalculateProgressPercentage()
                    }).ToList()
                };
            }

            var battle = _battleRepository.FindActiveFor(playerId);
            var battleStatus = battle?.GetBattleStatus(playerId);

            return new Status
            {
                PlayerStatus = playerStatus,
                ZoneStatus = zoneStatus,
                PassZoneStatus = passZoneStatus,
                BattleStatus = battleStatus
            };
        }
    }
}
