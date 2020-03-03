namespace TypeRealm.Domain.Battling
{
    public sealed class BattlePlayerResult
    {
        public BattlePlayerResult(string playerId, int hp)
        {
            PlayerId = playerId;
            Hp = hp;
        }

        public string PlayerId { get; }
        public int Hp { get; }
    }
}
