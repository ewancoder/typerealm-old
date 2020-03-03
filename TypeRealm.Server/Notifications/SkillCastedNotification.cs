using System;
using TypeRealm.Domain;
using TypeRealm.Domain.Battling;
using TypeRealm.Domain.Battling.Events;
using TypeRealm.Server.EventHandlers;

namespace TypeRealm.Server.Notifications
{
    public sealed class SkillCastedNotification : DomainEventHandler<SkillCasted>
    {
        public override void Handle(SkillCasted @event)
        {
            Console.WriteLine("Skill casted.");
        }
    }

    internal static class Setup
    {
        public static void Register(InMemoryEventHandlerFactory factory, IBattleRepository battleRepository, IPlayerRepository playerRepository)
        {
            factory.Register(typeof(SkillCasted), new SkillCastedNotification());

            // TODO: Move to sep. namespace.
            factory.Register(typeof(BattleEnded), new BattleEndedHandler(battleRepository, playerRepository));
        }
    }
}
