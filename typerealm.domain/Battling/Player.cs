using System;
using System.Collections.Generic;

namespace TypeRealm.Domain.Battling
{
    using TypeRealm.Domain.Battling.Skills;

    /// <summary>
    /// Entity.
    /// </summary>
    public sealed class Player
    {
        private readonly WeaponDamage _weaponDamage;
        private readonly Ability _cumulativeAbility;
        private readonly PlayerHp _playerHp;
        private readonly HashSet<int> _skillIds;

        internal WeaponDamage WeaponDamage => _weaponDamage;
        internal Ability CumulativeAbility => _cumulativeAbility;

        private bool _votedToEndBattle;

        internal Player(PlayerState state)
        {
            _weaponDamage = state.WeaponDamage;
            _cumulativeAbility = state.CumulativeAbility;
            _playerHp = state.PlayerHp;
            _skillIds = new HashSet<int>(state.SkillIds);
            PlayerId = state.PlayerId;
            _votedToEndBattle = state.VotedToEndBattle;
        }

        public Player(
            string playerId,
            WeaponDamage weaponDamage,
            Ability cumulativeAbility,
            PlayerHp playerHp,
            IEnumerable<int> skillIds)
        {
            PlayerId = playerId;
            _weaponDamage = weaponDamage;
            _cumulativeAbility = cumulativeAbility;
            _playerHp = playerHp;
            _skillIds = new HashSet<int>(skillIds);

            // Hack: just in case somebody holds reference to mutable types
            // after creating Player instance.
            _playerHp = new PlayerHp(playerHp.Current, playerHp.Maximum);

            // Methods should be internal so only Battle class can access them.
        }

        public string PlayerId { get; }
        public bool ReadyToEndBattle => IsDead() || _votedToEndBattle;

        internal PlayerState GetState()
        {
            return new PlayerState(
                PlayerId,
                _weaponDamage,
                _cumulativeAbility,
                // TODO: Make PlayerHp immutable struct (maybe).
                new PlayerHp(_playerHp.Current, _playerHp.Maximum), // Don't pass mutable objects.
                _skillIds,
                _votedToEndBattle);
        }

        internal BattlePlayerResult GetBattleResult()
        {
            if (!ReadyToEndBattle)
                throw new InvalidOperationException("Player should be in ReadyToEndBattle state.");

            return new BattlePlayerResult(PlayerId, _playerHp.Current);
        }

        internal void Damage(int hp)
        {
            _playerHp.Current -= hp;
        }

        internal void Heal(int hp)
        {
            _playerHp.Current += hp;
        }

        internal void VoteToEndBattle()
        {
            _votedToEndBattle = true;
        }

        internal void Cast(Skill skill, IEnumerable<Player> targets)
        {
            if (CanCast(skill))
                skill.Cast(this, targets);

            _votedToEndBattle = false;
        }

        private bool CanCast(Skill skill)
        {
            // TODO: Notify that skill cannot be casted.

            // Player doesn't have this skill.
            if (!HasSkill(skill))
                return false;

            // Can't cast skills if dead.
            if (IsDead())
                return false;

            return true;
        }

        private bool HasSkill(Skill skill)
        {
            return _skillIds.Contains(skill.SkillId);
        }

        private bool IsDead()
        {
            return _playerHp.Current == 0;
        }
    }
}
