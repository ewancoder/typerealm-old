/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using TypeRealm.Messages;
    using TypeRealm.Server.Messaging;

    internal sealed class TurnAroundCommandHandler : MessageHandler<TurnAroundCommand>
    {
        private readonly IPlayerRepository _playerRepository;

        public TurnAroundCommandHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public override void Handle(ConnectedClient sender, TurnAroundCommand message)
        {
            var player = _playerRepository.Find(sender.PlayerId);

            player.TurnAround();

            _playerRepository.Save(player);
        }
    }
}
