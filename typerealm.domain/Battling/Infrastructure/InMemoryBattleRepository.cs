using System.Collections.Generic;

namespace TypeRealm.Domain.Battling.Infrastructure
{
    using System.Linq;

    namespace Infrastructure
    {
        public sealed class InMemoryBattleRepository : IBattleRepository
        {
            private int _lastInsertedIndex;
            private readonly IPlayerActivity _playerActivity;

            private readonly Dictionary<int, BattleState> _allBattles
                = new Dictionary<int, BattleState>();

            private readonly Dictionary<string, int> _activeBattles
                = new Dictionary<string, int>();

            public InMemoryBattleRepository(IPlayerActivity playerActivity)
            {
                _playerActivity = playerActivity;
            }

            public Battle FindActiveFor(string playerId)
            {
                if (!_activeBattles.ContainsKey(playerId))
                    return null;

                var battleId = _activeBattles[playerId];

                return new Battle(_allBattles[battleId], _playerActivity);
            }

            public int GetNextId()
            {
                return _lastInsertedIndex++;
            }

            public void Save(Battle battle)
            {
                var battleState = battle.GetState();
                var playerStates = battleState.PlayerStates;

                if (_allBattles.ContainsKey(battle.BattleId))
                    _allBattles.Remove(battle.BattleId);

                _allBattles.Add(battle.BattleId, battleState);

                if (battleState.HasEnded)
                {
                    foreach (var playerState in playerStates)
                    {
                        if (_activeBattles.ContainsKey(playerState.PlayerId)
                            && _allBattles[_activeBattles[playerState.PlayerId]].BattleId == battleState.BattleId)
                        {
                            _activeBattles.Remove(playerState.PlayerId);
                        }
                    }

                    return;
                }

                foreach (var playerState in playerStates)
                {
                    if (!_activeBattles.ContainsKey(playerState.PlayerId))
                    {
                        _activeBattles.Add(playerState.PlayerId, battle.BattleId);
                    }
                }
            }

            public Battle Find(int battleId)
            {
                return new Battle(_allBattles.Values.FirstOrDefault(b => b.BattleId == battleId), _playerActivity);
            }
        }
    }
}
