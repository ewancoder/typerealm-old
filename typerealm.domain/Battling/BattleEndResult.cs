using System.Collections.Generic;

namespace TypeRealm.Domain.Battling
{
    public sealed class BattleEndResult
    {
        public BattleEndResult(IEnumerable<BattlePlayerResult> players)
        {
            Players = players;
        }

        public IEnumerable<BattlePlayerResult> Players { get; }
    }
}
