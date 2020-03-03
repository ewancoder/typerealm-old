namespace TypeRealm.Messages
{
    using System;

    // Move to some common assembly for all the other assemblies.
    public static class Randomizer
    {
        private static readonly Random _global = new Random();
        [ThreadStatic] private static Random _local;

        private static Random Local
        {
            get
            {
                if (_local == null)
                {
                    lock (_global)
                    {
                        if (_local == null)
                        {
                            var seed = _global.Next();
                            _local = new Random(seed);
                        }
                    }
                }

                return _local;
            }
        }

        public static int Next()
        {
            return Local.Next();
        }

        public static int Next(int min, int max)
        {
            return Local.Next(min, max);
        }
    }
}
