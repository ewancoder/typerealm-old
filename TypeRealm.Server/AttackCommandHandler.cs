/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using TypeRealm.Messages;
    using TypeRealm.Server.Messaging;

    internal sealed class AttackCommandHandler : MessageHandler<AttackCommand>
    {
        private readonly BattleRepository _battleRepository;

        public AttackCommandHandler(BattleRepository battleRepository)
        {
            _battleRepository = battleRepository;
        }

        public override void Handle(ConnectedClient sender, AttackCommand message)
        {
            var senderId = sender.PlayerId;
            var battle = _battleRepository.FindActiveFor(senderId);

            battle.Attack(senderId, message.TargetId, message.SkillId);
        }
    }
}
