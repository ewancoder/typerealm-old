namespace TypeRealm.Domain.Battling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public enum WeaponTypeNg
    {
        Fists = 1,
        Sharp = 2,
        Blunt = 3,
        Magic = 4
    }

    public interface IWeaponNg
    {
        decimal DamageModifier { get; }
        WeaponTypeNg Type { get; }
    }

    public interface ISkillNg
    {
        void Cast(PlayerNg caster, IEnumerable<PlayerNg> targets);
    }

    public struct ValueRangeNg
    {
        public ValueRangeNg(int min, int max)
        {
            if (min < 0)
                throw new ArgumentException("Min should be positive or zero.", nameof(min));

            if (min > max)
                throw new ArgumentException("Max should be larger or equal to min.", nameof(max));

            Min = min;
            Max = max;
        }

        public int Min { get; }
        public int Max { get; }

        public int MakeRandom()
        {
            if (Max == 0)
                return 0;

            return Messages.Randomizer.Next(Min, Max);
        }
    }

    public sealed class SharpAttackSkillNgNg : ISkillNg
    {
        private readonly ValueRangeNg _damage;

        public SharpAttackSkillNgNg(ValueRangeNg damage)
        {
            _damage = damage;
        }

        public void Cast(PlayerNg caster, IEnumerable<PlayerNg> targets)
        {
            var damage = _damage.MakeRandom();

            switch (caster.Weapon.Type)
            {
                case WeaponTypeNg.Sharp:
                    break;
                case WeaponTypeNg.Blunt:
                    damage /= 2;
                    break;
                default:
                    damage = 0;
                    break;
            }

            foreach (var target in targets)
            {
                target.Damage(damage);
            }
        }
    }

    public sealed class PlayerNg
    {
        private readonly string _playerId;
        private readonly HashSet<ISkillNg> _skills;
        private readonly int _maxHp;
        private readonly IWeaponNg _weapon;
        private int _hp;
        private bool _votedToEnd;

        public PlayerNg(string playerId, int hp, int maxHp, HashSet<ISkillNg> skills, IWeaponNg weapon)
        {
            if (hp <= 0)
                throw new ArgumentException("Health should be positive.", nameof(hp));

            if (hp > maxHp)
                throw new ArgumentException("Health should not exceed maximum health.", nameof(maxHp));

            if (skills == null || skills.Count == 0)
                throw new ArgumentException("Can't participate in battle without skills.", nameof(skills));

            _playerId = playerId;
            _hp = hp;
            _maxHp = maxHp;
            _skills = skills;
            _weapon = weapon;
        }

        public string PlayerId => _playerId;
        public bool IsVotedForBattleToEnd => _hp == 0 || _votedToEnd;
        public IWeaponNg Weapon => _weapon;

        public void VoteToEnd()
        {
            _votedToEnd = true;
        }

        public void Cast(ISkillNg skill, IEnumerable<PlayerNg> targets)
        {
            if (!CanCast(skill))
                return;

            _votedToEnd = false;

            skill.Cast(this, targets);
        }

        public void Damage(int hp)
        {
            _hp -= hp;

            if (_hp < 0)
                _hp = 0;
        }

        public void Heal(int hp)
        {
            if (_hp == 0)
                return; // Cannot heal the dead.

            _hp += hp;

            if (_hp > _maxHp)
                _hp = _maxHp;
        }

        public void Resurrect(int hp)
        {
            if (_hp > 0)
                return; // Cannot resurrect if already alive.

            _hp += hp;

            if (_hp > _maxHp)
                _hp = _maxHp;
        }

        private bool CanCast(ISkillNg skill)
        {
            // Can't cast skills if dead.
            if (_hp == 0)
                return false;

            if (_skills.Contains(skill))
                return true;

            // Here can be any logic.
            // Can cast particular skills only with particular weapon.
            // Can't cast something when debuffed.
            // Can cast something that wasn't able to - when buffed.
            // Etc.

            return false;
        }
    }

    // If player went offline or doesn't respond for long time, it becomes inactive.
    public interface IPlayerActivity
    {
        bool IsActive(string playerId);
    }

    public sealed class BattleNg
    {
        private readonly IPlayerActivity _playerActivity;
        private readonly Dictionary<string, PlayerNg> _players;
        private bool _hasEnded;

        public BattleNg(IPlayerActivity playerActivity, Dictionary<string, PlayerNg> players)
        {
            if (players.Count == 0)
                throw new ArgumentException("Can't start a battle without participants.", nameof(players));

            _playerActivity = playerActivity;
            _players = players;
        }

        public void Cast(string casterId, ISkillNg skill, IEnumerable<string> targetsIds)
        {
            if (_hasEnded)
                return;

            if (_players.ContainsKey(casterId))
                throw new InvalidOperationException("Caster is not in battle.");

            var caster = _players[casterId];
            var targets = new HashSet<PlayerNg>();

            foreach (var targetId in targetsIds)
            {
                if (!_players.ContainsKey(targetId))
                    throw new InvalidOperationException("Target is not in battle.");

                targets.Add(_players[targetId]);
            }

            caster.Cast(skill, targets);
        }

        public void VoteToEnd(string voterId)
        {
            if (_hasEnded)
                return;

            _players[voterId].VoteToEnd();

            foreach (var player in _players.Values)
            {
                if (player.IsVotedForBattleToEnd)
                    continue; // Check next player.

                if (!_playerActivity.IsActive(player.PlayerId))
                    continue; // Player is offline. Consider that he voted.

                // Player is active and has not voted yet. Don't end battle.
                return;
            }

            _hasEnded = true;
        }
    }

    public interface IBattleRepositoryNg
    {
        BattleNg FindFor(string playerId);
    }

    // Takes data from ISkill(Data)Repository and creates a skill.
    // Should cache all skills in current implementation for comparsion inside Player class.
    public interface ISkillFactoryNg
    {
        ISkillNg Make(int skillId);
    }

    // Takes data from IWeapon(Data)Repository and creates a weapon.
    public interface IWeaponFactoryNg
    {
        IWeaponNg Make(int weaponId);
    }

    public sealed class BattleApplication
    {
        private readonly IBattleRepositoryNg _battleRepository;
        private readonly ISkillFactoryNg _skillFactory;

        public BattleApplication(
            IBattleRepositoryNg battleRepository,
            ISkillFactoryNg skillRepository)
        {
            _battleRepository = battleRepository;
            _skillFactory = skillRepository;
        }

        public void Cast(string casterId, int skillId, IEnumerable<string> targetsIds)
        {
            var battle = _battleRepository.FindFor(casterId);
            if (battle == null)
                throw new InvalidOperationException($"Battle not found for player {casterId}.");

            var skill = _skillFactory.Make(skillId);

            battle.Cast(casterId, skill, targetsIds);
        }
    }
}
