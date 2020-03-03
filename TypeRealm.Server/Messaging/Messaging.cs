namespace TypeRealm.Server.Messaging
{
    using System;

    internal abstract class MessageHandler<TMessage> : IMessageHandler
    {
        public abstract void Handle(ConnectedClient sender, TMessage message);

        public void Handle(ConnectedClient sender, object message)
        {
            Handle(sender, (TMessage)message);
        }
    }

    internal sealed class MessageDispatcher
    {
        private readonly IMessageHandlerFactory _factory;

        public MessageDispatcher(IMessageHandlerFactory factory)
        {
            _factory = factory;
        }

        public void Dispatch(ConnectedClient sender, object message)
        {
            var handler = _factory.Resolve(message.GetType());

            if (handler == null)
                throw new UnregisteredMessageHandlerException(message);

            handler.Handle(sender, message);
        }
    }

    internal sealed class UnregisteredMessageHandlerException : Exception
    {
        public UnregisteredMessageHandlerException(object message)
            : base($"Message handler for type {message.GetType()} is not registered.")
        {
        }
    }

    internal interface IMessageHandlerFactory
    {
        IMessageHandler Resolve(Type messageType);
    }

    internal interface IMessageHandler
    {
        void Handle(ConnectedClient sender, object message);
    }
}
