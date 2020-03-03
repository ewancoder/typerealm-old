namespace TypeRealm.Messages
{
    using ProtoBuf;
    using System.Collections.Generic;

    [ProtoContract]
    public sealed class Authorize
    {
        [ProtoMember(1)]
        public string PlayerId { get; set; }
    }

    [ProtoContract]
    public sealed class StartMovingToZoneCommand
    {
        [ProtoMember(1)]
        public int PassZoneId { get; set; }
    }

    [ProtoContract]
    public sealed class FinishWalkingCommand
    {
    }

    [ProtoContract]
    public sealed class ArriveToZoneCommand
    {
    }

    [ProtoContract]
    public sealed class MoveForCommand
    {
        [ProtoMember(1)]
        public int Progress { get; set; }
    }

    [ProtoContract]
    public sealed class TurnAroundCommand
    {
    }

    [ProtoContract]
    public sealed class AttackCommand
    {
        [ProtoMember(1)]
        public int SkillId { get; set; }

        [ProtoMember(2)]
        public string TargetId { get; set; }
    }

    [ProtoContract]
    public sealed class StopBattleCommand
    {
    }

    public enum NotificationSeverity
    {
        Notification = 1,
        Alert = 2
    }

    [ProtoContract]
    public sealed class Notification
    {
        [ProtoMember(1)]
        public string Text { get; set; }

        [ProtoMember(2)]
        public NotificationSeverity Severity { get; set; }
    }

    [ProtoContract]
    public sealed class StartBattleCommand
    {
        [ProtoMember(1)]
        public List<string> ParticipantsExceptMeIds { get; set; } = new List<string>();
    }

    /*public enum State
    {
        InZone = 1,
        InBattle = 2
    }*/

    [ProtoContract]
    public sealed class Status
    {
        [ProtoMember(1)]
        public PlayerStatus PlayerStatus { get; set; }

        [ProtoMember(2)]
        public ZoneStatus ZoneStatus { get; set; }

        [ProtoMember(3)]
        public PassZoneStatus PassZoneStatus { get; set; }

        [ProtoMember(4)]
        public BattleStatus BattleStatus { get; set; }
    }

    [ProtoContract]
    public sealed class ZoneStatus
    {
        [ProtoMember(1)]
        public List<PlayerMessage> Neighbors { get; set; } = new List<PlayerMessage>();
    }

    [ProtoContract]
    public sealed class PassZoneStatus
    {
        [ProtoMember(1)]
        public PassPlayerMessage Me { get; set; }

        [ProtoMember(2)]
        public List<PassPlayerMessage> Neighbors { get; set; } = new List<PassPlayerMessage>();
    }










    // just to test persistence.
    [ProtoContract]
    public sealed class PlayerAggregateStorage
    {
        [ProtoMember(1)]
        public List<PlayerStatus> Players { get; set; } = new List<PlayerStatus>();
    }



















    [ProtoContract]
    public sealed class BattleStatus
    {
        [ProtoMember(1)]
        public List<BattleParticipantMessage> BattleNeighbors { get; set; } = new List<BattleParticipantMessage>();

        [ProtoMember(2)]
        public List<int> SkillIds { get; set; } = new List<int>();

        [ProtoMember(3)]
        public BattleParticipantMessage Player { get; set; }
    }

    [ProtoContract]
    public sealed class PlayerStatus
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public int MaxHp { get; set; }

        [ProtoMember(4)]
        public int Hp { get; set; }

        //[ProtoMember(5)]
        //public Battle Battle { get; set; }

        [ProtoMember(6)]
        public List<SkillMessage> Skills { get; set; } = new List<SkillMessage>();

        [ProtoMember(7)]
        public ZoneMessage Zone { get; set; }

        [ProtoMember(8)]
        public PassZoneMessage PassZone { get; set; }

        [ProtoMember(9)]
        public WeaponMessage Weapon { get; set; }

        // hack: needed for persistence only.
        [ProtoMember(10)]
        public string PlayerId { get; set; }
    }

    [ProtoContract]
    public sealed class WeaponMessage
    {
        [ProtoMember(1)]
        public int WeaponId { get; set; }
    }

    [ProtoContract]
    public sealed class SkillMessage
    {
        [ProtoMember(1)]
        public int SkillId { get; set; }
    }

    [ProtoContract]
    public sealed class BattleMessage
    {
        [ProtoMember(1)]
        public List<BattleTargetMessage> BattleTargets { get; set; } = new List<BattleTargetMessage>();
    }

    [ProtoContract]
    public sealed class BattleTargetMessage
    {
        [ProtoMember(1)]
        public string PlayerId { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public int MaxHp { get; set; }

        [ProtoMember(4)]
        public int Hp { get; set; }
    }























    [ProtoContract]
    public sealed class ZoneMessage
    {
        [ProtoMember(1)]
        public int ZoneId { get; set; }

        //[ProtoMember(2)]
        //public List<Player> Neighbors { get; set; } = new List<Player>();
    }

    [ProtoContract]
    public sealed class PassZoneMessage
    {
        [ProtoMember(1)]
        public int PassZoneId { get; set; }

        [ProtoMember(2)]
        public PassDirection Direction { get; set; }

        [ProtoMember(3)]
        public int Progress { get; set; }

        // TODO: Add NEIGHBORS, but include DIRECTION + PROGRESS for each neighbor. To show them on the road.
        // (racing concept - running naperegonki)
    }

    public enum PassDirection
    {
        Forward = 1,
        Backward = 2
    }

    [ProtoContract]
    public sealed class PlayerMessage
    {
        [ProtoMember(1)]
        public string PlayerId { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public bool IsInBattle { get; set; }

        [ProtoMember(4)]
        public bool IsDead { get; set; }
    }

    [ProtoContract]
    public sealed class BattleParticipantMessage
    {
        [ProtoMember(1)]
        public string PlayerId { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public int Hp { get; set; }

        [ProtoMember(4)]
        public int MaxHp { get; set; }

        [ProtoMember(5)]
        public bool IsVotedToStop { get; set; }
    }

    [ProtoContract]
    public sealed class PassPlayerMessage
    {
        [ProtoMember(1)]
        public string PlayerId { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public PassDirection Direction { get; set; }

        [ProtoMember(4)]
        public int ProgressPercentage { get; set; }
    }
}
