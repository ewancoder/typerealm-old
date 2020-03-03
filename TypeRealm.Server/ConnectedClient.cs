/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using System.IO;

    internal sealed class ConnectedClient
    {
        public ConnectedClient(string playerId, Stream stream)
        {
            PlayerId = playerId;
            Stream = stream;
        }

        public string PlayerId { get; }
        public Stream Stream { get; }
    }
}
