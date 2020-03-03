/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using TypeRealm.Domain;

    internal sealed class BattleRepository
    {
        // TODO: Maybe also use dictionary with playerId as key.
        private readonly HashSet<Battle> _battles = new HashSet<Battle>();

        private IEnumerable<Battle> ActiveBattles => _battles.Where(b => !b.IsFinished);

        public Battle FindActiveFor(string playerId)
        {
            return ActiveBattles.SingleOrDefault(b => b.HasPlayer(playerId));
        }

        public void StartNew(Battle battle)
        {
            // TODO: Check that player is unique for all battles.

            _battles.Add(battle);
        }

        public bool IsInBattle(string playerId)
        {
            return ActiveBattles.Any(b => b.HasPlayer(playerId));
        }
    }
}
