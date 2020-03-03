namespace TypeRealm.Domain.Battling.Events
{
    public sealed class PlayerVotedToEndBattle
    {
        public PlayerVotedToEndBattle(int battleId, string playerId)
        {
            BattleId = battleId;
            PlayerId = playerId;
        }

        public int BattleId { get; }
        public string PlayerId { get; }
    }
}
