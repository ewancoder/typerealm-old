namespace TypeRealm.ConsoleApp
{
    using System;
    using Messaging;
    using Messages;

    internal sealed class NotificationHandler : MessageHandler<Notification>
    {
        private readonly Game _game;

        public NotificationHandler(Game game)
        {
            _game = game;
        }

        public override void Handle(Notification message)
        {
            switch (message.Severity)
            {
                case NotificationSeverity.Alert:
                    _game.ShowAlert(message.Text);
                    break;
                case NotificationSeverity.Notification:
                    _game.ShowNotification(message.Text);
                    break;
                default:
                    throw new InvalidOperationException("Invalid notification severity.");
            }
        }
    }
}
