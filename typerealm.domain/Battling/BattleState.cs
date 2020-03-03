using System.Collections.Generic;

namespace TypeRealm.Domain.Battling
{
    public sealed class BattleState
    {
        public BattleState(
            int battleId,
            bool hasEnded,
            IEnumerable<PlayerState> playerStates)
        {
            BattleId = battleId;
            HasEnded = hasEnded;
            PlayerStates = playerStates;
        }

        public int BattleId { get; }
        public bool HasEnded { get; }
        public IEnumerable<PlayerState> PlayerStates { get; }
    }
}
