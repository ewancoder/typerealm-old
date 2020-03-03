namespace TypeRealm.Domain
{
    using System;
    using TypeRealm.Messages;

    public sealed class MovingInformation
    {
        private readonly Data.PassZone _passZone;
        private int _passedDistance;

        public MovingInformation(int passZoneId, PassDirection direction, int progress)
        {
            PassZoneId = passZoneId;
            Direction = direction;
            Progress = progress;

            _passZone = Data.Data.GetPassZone(passZoneId);
        }

        public int PassZoneId { get; }
        public PassDirection Direction { get; private set; }
        public int Progress { get; private set; }

        public int FromZoneId => _passZone.GetFromZoneId(Direction);
        public int ToZoneId => _passZone.GetToZoneId(Direction);
        public int Distance => _passZone.GetDistance(Direction);
        public int ReverseDistance => _passZone.GetReverseDistance(Direction);
        private Data.Passage Passage => _passZone.GetPassage(Direction);

        // Hacky impl. of healing.
        public int GainHp { get; private set; }
        public void ClearGainHp()
        {
            GainHp = 0;
        }

        public bool DidNotMoveYet()
        {
            return Progress == 0;
        }

        public int CalculateProgressPercentage()
        {
            return (int)Math.Floor(Progress * 100d / Distance);
        }

        public void ProgressFor(int progress)
        {
            if (progress < 0)
                throw new InvalidOperationException("Can't progress backward.");

            if (Progress + progress > Distance)
                throw new InvalidOperationException("Can't progress for more than actual distance.");

            Progress += progress;
            _passedDistance += progress;

            while (Passage.HealingFactor.IsValid && _passedDistance >= Passage.HealingFactor.Steps)
            {
                _passedDistance -= Passage.HealingFactor.Steps;
                GainHp += Passage.HealingFactor.Hp;
            }
        }

        public void Reverse()
        {
            var distance = Distance;
            var reverseDistance = ReverseDistance;

            var movedPercentage = Progress * 100d / distance;
            var reverseMovedPercentage = 100 - movedPercentage;

            var movedBackwards = (int)Math.Floor(reverseDistance * reverseMovedPercentage / 100);

            Direction = Direction == PassDirection.Forward ? PassDirection.Backward : PassDirection.Forward;
            Progress = movedBackwards;

            _passedDistance = 0;
        }

        public PassZoneMessage GetPassZone()
        {
            return new PassZoneMessage
            {
                PassZoneId = PassZoneId,
                Direction = Direction,
                Progress = Progress
            };
        }
    }
}
