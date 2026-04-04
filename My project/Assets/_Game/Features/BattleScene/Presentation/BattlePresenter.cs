using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Core.MasterData;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.BattleScene.Domain;
using Samsara.Features.BattleScene.Presentation.ActionOrder;
using Samsara.Features.BattleScene.Presentation.Skill;
using Samsara.Features.Skill.Domain;
using UnityEngine;

namespace Samsara.Features.BattleScene.Presentation
{
    public class BattlePresenter
    {
        private readonly string _logClass = $"[{nameof(BattlePresenter)}]";

        private readonly BattleUseCase _useCase;
        private readonly BattleView _view;
        private readonly ISceneNavigator _sceneNavigator;
        private readonly IPopupManager _popupManager;
        private readonly ISkillMasterDataRepository _skillMasterDataRepo;
        private readonly GameContext _gameContext;

        // Async state
        private UniTaskCompletionSource<int> _skillSelectionTcs;
        private UniTaskCompletionSource<int> _targetSelectionTcs;
        private UniTaskCompletionSource _resultConfirmTcs;

        // Cached handlers
        private Action<int> _skillSelectedHandler;
        private Action<int> _targetSelectedHandler;
        private Action _resultConfirmHandler;
        private Action _optionClickedHandler;

        public BattlePresenter(
            BattleUseCase useCase,
            BattleView view,
            ISceneNavigator sceneNavigator,
            IPopupManager popupManager,
            ISkillMasterDataRepository skillMasterDataRepo,
            GameContext gameContext)
        {
            _useCase = useCase;
            _view = view;
            _sceneNavigator = sceneNavigator;
            _popupManager = popupManager;
            _skillMasterDataRepo = skillMasterDataRepo;
            _gameContext = gameContext;
        }

        public void Initialize(PendingBattleContext context)
        {
            _useCase.InitializeBattle(context);

            var runtimeData = _useCase.RuntimeData;

            // Render allies & enemies
            _view.AllyField.RenderAllies(runtimeData.Allies.ToArray());
            _view.EnemyField.RenderEnemies(runtimeData.Enemies.ToArray());

            // Action order initial display
            UpdateActionOrderUI();

            // Turn number
            _view.TurnNumber.SetTurn(1);

            // Subscribe events
            _skillSelectedHandler = HandleSkillSelected;
            _targetSelectedHandler = HandleTargetSelected;
            _resultConfirmHandler = HandleResultConfirm;
            _optionClickedHandler = HandleOptionClicked;

            _view.OnSkillSelected += _skillSelectedHandler;
            _view.OnTargetSelected += _targetSelectedHandler;
            _view.OnResultConfirm += _resultConfirmHandler;
            _view.OnOptionClicked += _optionClickedHandler;

            // Deactivate skill panel initially
            _view.SkillSelection.SetActive(false);
            _view.EnemyField.EnableTargetSelection(false);

            // Start battle loop
            RunBattleLoop().Forget();

            Debug.Log($"{_logClass} Initialize 완료.");
        }

        public void Dispose()
        {
            _view.OnSkillSelected -= _skillSelectedHandler;
            _view.OnTargetSelected -= _targetSelectedHandler;
            _view.OnResultConfirm -= _resultConfirmHandler;
            _view.OnOptionClicked -= _optionClickedHandler;

            _skillSelectionTcs?.TrySetCanceled();
            _targetSelectionTcs?.TrySetCanceled();
            _resultConfirmTcs?.TrySetCanceled();

            Debug.Log($"{_logClass} Dispose 완료.");
        }

        // ──────────────────────────────────────────────
        // Battle Loop
        // ──────────────────────────────────────────────

