namespace TypeRealm.Domain.Battling
{
    using System;

    public struct PositiveValueRange
    {
        public PositiveValueRange(int min, int max)
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

        public int Random()
        {
            if (Max == 0)
                return 0;

            return Messages.Randomizer.Next(Min, Max);
        }

        public static PositiveValueRange Zero => new PositiveValueRange(0, 0);
    }
}
