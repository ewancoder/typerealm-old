/// <summary>
/// The network messaging engine was written at the night of new year 2019: from 8PM till 5AM.
/// </summary>
namespace TypeRealm.Server
{
    using System;
    using System.Collections.Generic;
    using TypeRealm.Messages;
    using TypeRealm.Server.Messaging;

    internal sealed class InMemoryMessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly Dictionary<Type, IMessageHandler> _handlers
            = new Dictionary<Type, IMessageHandler>();

        public InMemoryMessageHandlerFactory(IEnumerable<ConnectedClient> clients, IPlayerRepository playerRepository, BattleRepository battleRepository)
        {
            _handlers.Add(typeof(AttackCommand), new AttackCommandHandler(battleRepository));
            _handlers.Add(typeof(StartBattleCommand), new StartBattleCommandHandler(clients, playerRepository, battleRepository));
            _handlers.Add(typeof(StartMovingToZoneCommand), new StartMovingToZoneCommandHandler(playerRepository));
            _handlers.Add(typeof(TurnAroundCommand), new TurnAroundCommandHandler(playerRepository));
            _handlers.Add(typeof(MoveForCommand), new MoveForCommandHandler(playerRepository));
            _handlers.Add(typeof(FinishWalkingCommand), new FinishWalkingCommandHandler(playerRepository));
            _handlers.Add(typeof(StopBattleCommand), new StopBattleCommandHandler(battleRepository, playerRepository));
        }

        public IMessageHandler Resolve(Type messageType)
        {
            return _handlers[messageType];
        }
    }
}