        private async UniTaskVoid RunBattleLoop()
        {
            while (true)
            {
                // Tick until someone is ready
                List<BattleParticipant> readyQueue;
                do
                {
                    readyQueue = _useCase.ProcessTick();
                    if (readyQueue.Count == 0)
                        await UniTask.Yield();
                } while (readyQueue.Count == 0);

                // Process each ready participant
                for (int i = 0; i < readyQueue.Count; i++)
                {
                    var actor = readyQueue[i];
                    if (actor.IsDead) continue;

                    _useCase.ConsumeGauge(actor);

                    if (actor.IsAlly)
                        await ProcessAllyTurn(actor);
                    else
                        await ProcessEnemyTurn(actor);

                    _useCase.ReduceCooldowns(actor);

                    // Update UI after action
                    UpdateAllUnitHP();
                    UpdateActionOrderUI();

                    // Check battle end
                    var result = _useCase.CheckBattleEnd();
                    if (result.HasValue)
                    {
                        await HandleBattleEnd(result.Value);
                        return;
                    }
                }

                // Increment turn after all ready participants have acted
                _useCase.IncrementTurn();
                _view.TurnNumber.SetTurn(_useCase.RuntimeData.TurnNumber);
            }
        }

        // ──────────────────────────────────────────────
        // Ally Turn
        // ──────────────────────────────────────────────

        private async UniTask ProcessAllyTurn(BattleParticipant actor)
        {
            // Activate skill selection
            var usableSkills = _useCase.GetUsableSkills(actor);
            var skillDisplays = BuildSkillDisplayData(actor, usableSkills);
            _view.SkillSelection.SetSkills(skillDisplays);
            _view.SkillSelection.SetActive(true);

            // Await skill selection
            int selectedSkillId = await WaitForSkillSelection();
            _view.SkillSelection.SetActive(false);

            // Await target selection
            _view.EnemyField.EnableTargetSelection(true);
            int targetId = await WaitForTargetSelection();
            _view.EnemyField.EnableTargetSelection(false);

            var target = FindParticipantById(targetId);

            // QTE (if skill has QTE pattern)
            float qteRate = 1.0f;
            if (selectedSkillId != BattleUseCase.DefaultAttackId)
            {
                var skillSO = _skillMasterDataRepo.GetSkill(selectedSkillId);
                if (skillSO.QtePatternId > 0)
                {
                    var qtePattern = _skillMasterDataRepo.GetQTEPattern(skillSO.QtePatternId);
                    var qteResults = await _view.BattleQTE.RunQTE(qtePattern.QteDataList, true);
                    qteRate = _useCase.CalculateQTERate(true, qteResults, qteResults.Length);
                }
            }

            // Execute action
            int damage = _useCase.ExecuteAction(actor, selectedSkillId, target, qteRate);

            // Show damage popup
            var targetUnit = _view.EnemyField.GetUnit(targetId);
            _view.DamagePopup.ShowDamage(damage, targetUnit.transform.position);

            // Check death
            if (target.IsDead)
                targetUnit.SetDead();

            await UniTask.Delay(300);
        }

        // ──────────────────────────────────────────────
        // Enemy Turn
        // ──────────────────────────────────────────────

        private async UniTask ProcessEnemyTurn(BattleParticipant actor)
        {
            int skillId = _useCase.SelectEnemySkill(actor);
            var target = _useCase.SelectEnemyTarget(actor);

            if (target == null) return;

            // Defense QTE (if skill has QTE pattern)
            float qteRate = 1.0f;
            if (skillId != BattleUseCase.DefaultAttackId)
            {
                var skillSO = _skillMasterDataRepo.GetSkill(skillId);
                if (skillSO.QtePatternId > 0)
                {
                    var qtePattern = _skillMasterDataRepo.GetQTEPattern(skillSO.QtePatternId);
                    var qteResults = await _view.BattleQTE.RunQTE(qtePattern.QteDataList, false);
                    qteRate = _useCase.CalculateQTERate(false, qteResults, qteResults.Length);
                }
            }

            // Execute action
            int damage = _useCase.ExecuteAction(actor, skillId, target, qteRate);

            // Show damage popup
            var targetUnit = _view.AllyField.GetUnit(target.Id);
            _view.DamagePopup.ShowDamage(damage, targetUnit.transform.position);

            // Check death
            if (target.IsDead)
                targetUnit.SetDead();

            await UniTask.Delay(500);
        }

        // ──────────────────────────────────────────────
        // Battle End
        // ──────────────────────────────────────────────

        private async UniTask HandleBattleEnd(BattleResult result)
        {
            _useCase.CleanupBattle();

            _view.BattleResultPopup.Show(result);
            await WaitForResultConfirm();

            _gameContext.LastBattleResult = result;
            _gameContext.PendingBattleContext = null;

            await _sceneNavigator.NavigateToAsync(SceneKey.Stage);
        }

