namespace TypeRealm.Domain
{
    using System.Collections.Generic;
    using System.Linq;

    using Battling;

    public sealed class PlayerActivity : IPlayerActivity
    {
        private readonly IEnumerable<string> _activeClientsSource;

        public PlayerActivity(IEnumerable<string> activeClientsSource)
        {
            _activeClientsSource = activeClientsSource;
        }

        public bool IsActive(string playerId)
        {
            return _activeClientsSource.Contains(playerId);
        }
    }
}
