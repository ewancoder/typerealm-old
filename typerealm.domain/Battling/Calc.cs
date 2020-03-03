namespace TypeRealm.Domain.Battling
{
    using System;

    public static class Calc
    {
        public static int Round(double value)
        {
            return (int)Math.Floor(value);
        }
    }
}
