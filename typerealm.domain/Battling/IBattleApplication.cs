using System.Collections.Generic;

namespace TypeRealm.Domain.Battling
{
    public interface IBattleApplication
    {
        int StartBattle(IEnumerable<Player> participants);
        void Cast(string casterId, int skillId, IEnumerable<string> targetIds);
        void VoteToEndBattle(string voterId);

        // hacks to make it work.
        BattleState FindActiveBattleStateFor(string playerId);
    }
}
