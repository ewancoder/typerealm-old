// TODO: Healing.
// When passing from location to location, there can be some healing factor (each 10 meters +x hp for instance).
// Change the way progress is passed. Don't pass raw value. Pass +X value.
// Typed 5 characters - pass 5. Typed another 5 - pass another 5.
// When changing direction - pass latest amount of characters typed (3 for instance) and then the direction change.
// Let server completely update your current position down to the character.

namespace TypeRealm.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Messaging;
    using Networking;
    using Typing;
    using Messages;

    internal sealed class Game : IDisposable
    {
        private readonly Connection _connection;
        private readonly MessageDispatcher _dispatcher;
        private readonly StatusPrinter _printer = new StatusPrinter(new Output());

        private readonly Typers _typers = new Typers();
        private readonly object _lock = new object();
        private readonly List<string> _notifications = new List<string>();
        private readonly Queue<string> _alerts = new Queue<string>();

        private Status _status;

        public Game(string server, int port, AuthInformation authInformation)
        {
            _connection = new Connection(server, port, authInformation);

            var factory = new InMemoryMessageHandlerFactory();
            factory.Register(typeof(Status), new StatusHandler(this));
            factory.Register(typeof(Notification), new NotificationHandler(this));
            _dispatcher = new MessageDispatcher(factory);

            _connection.ReconnectAndAuthorize();

            Task.Run(() =>
            {
                while (true)
                {
                    var message = MessageSerializer.Deserialize(_connection.Stream);

                    lock (_lock) // Puts message handlers in queue.
                    {
                        _dispatcher.Dispatch(message);
                    }
                }
            });
        }

        public void StopBattle()
        {
            _connection.Send(new StopBattleCommand());
        }

        public void ShowAlert(string text)
        {
            _alerts.Enqueue(text);
        }

        public void ShowNotification(string text)
        {
            _notifications.Add(text);
        }

        public void CloseAlert()
        {
            _typers.CloseAlert();
        }





        // hacks.
        private bool _isPlayerSelecting; // If true - shows neighbor selection window for selection.
        private List<string> _selectedPlayers; // Being set only after OK is pressed.
        private PassDirection _previousDirection; // hack.





        public void FinishWalking()
        {
            _connection.Send(new FinishWalkingCommand());
        }

        public void TurnAround()
        {
            _connection.Send(new TurnAroundCommand());
        }

        public void MoveFor(int progress)
        {
            _connection.Send(new MoveForCommand
            {
                Progress = progress
            });
        }

        public void UpdateStatus(Status status)
        {
            _status = status;
            Update();
        }

        public void Input(ConsoleKeyInfo key)
        {
            if (_typers.InputHandler == null)
            {
                // If the game is still not loaded yet.
                return;
            }

            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                    _typers.InputHandler.Backspace();
                    break;
                case ConsoleKey.Escape:
                    _typers.InputHandler.Escape();
                    break;
                case ConsoleKey.Tab:
                    _typers.InputHandler.Tab();
                    break;
                default:
                    _typers.InputHandler.Type(key.KeyChar);
                    break;
            }

            Update();
        }

        public void StartMovingToZone(int passZoneId)
        {
            _connection.Send(new StartMovingToZoneCommand
            {
                PassZoneId = passZoneId
            });
        }

        public void Attack(int skillId, string targetId)
        {
            _connection.Send(new AttackCommand
            {
                SkillId = skillId,
                TargetId = targetId
            });
        }

        public void InitiateBattle()
        {
            _isPlayerSelecting = true;
        }

        public void CancelPlayerSelection()
        {
            _isPlayerSelecting = false;
        }

        // TODO: Maybe pass selected array here.
        public void FinalizePlayerSelection()
        {
            if (!_typers.PlayerSelectionTypers.Selected.Any())
            {
                CancelPlayerSelection();
                return;
            }

            _selectedPlayers = _typers.PlayerSelectionTypers.Selected.ToList();
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }

        private string _currentAlert;
        private void Update()
        {
            lock (_lock)
            {
                Console.Clear();

                if (_status == null)
                {
                    _printer.PrintLoadingScreen();
                    return;
                }

                _printer.PrintNotifications(_notifications);

                // Battle priority is higher than alerts.
                if (_status.BattleStatus != null)
                {
                    if (_typers.BattleTypers == null)
                    {
                        var battleTypers = new BattleTypers(this, _status.BattleStatus.SkillIds, _status.BattleStatus.BattleNeighbors.Concat(new[] { _status.BattleStatus.Player }).Select(p => p.PlayerId));
                        _typers.SetBattle(battleTypers);
                    }

                    _printer.PrintBattleStatus(_status.BattleStatus, _typers.BattleTypers);
                    return;
                }

                _printer.PrintPlayerStatus(_status.PlayerStatus);

                if (_typers.AlertTypers == null && _alerts.Any())
                {
                    _currentAlert = _alerts.Dequeue();
                    var alertTypers = new AlertTypers(this);
                    _typers.OpenAlert(alertTypers);
                }

                if (_typers.AlertTypers != null)
                {
                    _printer.PrintAlert(_currentAlert, _typers.AlertTypers);
                    return;
                }

                if (_isPlayerSelecting)
                {
                    var battleEligibleNeighbors = _status.ZoneStatus.Neighbors
                        .Where(n => !n.IsInBattle && !n.IsDead);

                    if (_typers.PlayerSelectionTypers == null)
                    {
                        var playerSelectionTypers = new PlayerSelectionTypers(this, battleEligibleNeighbors.Select(n => n.PlayerId));
                        _typers.SetPlayerSelection(playerSelectionTypers);
                    }

                    if (_selectedPlayers != null) // Typer is done.
                    {
                        _connection.Send(new StartBattleCommand
                        {
                            ParticipantsExceptMeIds = _selectedPlayers
                        });

                        _selectedPlayers = null;
                        _isPlayerSelecting = false;
                        return;
                    }

                    _printer.PrintPlayerSelectionStatus(battleEligibleNeighbors, _typers.PlayerSelectionTypers);
                    return;
                }

                if (_status.PlayerStatus.PassZone != null)
                {
                    if (_typers.PassZoneTypers == null)
                    {
                        var passZoneTypers = new PassZoneTypers(this, _status.PlayerStatus.PassZone);
                        _typers.SetPassZone(passZoneTypers);
                        _previousDirection = _status.PlayerStatus.PassZone.Direction;
                    }

                    if (_previousDirection != _status.PlayerStatus.PassZone.Direction)
                    {
                        // Update for new direction and distance.
                        var passZoneTypers = new PassZoneTypers(this, _status.PlayerStatus.PassZone); // Can't flip direction. Need to recreate because need smaller/bigger text for reverse direction.
                        _typers.SetPassZone(passZoneTypers);
                        _previousDirection = _status.PlayerStatus.PassZone.Direction;
                    }

                    _printer.PrintPassingStatus(_status.PassZoneStatus, _status.PlayerStatus.PassZone, _typers.PassZoneTypers);
                    return;
                }

                var zone = Data.Data.GetZone(_status.PlayerStatus.Zone.ZoneId);

                if (_typers.ZoneTypers == null)
                {
                    var zoneTypers = new ZoneTypers(this, zone.ZoneExits.Concat(zone.ZoneEntrances));
                    _typers.SetZone(zoneTypers);
                }

                _printer.PrintZoneInfo(_status.ZoneStatus, _status.PlayerStatus, _typers.ZoneTypers);
            }
        }
    }
}
