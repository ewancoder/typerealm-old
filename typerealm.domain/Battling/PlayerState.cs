using System.Collections.Generic;

namespace TypeRealm.Domain.Battling
{
    public sealed class PlayerState
    {
        public PlayerState(
            string playerId,
            WeaponDamage weaponDamage,
            Ability cumulativeAbility,
            PlayerHp playerHp,
            IEnumerable<int> skillIds,
            bool votedToEndBattle)
        {
            PlayerId = playerId;
            WeaponDamage = weaponDamage;
            CumulativeAbility = cumulativeAbility;
            PlayerHp = playerHp;
            SkillIds = skillIds;
            VotedToEndBattle = votedToEndBattle;
        }

        public string PlayerId { get; }
        public WeaponDamage WeaponDamage { get; }
        public Ability CumulativeAbility { get; }
        public PlayerHp PlayerHp { get; }
        public IEnumerable<int> SkillIds { get; }
        public bool VotedToEndBattle { get; }
    }
}
