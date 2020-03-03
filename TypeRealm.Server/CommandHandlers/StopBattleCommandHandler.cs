namespace TypeRealm.Server.CommandHandlers
{
    using Messages;
    using Messaging;
    using TypeRealm.Domain.Battling;

    internal sealed class StopBattleCommandHandler : MessageHandler<StopBattleCommand>
    {
        private readonly IBattleApplication _battleApp;

        public StopBattleCommandHandler(IBattleApplication battleApp)
        {
            _battleApp = battleApp;
        }

        public override void Handle(ConnectedClient sender, StopBattleCommand message)
        {
            _battleApp.VoteToEndBattle(sender.PlayerId);
        }
    }
}
