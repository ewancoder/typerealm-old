/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using System.Collections.Generic;
    using Messages;
    using Messaging;

    internal sealed class StopBattleCommandHandler : MessageHandler<StopBattleCommand>
    {
        private readonly BattleRepository _battleRepository;
        private readonly IPlayerRepository _playerRepository;

        public StopBattleCommandHandler(BattleRepository battleRepository, IPlayerRepository playerRepository)
        {
            _battleRepository = battleRepository;
            _playerRepository = playerRepository;
        }

        public override void Handle(ConnectedClient sender, StopBattleCommand message)
        {
            var battle = _battleRepository.FindActiveFor(sender.PlayerId);

            battle.StopBattle(sender.PlayerId);

            if (battle.IsFinished)
            {
                // Finalize battle. Sync to all the player aggregates.

                foreach (var result in battle.GetBattleResults())
                {
                    var player = _playerRepository.Find(result.PlayerId);
                    player.UpdateHp(result.Hp);
                    _playerRepository.Save(player);
                }
            }
        }
    }
}
