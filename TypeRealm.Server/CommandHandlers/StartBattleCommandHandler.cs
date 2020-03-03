namespace TypeRealm.Server.CommandHandlers
{
    using System.Linq;
    using Messages;
    using Messaging;

    using Domain.Battling;
    using ExploringPlayer = Domain.Exploring.Player;
    using BattlePlayer = Domain.Battling.Player;

    internal sealed class StartBattleCommandHandler : MessageHandler<StartBattleCommand>
    {
        private readonly IPlayerRepository _playerRepo;
        private readonly IBattleApplication _battleApp;

        public StartBattleCommandHandler(IPlayerRepository playerRepo, IBattleApplication battleApp)
        {
            _playerRepo = playerRepo;
            _battleApp = battleApp;
        }

        public override void Handle(ConnectedClient sender, StartBattleCommand message)
        {
            var players = message.ParticipantsExceptMeIds
                .Select(id => _playerRepo.Find(id)).Concat(new[] { _playerRepo.Find(sender.PlayerId) })
                .Select(p => ToBattlePlayer(p))
                .ToList();

            // TODO: Uncomment this IMPORTANT code.

            /*foreach (var player in players)
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
            }*/

            _battleApp.StartBattle(players);
        }

        private static BattlePlayer ToBattlePlayer(ExploringPlayer player)
        {
            return new BattlePlayer(
                player.PlayerId,
                new WeaponDamage(new PositiveValueRange(10, 15)), // TODO: get real values.
                new Ability(105), // TODO: get real values.
                new PlayerHp(player.Hp, player.MaxHp),
                player.SkillIds);
        }
    }
}
