namespace TypeRealm.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Typing;

    internal sealed class StatusPrinter
    {
        private const int HalfWidth = 90;
        private const int SeparatorWidth = 5;
        private const int Width = HalfWidth * 2 + SeparatorWidth;
        private readonly Output _output;

        public StatusPrinter(Output output)
        {
            _output = output;
        }

        public void PrintPassingStatus(PassZoneStatus status, PassZoneMessage passZone, PassZoneTypers passZoneTypers)
        {
            var info = Data.Data.GetPassZone(passZone.PassZoneId);
            var name = info.GetName(passZone.Direction);
            var description = info.GetDescription(passZone.Direction);

            _output.WriteLine(new string('-', HalfWidth));
            _output.WriteLine();
            _output.WriteLine(name);
            _output.WriteLine();
            PrintWrapped(description);
            _output.WriteLine();
            _output.WriteLine(new string('-', HalfWidth));
            _output.WriteLine();

            _output.Write(passZoneTypers.Typer);
            _output.WriteLine();

            var index = 1;
            foreach (var wanderer in status.Neighbors)
            {
                _output.WriteLine($"{index} - {wanderer.Name}");
                index++;
            }

            _output.WriteLine();

            PrintPassingPosition(status.Me, "M");

            index = 1;
            foreach (var wanderer in status.Neighbors)
            {
                PrintPassingPosition(wanderer, index.ToString());
                index++;
            }

            _output.WriteLine();
            _output.WriteLine(new string('-', HalfWidth));
            _output.WriteLine();

            _output.WriteLine();
            _output.WriteLine(new string('-', HalfWidth));
            _output.WriteLine(new string('_', Width));
        }

        private void PrintPassingPosition(PassPlayerMessage wanderer, string signChar)
        {
            var signWidth = 3;
            var nonSignWidth = HalfWidth - signWidth;
            var left = (int)Math.Floor(wanderer.ProgressPercentage * (nonSignWidth) / 100d);
            var sign = wanderer.Direction == PassDirection.Forward ? $"{signChar}>".PadLeft(signWidth) : $"<{signChar}".PadRight(signWidth);

            if (wanderer.Direction == PassDirection.Backward)
                left = nonSignWidth - left;

            var right = nonSignWidth - left;

            _output.WriteLine($"{new string('=', left)}{sign}{new string('=', right)}");
        }

        public void PrintLoadingScreen()
        {
            _output.WriteLine(new string('-', Width));
            _output.WriteLine();
            _output.WriteLine("LOADING...");
            _output.WriteLine();
            _output.WriteLine(new string('-', Width));
        }

        public void PrintPlayerStatus(PlayerStatus status)
        {
            // Print player info.
            _output.WriteLine(new string('-', Width));
            _output.WriteLine($"{status.Name}   [ {status.Hp} / {status.MaxHp} ]");
            _output.WriteLine(new string('-', Width));
            _output.WriteLine();
        }

        public void PrintPlayerSelectionStatus(IEnumerable<PlayerMessage> players, PlayerSelectionTypers typers)
        {
            _output.WriteLine(new string('-', Width));
            _output.WriteLine();

            foreach (var player in players)
            {
                if (typers.Selected.Contains(player.PlayerId))
                    Console.ForegroundColor = ConsoleColor.Green;

                _output.Write($"{player.Name}   ");
                Console.ForegroundColor = ConsoleColor.Gray;

                _output.WriteLine(typers.GetPlayerTyper(player.PlayerId));
                _output.WriteLine();
            }

            _output.Write($"Cancel - ");
            _output.Write(typers.CancelTyper);
            _output.Write("   OK - ");
            _output.WriteLine(typers.OkTyper);
            _output.WriteLine();

            _output.WriteLine(new string('-', Width));
        }

        public void PrintBattleStatus(BattleStatus status, BattleTypers typers)
        {
            _output.WriteLine(new string('-', Width));
            _output.WriteLine();

            if (typers.SelectedPlayerId == status.Player.PlayerId)
                Console.ForegroundColor = ConsoleColor.Green;

            _output.Write($"{(status.Player.IsVotedToStop ? "(stop) " : string.Empty)}{status.Player.Name}   [ {status.Player.Hp} / {status.Player.MaxHp} ]   ");
            Console.ForegroundColor = ConsoleColor.Gray;

            _output.WriteLine(typers.GetPlayerTyper(status.Player.PlayerId));
            _output.WriteLine();

            foreach (var player in status.BattleNeighbors)
            {
                if (typers.SelectedPlayerId == player.PlayerId)
                    Console.ForegroundColor = ConsoleColor.Green;

                _output.Write($"{(player.IsVotedToStop ? "(stop) " : string.Empty)}{player.Name}   [ {player.Hp} / {player.MaxHp} ]   ");
                Console.ForegroundColor = ConsoleColor.Gray;

                _output.WriteLine(typers.GetPlayerTyper(player.PlayerId));
                _output.WriteLine();
            }

            _output.WriteLine(new string('-', Width));
            _output.WriteLine();

            foreach (var skillId in status.SkillIds)
            {
                var skill = Data.Data.GetSkill(skillId);

                if (typers.SelectedSkillId == skillId)
                    Console.ForegroundColor = ConsoleColor.Green;

                _output.Write($"{skill.Name}   ");

                if (typers.SelectedSkillId == skillId)
                    Console.ForegroundColor = ConsoleColor.Gray;

                _output.Write(typers.GetSkillTyper(skill.SkillId));
                _output.WriteLine();
                _output.WriteLine();
            }

            _output.WriteLine(new string('-', Width));

            _output.WriteLine();
            _output.Write("Stop - ");
            _output.WriteLine(typers.StopTyper);
        }

        internal void PrintNotifications(List<string> notifications)
        {
            _output.WriteLine(new string('-', Width));
            _output.WriteLine();

            foreach (var notification in notifications.Skip(Math.Max(0, notifications.Count -5)))
            {
                _output.WriteLine(notification);
                _output.WriteLine();
            }

            _output.WriteLine(new string('-', Width));
        }

        public void PrintAlert(string currentAlert, AlertTypers alertTypers)
        {
            _output.WriteLine(new string('-', Width));
            _output.WriteLine();
            _output.WriteLine(currentAlert);
            _output.WriteLine();
            _output.Write("OK - ");
            _output.WriteLine(alertTypers.OkTyper);
            _output.WriteLine();
            _output.WriteLine(new string('-', Width));
        }

        public void PrintZoneInfo(ZoneStatus zoneStatus, PlayerStatus status, ZoneTypers zoneTypers)
        {
            // Print zone info.
            var zone = Data.Data.GetZone(status.Zone.ZoneId);

            _output.WriteLine(new string('-', HalfWidth));
            _output.WriteLine(zone.Name);
            _output.WriteLine();

            PrintWrapped(zone.Description);
            _output.WriteLine($"{new string(' ', HalfWidth + SeparatorWidth)}{new string('-', HalfWidth)}");

            var exits = zone.ZoneExits
                .Select(id => Data.Data.GetPassZone(id))
                .Select(z => new
                {
                    PassZoneId = z.PassZoneId,
                    Name = z.ForwardPassage.Name,
                    Description = z.ForwardPassage.Description,
                    Distance = z.ForwardPassage.Distance
                });

            var entrances = zone.ZoneEntrances
                .Select(id => Data.Data.GetPassZone(id))
                .Select(z => new
                {
                    PassZoneId = z.PassZoneId,
                    Name = z.BackwardPassage.Name,
                    Description = z.BackwardPassage.Description,
                    Distance = z.BackwardPassage.Distance
                });

            var ways = exits.Concat(entrances).OrderBy(z => z.Name);

            foreach (var way in ways)
            {
                var lines = WrapText(way.Description, HalfWidth);

                var isFirstLine = true;
                foreach (var line in lines)
                {
                    if (isFirstLine)
                    {
                        _output.Write($"{line.PadRight(HalfWidth)}{new string(' ', SeparatorWidth)}{way.Name}   ");
                        _output.Write(zoneTypers.GetZoneTyper(way.PassZoneId));
                        _output.WriteLine();
                    }
                    else
                        _output.WriteLine(line);

                    isFirstLine = false;
                }

                _output.WriteLine($"{new string(' ', HalfWidth + SeparatorWidth)}{new string('-', HalfWidth)}");
            }

            _output.WriteLine(new string('-', HalfWidth));
            _output.WriteLine();

            foreach (var neighbor in zoneStatus.Neighbors)
            {
                var isInBattle = neighbor.IsInBattle ? "B " : string.Empty;
                var isDead = neighbor.IsDead ? "D " : string.Empty;
                _output.WriteLine($"{isDead}{isInBattle}{neighbor.Name}");
            }

            _output.WriteLine(new string('_', Width));
            _output.WriteLine();

            _output.Write($"Attack   ");
            _output.WriteLine(zoneTypers.AttackTyper);
        }

        private void PrintWrapped(string text)
        {
            foreach (var line in WrapText(text, HalfWidth))
            {
                _output.WriteLine(line);
            }
        }

        private IEnumerable<string> WrapText(string text, int width)
        {
            while (text.Length > 0)
            {
                var part = text.Substring(0, Math.Min(width, text.Length));

                if (part.Contains(' ') && part != text)
                {
                    while (part[part.Length - 1] != ' ')
                    {
                        part = part.Substring(0, part.Length - 1);
                    }

                    part = part.Substring(0, part.Length - 1);
                    text = text.Substring(1);
                }

                text = text.Substring(part.Length);

                yield return part;
            }
        }
    }
}
