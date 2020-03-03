namespace TypeRealm.Domain.Tests.Battling
{
    using Xunit;
    using TypeRealm.Domain.Battling;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using TypeRealm.Domain.Battling.Events;

    public class BattleTests
    {
        private Battle _sut;

        [Fact]
        public void ShouldNotifyAboutStart()
        {
            var battleId = 3;
            var playerActivityMock = new Mock<IPlayerActivity>();
            var players = new List<Player>();

            _sut = new Battle(battleId, playerActivityMock.Object, players);

            var events = ((IEventSource)_sut).GetDomainEvents().ToList();
            Assert.Single(events);
            Assert.Equal(battleId, ((BattleStarted)events[0]).BattleId);
        }
    }
}
