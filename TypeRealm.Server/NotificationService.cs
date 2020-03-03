/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using Messages;

    internal sealed class NotificationService
    {
        public void Notify(ConnectedClient client, Notification notification)
        {
            MessageSerializer.Serialize(client.Stream, notification);
        }
    }
}
