namespace TypeRealm.Domain.Battling.Skills
{
    using System.Collections.Generic;

    /// <summary>
    /// Aggregate root.
    /// </summary>
    // Not passive ability. Only something that is CASTABLE.
    public abstract class Skill
    {
        protected Skill(int skillId)
        {
            SkillId = skillId;
        }

        public int SkillId { get; }

        /// <summary>
        /// Should be called only by Player class.
        /// </summary>
        internal abstract void Cast(Player caster, IEnumerable<Player> targets);
    }
}
