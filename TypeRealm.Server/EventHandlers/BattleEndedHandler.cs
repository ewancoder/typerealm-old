using TypeRealm.Domain;
using TypeRealm.Domain.Battling;
using TypeRealm.Domain.Battling.Events;

namespace TypeRealm.Server.EventHandlers
{
    internal sealed class BattleEndedHandler : DomainEventHandler<BattleEnded>
    {
        private readonly IBattleRepository _battleRepository;
        private readonly IPlayerRepository _playerRepository;

        public BattleEndedHandler(
            IBattleRepository battleRepository,
            IPlayerRepository playerRepository)
        {
            _battleRepository = battleRepository;
            _playerRepository = playerRepository;
        }

        public override void Handle(BattleEnded @event)
        {
            var battle = _battleRepository.Find(@event.BattleId);
            var result = battle.GetEndResult();

            foreach (var playerResult in result.Players)
            {
                var player = _playerRepository.Find(playerResult.PlayerId);

                player.UpdateHp(playerResult.Hp);

                _playerRepository.Save(player);
            }
        }
    }
}
