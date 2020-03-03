namespace TypeRealm.Domain.Battling.Skills
{
    using System;
    using System.Collections.Generic;

    public interface ISkillRepository
    {
        Skill Find(int skillId);
    }

    namespace Infrastructure
    {
        public enum SkillType
        {
            WeaponAttack = 1,
            Heal = 2
        }

        public sealed class SkillDao
        {
            public int SkillId { get; set; }
            public SkillType Type { get; set; }
        }

        public sealed class HealSkillDao
        {
            public int SkillId { get; set; }
            public PositiveValueRange HealingPower { get; set; }
        }

        public sealed class InMemorySkillRepository : ISkillRepository
        {
            private readonly Dictionary<int, SkillDao> _skills
                = new Dictionary<int, SkillDao>();

            private readonly Dictionary<int, HealSkillDao> _healSkills
                = new Dictionary<int, HealSkillDao>();

            public InMemorySkillRepository()
            {
                PopulateData();
            }

            public Skill Find(int skillId)
            {
                if (!_skills.ContainsKey(skillId))
                    throw new InvalidOperationException($"Skill {skillId} does not exist.");

                var skill = _skills[skillId];

                switch (skill.Type)
                {
                    case SkillType.WeaponAttack:
                        return new WeaponAttackSkill(skillId);
                    case SkillType.Heal:
                        return MakeHealSkill(skillId);
                    default:
                        throw new InvalidOperationException("Invalid skill type.");
                }
            }

            private HealSkill MakeHealSkill(int skillId)
            {
                if (!_healSkills.ContainsKey(skillId))
                    throw new InvalidOperationException($"Invalid state of skill {skillId}: it's not a heal skill.");

                var healSkill = _healSkills[skillId];

                return new HealSkill(skillId, healSkill.HealingPower);
            }

            private void PopulateData()
            {
                _skills.Add(1, new SkillDao
                {
                    SkillId = 1,
                    Type = SkillType.WeaponAttack
                });

                _skills.Add(2, new SkillDao
                {
                    SkillId = 2,
                    Type = SkillType.Heal
                });

                _healSkills.Add(2, new HealSkillDao
                {
                    SkillId = 2,
                    HealingPower = new PositiveValueRange(5, 10)
                });

                _skills.Add(3, new SkillDao
                {
                    SkillId = 3,
                    Type = SkillType.Heal
                });

                _healSkills.Add(3, new HealSkillDao
                {
                    SkillId = 3,
                    HealingPower = new PositiveValueRange(20, 30)
                });
            }
        }
    }
}
