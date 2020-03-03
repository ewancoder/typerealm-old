namespace TypeRealm.ConsoleApp.Messaging
{
    using System;

    internal sealed class UnregisteredMessageHandlerException : Exception
    {
        public UnregisteredMessageHandlerException(object message)
            : base($"Message handler for type {message.GetType()} is not registered.")
        {
        }
    }
}
