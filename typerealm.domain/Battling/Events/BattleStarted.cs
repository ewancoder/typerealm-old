namespace TypeRealm.Domain.Battling.Events
{
    public sealed class BattleStarted
    {
        public BattleStarted(int battleId)
        {
            BattleId = battleId;
        }

        public int BattleId { get; }
    }
}
