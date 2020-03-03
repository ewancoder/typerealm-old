namespace TypeRealm.ConsoleApp.Messaging
{
    using System;
    using System.Collections.Generic;

    internal sealed class InMemoryMessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly Dictionary<Type, IMessageHandler> _handlers
            = new Dictionary<Type, IMessageHandler>();

        public IMessageHandler Resolve(Type messageType)
        {
            return _handlers[messageType];
        }

        internal void Register(Type messageType, IMessageHandler handler)
        {
            _handlers.Add(messageType, handler);
        }
    }
}
