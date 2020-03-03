namespace TypeRealm.ConsoleApp.Typing
{
    internal sealed class AlertTypers : MultiTyper
    {
        public AlertTypers(Game game) : base(game)
        {
            OkTyper = MakeUniqueTyper();
        }

        public Typer OkTyper { get; }

        protected override void OnTyped(Typer typer)
        {
            Game.CloseAlert();
        }
    }
}
