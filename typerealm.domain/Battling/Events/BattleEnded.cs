namespace TypeRealm.Domain.Battling.Events
{
    public sealed class BattleEnded
    {
        public BattleEnded(int battleId)
        {
            BattleId = battleId;
        }

        public int BattleId { get; }
    }
}
