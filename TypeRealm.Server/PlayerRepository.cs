/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using TypeRealm.Domain;
    using TypeRealm.Messages;

    internal sealed class PlayerRepository
    {
        private readonly Dictionary<string, Player> _players = new Dictionary<string, Player>();

        public Player Find(string playerId)
        {
            if (!_players.ContainsKey(playerId))
                return null;

            return _players[playerId];
        }

        public void Create(Player player)
        {
            _players.Add(player.PlayerId, player);
        }

        internal IEnumerable<Player> FindNeighborsOf(Player player)
        {
            return _players.Values
                .Where(p => p.IsInSameZone(player))
                .Where(p => p.PlayerId != player.PlayerId);
        }

       }

        internal interface IPlayerRepository
        {
            Player Find(string playerId);
            void Save(Player player);
            IEnumerable<Player> FindNeighborsOf(Player player);
        }

    internal sealed class DiskPlayerRepository : IPlayerRepository
    {
        private readonly string _playersFileName;

        public DiskPlayerRepository(string playersFileName)
        {
            _playersFileName = playersFileName;
        }

        public Player Find(string playerId)
        {
            using (var stream = File.Open(_playersFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (stream.Length == 0)
                    return null;

                var storage = Serializer.Deserialize<PlayerAggregateStorage>(stream);
                var status = storage.Players.SingleOrDefault(p => p.PlayerId == playerId);

                if (status == null)
                    return null;

                return Player.FromStatus(status);
            }
        }

        public IEnumerable<Player> FindNeighborsOf(Player player)
        {
            throw new NotImplementedException();
        }



        public void Save(Player player)
        {
            var storage = new PlayerAggregateStorage();

            using (var stream = File.Open(_playersFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (stream.Length > 0)
                    storage = Serializer.Deserialize<PlayerAggregateStorage>(stream);
            }

            var existingPlayer = storage.Players.SingleOrDefault(p => p.PlayerId == player.PlayerId);
            if (existingPlayer != null)
                storage.Players.Remove(existingPlayer);

            var status = player.GetPlayerStatus();
            storage.Players.Add(status);

            using (var stream = File.Open(_playersFileName, FileMode.Create, FileAccess.Write))
            {
                // TODO: Remove System.XML reference when this will be moved to another assembly / sql implementation.
                Serializer.Serialize(stream, storage);
            }
        }
    }

    internal sealed class CachedPlayerRepository : IPlayerRepository
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly HashSet<string> _nonExistentPlayers
            = new HashSet<string>();
        private readonly HashSet<Player> _savePending
            = new HashSet<Player>();
        private readonly Dictionary<string, Player> _players
            = new Dictionary<string, Player>();

        public CachedPlayerRepository(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public Player Find(string playerId)
        {
            if (_players.ContainsKey(playerId))
                return _players[playerId];

            if (_nonExistentPlayers.Contains(playerId))
                return null;

            var player = _playerRepository.Find(playerId);
            if (player == null)
            {
                _nonExistentPlayers.Add(playerId);
                return null;
            }

            _players.Add(playerId, player);
            return player;
        }

        // TODO: Move this method somewhere. Remove it from interface. Make an extension or smth.
        public IEnumerable<Player> FindNeighborsOf(Player player)
        {
            return _players.Values
                .Where(p => p.IsInSameZone(player))
                .Where(p => p.PlayerId != player.PlayerId);
        }



        public void Save(Player player)
        {
            _savePending.Add(player);

            if (!_players.ContainsKey(player.PlayerId))
                _players.Add(player.PlayerId, player);

            if (_nonExistentPlayers.Contains(player.PlayerId))
                _nonExistentPlayers.Remove(player.PlayerId);
        }

        public void PersistAllPending()
        {

            foreach (var player in _savePending)
            {
                _playerRepository.Save(player);
            }

            _savePending.Clear();
        }
    }
}
