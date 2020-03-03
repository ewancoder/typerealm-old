namespace TypeRealm.Domain.Exploring
{
    public interface IPlayerRepository
    {
        string GetNextId();
        Player Find(string playerId);
        void Save(Player player);
    }
}
