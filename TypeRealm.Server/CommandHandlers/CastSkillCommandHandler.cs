namespace TypeRealm.Server.CommandHandlers
{
    using TypeRealm.Domain.Battling;
    using TypeRealm.Messages;
    using TypeRealm.Server.Messaging;

    internal sealed class CastSkillCommandHandler : MessageHandler<CastSkillCommand>
    {
        private readonly IBattleApplication _battleApp;

        public CastSkillCommandHandler(IBattleApplication battleApp)
        {
            _battleApp = battleApp;
        }

        public override void Handle(ConnectedClient sender, CastSkillCommand message)
        {
            _battleApp.Cast(sender.PlayerId, message.SkillId, new[] { message.TargetId });
        }
    }
}
