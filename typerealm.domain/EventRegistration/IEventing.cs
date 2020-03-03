using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeRealm.Domain
{
    public interface IEventSource
    {
        IEnumerable<object> GetDomainEvents();
        void CommitDomainEvents();
    }

    public interface IEventHandler
    {
        void Handle(object @event);
    }

    public interface IEventHandlerFactory
    {
        IEnumerable<IEventHandler> Resolve(Type eventType);
    }

    public sealed class InMemoryEventHandlerFactory : IEventHandlerFactory
    {
        private readonly Dictionary<Type, List<IEventHandler>> _handlers
            = new Dictionary<Type, List<IEventHandler>>();

        public IEnumerable<IEventHandler> Resolve(Type eventType)
        {
            if (!_handlers.ContainsKey(eventType))
                return Enumerable.Empty<IEventHandler>();

            return _handlers[eventType];
        }

        public void Register(Type type, IEventHandler handler)
        {
            if (!_handlers.ContainsKey(type))
            {
                _handlers.Add(type, new List<IEventHandler> { handler });
                return;
            }

            var existing = _handlers[type];
            existing.Add(handler);
        }
    }

    public abstract class DomainEventHandler<TEvent> : IEventHandler
    {
        public abstract void Handle(TEvent @event);

        public void Handle(object @event)
        {
            Handle((TEvent)@event);
        }
    }

    public sealed class EventDispatcher
    {
        private readonly IEventHandlerFactory _factory;

        public EventDispatcher(IEventHandlerFactory factory)
        {
            _factory = factory;
        }

        public void Dispatch(IEventSource eventSource)
        {
            foreach (var @event in eventSource.GetDomainEvents())
            {
                Dispatch(@event);
            }

            eventSource.CommitDomainEvents();
        }

        private void Dispatch(object @event)
        {
            var handlers = _factory.Resolve(@event.GetType());

            foreach (var handler in handlers)
            {
                handler.Handle(@event);
            }
        }
    }
}
