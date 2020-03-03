namespace TypeRealm.ConsoleApp.Messaging
{
    internal interface IMessageHandler
    {
        void Handle(object message);
    }
}
