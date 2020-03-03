/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using TypeRealm.Messages;
    using TypeRealm.Server.Messaging;

    internal sealed class FinishWalkingCommandHandler : MessageHandler<FinishWalkingCommand>
    {
        private readonly IPlayerRepository _playerRepository;

        public FinishWalkingCommandHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public override void Handle(ConnectedClient sender, FinishWalkingCommand message)
        {
            var player = _playerRepository.Find(sender.PlayerId);
            player.ArriveTo();
            _playerRepository.Save(player);
        }
    }
}
