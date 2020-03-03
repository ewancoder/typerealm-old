namespace TypeRealm.Domain.Battling
{
    public interface IBattleRepository
    {
        int GetNextId();
        Battle Find(int battleId);
        void Save(Battle battle);

        Battle FindActiveFor(string playerId);
    }
}
