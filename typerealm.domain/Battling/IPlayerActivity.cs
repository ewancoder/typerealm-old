namespace TypeRealm.Domain.Battling
{
    // If player went offline or doesn't respond for long time, it becomes inactive.
    public interface IPlayerActivity
    {
        bool IsActive(string playerId);
    }
}
