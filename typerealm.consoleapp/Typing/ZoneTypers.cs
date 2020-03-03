namespace TypeRealm.ConsoleApp.Typing
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class ZoneTypers : MultiTyper
    {
        private readonly Dictionary<int, Typer> _zoneTypers
            = new Dictionary<int, Typer>();

        public ZoneTypers(Game game, IEnumerable<int> zoneIds) : base(game)
        {
            foreach (var zoneId in zoneIds)
            {
                _zoneTypers.Add(zoneId, MakeUniqueTyper());
            }

            AttackTyper = MakeUniqueTyper();
        }

        public Typer AttackTyper { get; }

        public Typer GetZoneTyper(int zoneId)
        {
            return _zoneTypers[zoneId];
        }

        protected override void OnTyped(Typer typer)
        {
            if (typer == AttackTyper)
            {
                Game.InitiateBattle();
                return;
            }

            var zoneId = _zoneTypers.Single(t => t.Value == typer).Key;
            Game.StartMovingToZone(zoneId);
        }
    }
}