        // ──────────────────────────────────────────────
        // Await Helpers
        // ──────────────────────────────────────────────

        private UniTask<int> WaitForSkillSelection()
        {
            _skillSelectionTcs = new UniTaskCompletionSource<int>();
            return _skillSelectionTcs.Task;
        }

        private UniTask<int> WaitForTargetSelection()
        {
            _targetSelectionTcs = new UniTaskCompletionSource<int>();
            return _targetSelectionTcs.Task;
        }

        private UniTask WaitForResultConfirm()
        {
            _resultConfirmTcs = new UniTaskCompletionSource();
            return _resultConfirmTcs.Task;
        }

        // ──────────────────────────────────────────────
        // Event Handlers
        // ──────────────────────────────────────────────

        private void HandleSkillSelected(int skillId)
        {
            _skillSelectionTcs?.TrySetResult(skillId);
        }

        private void HandleTargetSelected(int participantId)
        {
            _targetSelectionTcs?.TrySetResult(participantId);
        }

        private void HandleResultConfirm()
        {
            _resultConfirmTcs?.TrySetResult();
        }

        private void HandleOptionClicked()
        {
            Debug.Log($"{_logClass} Option 클릭 — 미구현.");
        }

        // ──────────────────────────────────────────────
        // UI Update Helpers
        // ──────────────────────────────────────────────

        private void UpdateAllUnitHP()
        {
            var runtimeData = _useCase.RuntimeData;
            if (runtimeData == null) return;

            foreach (var ally in runtimeData.Allies)
            {
                var unit = _view.AllyField.GetUnit(ally.Id);
                unit.SetHp(ally.CurrentHp, ally.MaxHp);
                if (ally.IsDead) unit.SetDead();
            }

            foreach (var enemy in runtimeData.Enemies)
            {
                var unit = _view.EnemyField.GetUnit(enemy.Id);
                unit.SetHp(enemy.CurrentHp, enemy.MaxHp);
                if (enemy.IsDead) unit.SetDead();
            }
        }

        private void UpdateActionOrderUI()
        {
            var order = _useCase.GetPredictedActionOrder(8);
            var runtimeData = _useCase.RuntimeData;
            if (runtimeData == null) return;

            var entries = new ActionOrderEntry[order.Count];
            for (int i = 0; i < order.Count; i++)
            {
                var participant = FindParticipantById(order[i]);
                entries[i] = new ActionOrderEntry
                {
                    Id = participant.Id,
                    SpriteKey = participant.SpriteKey,
                    IsAlly = participant.IsAlly
                };
            }

            _view.ActionOrder.SetOrder(entries);
        }

        private BattleParticipant FindParticipantById(int id)
        {
            var runtimeData = _useCase.RuntimeData;

            for (int i = 0; i < runtimeData.Allies.Count; i++)
            {
                if (runtimeData.Allies[i].Id == id)
                    return runtimeData.Allies[i];
            }

            for (int i = 0; i < runtimeData.Enemies.Count; i++)
            {
                if (runtimeData.Enemies[i].Id == id)
                    return runtimeData.Enemies[i];
            }

            throw new InvalidOperationException($"{_logClass} Participant not found: {id}");
        }

        private SkillDisplayData[] BuildSkillDisplayData(BattleParticipant actor, List<int> usableSkillIds)
        {
            var displays = new SkillDisplayData[actor.SkillIds.Length];
            var usableSet = new HashSet<int>(usableSkillIds);

            for (int i = 0; i < actor.SkillIds.Length; i++)
            {
                int skillId = actor.SkillIds[i];
                var skillSO = _skillMasterDataRepo.GetSkill(skillId);
                actor.SkillCooldowns.TryGetValue(skillId, out int cooldown);

                displays[i] = new SkillDisplayData
                {
                    SkillId = skillId,
                    SpriteKey = skillSO.IconSpriteKey,
                    CooldownRemaining = cooldown,
                    IsUsable = usableSet.Contains(skillId)
                };
            }

            return displays;
        }
    }
}
