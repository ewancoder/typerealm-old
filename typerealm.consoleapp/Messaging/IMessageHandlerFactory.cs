namespace TypeRealm.ConsoleApp.Messaging
{
    using System;

    internal interface IMessageHandlerFactory
    {
        IMessageHandler Resolve(Type messageType);
    }
}
