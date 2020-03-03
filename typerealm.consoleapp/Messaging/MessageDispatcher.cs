namespace TypeRealm.ConsoleApp.Messaging
{
    internal sealed class MessageDispatcher
    {
        private readonly IMessageHandlerFactory _factory;

        public MessageDispatcher(IMessageHandlerFactory factory)
        {
            _factory = factory;
        }

        public void Dispatch(object message)
        {
            var handler = _factory.Resolve(message.GetType());

            if (handler == null)
                throw new UnregisteredMessageHandlerException(message);

            handler.Handle(message);
        }
    }
}
