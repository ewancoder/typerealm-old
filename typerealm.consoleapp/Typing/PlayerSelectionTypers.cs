namespace TypeRealm.ConsoleApp.Typing
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class PlayerSelectionTypers : MultiTyper
    {
        private readonly Dictionary<string, Typer> _playerTypers
            = new Dictionary<string, Typer>();
        private readonly HashSet<string> _selected = new HashSet<string>();

        public PlayerSelectionTypers(Game game, IEnumerable<string> playerIds)
            : base(game)
        {
            foreach (var playerId in playerIds)
            {
                _playerTypers.Add(playerId, MakeUniqueTyper());
            }

            CancelTyper = MakeUniqueTyper();
            OkTyper = MakeUniqueTyper();
        }

        public IEnumerable<string> Selected => _selected;
        public Typer CancelTyper { get; }
        public Typer OkTyper { get; }

        public Typer GetPlayerTyper(string playerId)
        {
            return _playerTypers[playerId];
        }

        protected override void OnTyped(Typer typer)
        {
            if (typer == CancelTyper)
            {
                Game.CancelPlayerSelection();
                return;
            }

            if (typer == OkTyper)
            {
                Game.FinalizePlayerSelection();
                return;
            }

            // Else it's one of player's typers.
            var player = _playerTypers.Single(t => t.Value == typer);

            if (_selected.Contains(player.Key))
                _selected.Remove(player.Key);
            else
                _selected.Add(player.Key);
        }
    }
}
