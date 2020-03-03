namespace TypeRealm.Domain.Battling
{
    public sealed class Ability
    {
        public Ability(Percentage damageModifier)
        {
            DamageModifier = damageModifier;
        }

        public Percentage DamageModifier { get; }
    }
}
