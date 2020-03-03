namespace TypeRealm.Domain
{
    using System;
    using System.Collections.Generic;
    using TypeRealm.Messages;

    public sealed class BattlePlayer
    {
        private readonly HashSet<int> _skillIds;
        private readonly string _name; // Hack to generate status more easily. Get the name from another context!! (what if player renames himself and re-sign-ins to battle?)
        private readonly int _weaponId;
        private int _hp;

        public BattlePlayer(string playerId, string name, int hp, int maxHp, IEnumerable<int> skillIds, int weaponId)
        {
            PlayerId = playerId;
            _name = name;
            MaxHp = maxHp;
            _hp = hp;
            _skillIds = new HashSet<int>(skillIds);
            _weaponId = weaponId;
        }

        public bool IsVotedToStop { get; private set; }

        public string PlayerId { get; }
        public int Hp
        {
            get => _hp;
            private set
            {
                if (value > MaxHp)
                {
                    _hp = value;
                    return;
                }

                if (value < 0)
                {
                    _hp = 0;
                    return;
                }

                _hp = value;
            }
        }
        private int MaxHp { get; }

        private bool IsDead()
        {
            return Hp == 0;
        }

        public void VoteToStop()
        {
            IsVotedToStop = true;
        }

        public void Attack(BattlePlayer target, int skillId)
        {
            if (!_skillIds.Contains(skillId))
                throw new InvalidOperationException($"Player {PlayerId} doesn't have skill {skillId}.");

            if (IsDead())
                return; // Can't attack anyone if dead.

            var skill = Data.Data.GetSkill(skillId);
            var weapon = Data.Data.GetWeapon(_weaponId);
            var damage = skill.GetDamage(weapon);

            // TODO: Create a notification that this skill is useless.
            if (damage.Min == 0 && damage.Max == 0)
                return; // This skill doesn't work with this type of weapon.

            target.Hp -= (int)Math.Floor(Randomizer.Next(damage.Min, damage.Max) * weapon.DamageModifier);

            // If you continued attacking - you are not voting anymore.
            IsVotedToStop = false;
        }

        public BattleParticipantMessage GetStatus()
        {
            return new BattleParticipantMessage
            {
                PlayerId = PlayerId,
                Name = _name,
                Hp = Hp,
                MaxHp = MaxHp,
                IsVotedToStop = IsVotedToStop
            };
        }

        internal IEnumerable<int> GetSkills()
        {
            return _skillIds;
        }
    }
}
