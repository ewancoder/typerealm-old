namespace TypeRealm.ConsoleApp.Messaging
{
    internal abstract class MessageHandler<TMessage> : IMessageHandler
    {
        public abstract void Handle(TMessage message);

        public void Handle(object message)
        {
            Handle((TMessage)message);
        }
    }
}
