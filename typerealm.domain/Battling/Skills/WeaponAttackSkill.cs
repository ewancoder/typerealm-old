namespace TypeRealm.Domain.Battling.Skills
{
    using System.Collections.Generic;

    public sealed class WeaponAttackSkill : Skill
    {
        public WeaponAttackSkill(int skillId) : base(skillId)
        {
        }

        internal override void Cast(Player caster, IEnumerable<Player> targets)
        {
            var damage = Calc.Round(
                caster.WeaponDamage.Value.Random()
                * caster.CumulativeAbility.DamageModifier / 100d);

            foreach (var target in targets)
            {
                target.Damage(damage);
            }
        }
    }
}
