/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Messages;
    using Messaging;

    internal sealed class StartBattleCommandHandler : MessageHandler<StartBattleCommand>
    {
        private readonly IEnumerable<ConnectedClient> _activeClients; // Hack. find a way to do it without passing this here.
        private readonly IPlayerRepository _playerRepository;
        private readonly BattleRepository _battleRepository;
        private readonly NotificationService _notificationService = new NotificationService();

        public StartBattleCommandHandler(IEnumerable<ConnectedClient> activeClients, IPlayerRepository playerRepository, BattleRepository battleRepository)
        {
            _activeClients = activeClients;
            _playerRepository = playerRepository;
            _battleRepository = battleRepository;
        }

        public override void Handle(ConnectedClient sender, StartBattleCommand message)
        {
            // Maybe just leave the latest battle hanging there.
            //if (sender.BattleAggregate != null)
            //throw new InvalidOperationException("In-memory client already has battle aggregate set.");

            var players = message.ParticipantsExceptMeIds.Select(id => _playerRepository.Find(id)).Concat(new[] { _playerRepository.Find(sender.PlayerId) });
            var battle = new Battle(players, _activeClients.Select(c => c.PlayerId));

            foreach (var player in players)
            {
                if (_battleRepository.IsInBattle(player.PlayerId))
                {
                    _notificationService.Notify(sender, new Notification
                    {
                        Severity = NotificationSeverity.Alert,
                        Text = $"Player {player.Name} is already in another battle."
                    });

                    return;
                }

                if (player.IsDead())
                {
                    _notificationService.Notify(sender, new Notification
                    {
                        Severity = NotificationSeverity.Alert,
                        Text = player.PlayerId == sender.PlayerId ? "You are dead." : $"Player {player.Name} is dead."
                    });

                    return;
                }
            }

            _battleRepository.StartNew(battle);

            // Whether the player is in the battle or not - determined by querying current active list of battles.
        }
    }
}
