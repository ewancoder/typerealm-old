namespace TypeRealm.ConsoleApp.Typing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class BattleTypers : MultiTyper
    {
        private readonly Dictionary<int, Typer> _skillTypers
            = new Dictionary<int, Typer>();
        private readonly Dictionary<string, Typer> _playerTypers
            = new Dictionary<string, Typer>();

        public BattleTypers(
            Game game,
            IEnumerable<int> skillIds,
            IEnumerable<string> playerIds)
            : base(game)
        {
            foreach (var skillId in skillIds)
            {
                _skillTypers.Add(skillId, MakeUniqueTyper());
            }

            foreach (var playerId in playerIds)
            {
                _playerTypers.Add(playerId, MakeUniqueTyper());
            }

            StopTyper = MakeUniqueTyper();
        }

        public Typer StopTyper { get; }
        public int SelectedSkillId { get; private set; }
        public string SelectedPlayerId { get; private set; }

        public Typer GetPlayerTyper(string playerId)
        {
            return _playerTypers[playerId];
        }

        public Typer GetSkillTyper(int skillId)
        {
            return _skillTypers[skillId];
        }

        protected override void OnTyped(Typer typer)
        {
            if (typer == StopTyper)
            {
                // Vote to stop a battle.
                Game.StopBattle();
                return;
            }

            var skill = _skillTypers.SingleOrDefault(x => x.Value == typer);
            if (skill.Value != null)
            {
                SelectedSkillId = skill.Key;

                AttackIfReady();
                return;
            }

            var player = _playerTypers.SingleOrDefault(x => x.Value == typer);
            if (player.Value != null)
            {
                SelectedPlayerId = player.Key;

                AttackIfReady();
                return;
            }

            throw new InvalidOperationException("Invalid typer.");
        }

        private void AttackIfReady()
        {
            if (SelectedPlayerId != null && SelectedSkillId > 0)
            {
                Game.Attack(SelectedSkillId, SelectedPlayerId);
                SelectedSkillId = 0;
                SelectedPlayerId = null;
            }
        }
    }
}
