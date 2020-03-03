namespace TypeRealm.Domain.Battling
{
    public struct Percentage
    {
        private readonly int _value;

        public Percentage(int value)
        {
            _value = value;
        }

        public static Percentage Zero => new Percentage();

        public static implicit operator Percentage(int value)
            => new Percentage(value);

        public static implicit operator int(Percentage percentage)
            => percentage._value;
    }
}
