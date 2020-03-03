namespace TypeRealm.ConsoleApp
{
    using TypeRealm.ConsoleApp.Messaging;
    using TypeRealm.Messages;

    internal sealed class StatusHandler : MessageHandler<Status>
    {
        private readonly Game _game;

        public StatusHandler(Game game)
        {
            _game = game;
        }

        public override void Handle(Status message)
        {
            _game.UpdateStatus(message);
        }
    }
}
