namespace TypeRealm.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TypeRealm.Messages;

    public sealed class Battle
    {
        private readonly Dictionary<string, BattlePlayer> _players;
        private readonly IEnumerable<string> _activeClients; // Should be linked to a changing data source.

        public Battle(IEnumerable<Player> players, IEnumerable<string> activeClients) // TODO: Pass here DTOs.
        {
            _players = players.Select(p => new BattlePlayer(p.PlayerId, p.Name, p.Hp, p.MaxHp, p.SkillIds, p.WeaponId)).ToDictionary(p => p.PlayerId, p => p);
            _activeClients = activeClients;
        }

        public bool IsFinished { get; private set; }

        public IEnumerable<PlayerBattleResult> GetBattleResults()
        {
            if (!IsFinished)
                throw new InvalidOperationException("Can't get battle results before the battle is over.");

            foreach (var player in _players.Values)
            {
                yield return new PlayerBattleResult(player.PlayerId, player.Hp);
            }
        }

        public void Attack(string initiatorId, string targetId, int skillId)
        {
            if (IsFinished)
                return;

            var initiator = _players[initiatorId];
            var target = _players[targetId];

            initiator.Attack(target, skillId);
        }

        public void StopBattle(string playerId)
        {
            _players[playerId].VoteToStop();

            if (_players.Values
                .Where(p => _activeClients.Contains(p.PlayerId))
                .Where(p => p.Hp > 0) // TODO: Make IsDead() method maybe.
                .All(p => p.IsVotedToStop))
            {
                foreach (var player in _players.Values)
                {
                    player.VoteToStop(); // Disconnected clients vote automatically.
                }

                IsFinished = true;
            }
        }

        public bool HasPlayer(string playerId)
        {
            return _players.ContainsKey(playerId);
        }

        public BattleStatus GetBattleStatus(string playerId)
        {
            return new BattleStatus
            {
                SkillIds = _players[playerId].GetSkills().ToList(),
                BattleNeighbors = _players.Values.Where(p => p.PlayerId != playerId).Select(p => p.GetStatus()).ToList(),
                Player = _players[playerId].GetStatus()
            };
        }
    }
}
