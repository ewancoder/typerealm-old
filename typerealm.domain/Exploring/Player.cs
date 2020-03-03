namespace TypeRealm.Domain.Exploring
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TypeRealm.Messages;

    public sealed class Player
    {
        private readonly List<int> _skillIds;

        private Player(PlayerStatus status)
        {
            _skillIds = status.Skills.Select(s => s.SkillId).ToList();
            PlayerId = status.PlayerId;
            Name = status.Name;
            ZoneId = status.Zone.ZoneId;
            _hp = status.Hp;
            MaxHp = status.MaxHp;
            WeaponId = status.Weapon.WeaponId;
            MovingInformation = status.PassZone == null ? null : new MovingInformation(status.PassZone.PassZoneId, status.PassZone.Direction, status.PassZone.Progress);
        }

        public Player(string playerId, string name, int zoneId, MovingInformation movingInformation)
        {
            PlayerId = playerId;
            Name = name;
            ZoneId = zoneId;
            MovingInformation = movingInformation;

            _hp = 100;
            MaxHp = 100;
            WeaponId = 1;
            _skillIds = new List<int> { 1, 2, 3 };
        }

        public static Player FromStatus(PlayerStatus status)
            => new Player(status);

        public string PlayerId { get; }
        public string Name { get; }
        private int ZoneId { get; set; }

        private int _hp;
        public int Hp
        {
            get => _hp;
            set
            {
                if (value < 0)
                {
                    _hp = 0;
                    return;
                }

                if (value > MaxHp)
                {
                    _hp = MaxHp;
                    return;
                }

                _hp = value;
            }
        }
        public int MaxHp { get; }
        public int WeaponId { get; }
        public IEnumerable<int> SkillIds => _skillIds;
        public MovingInformation MovingInformation { get; private set; }

        public bool IsMoving()
        {
            return MovingInformation != null;
        }

        public bool IsInSameZone(Player player)
        {
            return player.ZoneId == ZoneId && player.MovingInformation?.PassZoneId == MovingInformation?.PassZoneId;
        }

        public PlayerStatus GetPlayerStatus()
        {
            return new PlayerStatus
            {
                PlayerId = PlayerId,
                Zone = new ZoneMessage
                {
                    ZoneId = ZoneId
                },
                Name = Name,
                Hp = Hp,
                MaxHp = MaxHp,
                Weapon = new WeaponMessage
                {
                    WeaponId = WeaponId
                },
                Skills = SkillIds.Select(sid => new SkillMessage
                {
                    SkillId = sid
                }).ToList(),
                PassZone = MovingInformation == null ? null : MovingInformation.GetPassZone()
            };
        }

        public void UpdateHp(int hp)
        {
            if (hp < 0)
                throw new InvalidOperationException("HP can't be negative.");

            if (hp > MaxHp)
                throw new InvalidOperationException("HP can't be more than MAX HP.");

            Hp = hp;
        }

        public void StartMovingTo(int passZoneId)
        {
            // TODO: Return back, uncomment.
            if (MovingInformation != null)
                throw new InvalidOperationException($"Player {PlayerId} is already moving.");// in pass zone {MovingInformation.PassZoneId}.");

            var zone = TypeRealm.Data.Data.GetZone(ZoneId);
            var direction = zone.ZoneExits.Contains(passZoneId) ? PassDirection.Forward : PassDirection.Backward;
            if (direction == PassDirection.Backward && !zone.ZoneEntrances.Contains(passZoneId))
                throw new InvalidOperationException($"There is no passage to {passZoneId} pass zone from {ZoneId} zone.");

            MovingInformation = new MovingInformation(passZoneId, direction, 0);
        }

        public void ProgressFor(int progress)
        {
            if (MovingInformation == null)
                throw new InvalidOperationException($"Player {PlayerId} is not moving anywhere.");

            MovingInformation.ProgressFor(progress);

            if (MovingInformation.GainHp != 0)
            {
                Hp += MovingInformation.GainHp;
                MovingInformation.ClearGainHp();
            }
        }

        public void TurnAround()
        {
            if (MovingInformation == null)
                throw new InvalidOperationException($"Player {PlayerId} is not moving anywhere.");

            if (MovingInformation.DidNotMoveYet()) // Turned back without moving, decided not to go.
            {
                MovingInformation = null;
                return;
            }

            MovingInformation.Reverse();
        }

        public void ArriveTo()
        {
            if (MovingInformation == null)
                throw new InvalidOperationException($"Player {PlayerId} is not moving anywhere.");

            var targetZoneId = MovingInformation.ToZoneId;

            var distance = MovingInformation.Distance;
            if (MovingInformation.Progress != distance)
                throw new InvalidOperationException($"Player {PlayerId} did not reach the destination yet.");

            ZoneId = targetZoneId;
            MovingInformation = null;
        }

        public bool IsDead()
        {
            return Hp == 0;
        }
    }
}
