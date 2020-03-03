namespace TypeRealm.ConsoleApp.Typing
{
    internal sealed class Typers
    {
        public void SetZone(ZoneTypers zoneTypers)
        {
            Reset(zoneTypers);
            ZoneTypers = zoneTypers;
        }

        public void SetPassZone(PassZoneTypers passZoneTypers)
        {
            Reset(passZoneTypers);
            PassZoneTypers = passZoneTypers;
        }

        public void SetPlayerSelection(PlayerSelectionTypers playerSelectionTypers)
        {
            Reset(playerSelectionTypers);
            PlayerSelectionTypers = playerSelectionTypers;
        }

        public void SetBattle(BattleTypers battleTypers)
        {
            Reset(battleTypers);
            BattleTypers = battleTypers;
        }

        private IInputHandler _previousInputHandler;
        public void OpenAlert(AlertTypers alertTypers)
        {
            AlertTypers = alertTypers;
            _previousInputHandler = InputHandler;
            InputHandler = alertTypers;
        }

        public void CloseAlert()
        {
            AlertTypers = null;
            InputHandler = _previousInputHandler;
        }

        private void Reset(IInputHandler inputHandler)
        {
            AlertTypers = null;
            BattleTypers = null;
            PassZoneTypers = null;
            PlayerSelectionTypers = null;
            ZoneTypers = null;

            InputHandler = inputHandler;
        }

        public AlertTypers AlertTypers { get; private set; }
        public BattleTypers BattleTypers { get; private set; }
        public PassZoneTypers PassZoneTypers { get; private set; }
        public PlayerSelectionTypers PlayerSelectionTypers { get; private set; }
        public ZoneTypers ZoneTypers { get; private set; }

        public IInputHandler InputHandler { get; private set; }
    }
}
