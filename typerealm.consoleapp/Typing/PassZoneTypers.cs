namespace TypeRealm.ConsoleApp.Typing
{
    using System;
    using Messages;

    internal sealed class PassZoneTypers : IInputHandler
    {
        private readonly Game _game;
        private int _typeCount;

        public PassZoneTypers(Game game, PassZoneMessage passZoneStatus)
        {
            _game = game;
            var passZoneId = passZoneStatus.PassZoneId;
            var direction = passZoneStatus.Direction;

            var passZone = Data.Data.GetPassZone(passZoneId);
            var distance = passZone.GetDistance(direction);
            var progress = passZoneStatus.Progress;

            var text = Data.Data.GetText(distance, 90);

            Typer = new Typer(text, progress);
        }

        public Typer Typer { get; }

        public void Type(char character)
        {
            Typer.Type(character);

            if (Typer.Error.Length == 0)
                _typeCount++;

            if (_typeCount == 10)
            {
                _game.MoveFor(_typeCount);
                _typeCount = 0;
            }

            if (Typer.IsFinishedTyping)
            {
                _game.MoveFor(_typeCount);
                _typeCount = 0;

                _game.FinishWalking();
            }
        }

        public void Tab()
        {
            _game.MoveFor(_typeCount);
            _typeCount = 0;

            _game.TurnAround();
        }

        public void Backspace()
        {
            // Allow backspaces only for errors here.
            if (Typer.Error.Length > 0)
                Typer.Backspace();
        }

        public void Escape()
        {
            // Doesn't have escape functionality.
        }
    }
}
