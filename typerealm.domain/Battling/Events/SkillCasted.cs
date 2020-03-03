using System.Collections.Generic;

namespace TypeRealm.Domain.Battling.Events
{
    public sealed class SkillCasted
    {
        public SkillCasted(int battleId, string casterId, int skillId, IEnumerable<string> targetIds)
        {
            BattleId = battleId;
            CasterId = casterId;
            SkillId = skillId;
            TargetIds = targetIds;
        }

        public int BattleId { get; }
        public string CasterId { get; }
        public int SkillId { get; }
        public IEnumerable<string> TargetIds { get; }
    }
}
