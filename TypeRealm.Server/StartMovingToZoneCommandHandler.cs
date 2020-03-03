/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using TypeRealm.Messages;
    using TypeRealm.Server.Messaging;

    internal sealed class StartMovingToZoneCommandHandler : MessageHandler<StartMovingToZoneCommand>
    {
        private readonly IPlayerRepository _playerRepository;

        public StartMovingToZoneCommandHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public override void Handle(ConnectedClient sender, StartMovingToZoneCommand message)
        {
            var player = _playerRepository.Find(sender.PlayerId);

            player.StartMovingTo(message.PassZoneId);
            _playerRepository.Save(player);
        }
    }
}
