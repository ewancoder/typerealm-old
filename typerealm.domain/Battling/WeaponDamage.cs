namespace TypeRealm.Domain.Battling
{
    /// <summary>
    /// Value object.
    /// Contains all the data about current wielded weapon(s).
    /// </summary>
    public sealed class WeaponDamage
    {
        public WeaponDamage(TypeRealm.Domain.Battling.PositiveValueRange value)
        {
            Value = value;
        }

        public TypeRealm.Domain.Battling.PositiveValueRange Value { get; }
    }
}
