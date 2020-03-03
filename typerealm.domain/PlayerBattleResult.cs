namespace TypeRealm.Domain
{
    public sealed class PlayerBattleResult
    {
        public PlayerBattleResult(string playerId, int hp)
        {
            PlayerId = playerId;
            Hp = hp;
        }

        public string PlayerId { get; }
        public int Hp { get; }
    }
}
