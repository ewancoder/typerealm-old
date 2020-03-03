using System;
using System.Collections.Generic;

namespace TypeRealm.Domain.Battling
{
    using TypeRealm.Domain.Battling.Skills;

    // TODO: Move locking as a cross-cutting concern to another class-decorator.
    public sealed class BattleApplication : IBattleApplication
    {
        private readonly IPlayerActivity _playerActivity;
        private readonly IBattleRepository _battleRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly EventDispatcher _eventDispatcher;
        private readonly object _lock = new object();

        public BattleApplication(
            IPlayerActivity playerActivity,
            IBattleRepository battleRepository,
            ISkillRepository skillRepository,
            EventDispatcher eventDispatcher)
        {
            _playerActivity = playerActivity;
            _battleRepository = battleRepository;
            _skillRepository = skillRepository;
            _eventDispatcher = eventDispatcher;
        }

        public int StartBattle(IEnumerable<Player> participants)
        {
            lock (_lock)
            {
                var battleId = _battleRepository.GetNextId();
                var battle = new Battle(battleId, _playerActivity, participants);

                _battleRepository.Save(battle);
                _eventDispatcher.Dispatch(battle);

                return battleId;
            }
        }

        public void Cast(string casterId, int skillId, IEnumerable<string> targetIds)
        {
            lock (_lock)
            {
                var battle = _battleRepository.FindActiveFor(casterId);
                if (battle == null)
                    throw new InvalidOperationException("Active battle does not exist for caster.");

                var skill = _skillRepository.Find(skillId);
                if (skill == null)
                    throw new InvalidOperationException("Skill does not exist.");

                battle.Cast(casterId, skill, targetIds);

                _battleRepository.Save(battle);
                _eventDispatcher.Dispatch(battle);
            }
        }

        public void VoteToEndBattle(string voterId)
        {
            lock (_lock)
            {
                var battle = _battleRepository.FindActiveFor(voterId);
                if (battle == null)
                    throw new InvalidOperationException("Active battle does not exist for voter.");

                battle.VoteToEnd(voterId);

                _battleRepository.Save(battle);
                _eventDispatcher.Dispatch(battle);
            }
        }

        public BattleState FindActiveBattleStateFor(string playerId)
        {
            lock (_lock)
            {
                var battle = _battleRepository.FindActiveFor(playerId);
                if (battle == null)
                    return null;

                return battle.GetState();
            }
        }
    }
}
