namespace TypeRealm.Domain.Battling.Skills
{
    using System.Collections.Generic;

    public sealed class HealSkill : Skill
    {
        private readonly PositiveValueRange _healingPower;

        public HealSkill(int skillId, PositiveValueRange healingPower) : base(skillId)
        {
            _healingPower = healingPower;
        }

        internal override void Cast(Player caster, IEnumerable<Player> targets)
        {
            var healingPower = Calc.Round(
                _healingPower.Random() /* caster.CumulativeAbility.HealingModifier / 100d*/);

            foreach (var target in targets)
            {
                target.Heal(healingPower);
            }
        }
    }
}
