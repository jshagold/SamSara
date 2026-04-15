using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Core.AssetLoading;
using Samsara.Core.MasterData;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.BattleScene.Domain;
using Samsara.Features.BattleScene.Presentation.Field;
using Samsara.Features.BattleScene.Presentation.Skill;
using Samsara.Features.Skill.Domain;
using UnityEngine;

namespace Samsara.Features.BattleScene.Presentation
{
    public class BattlePresenter
    {
        private readonly string _logClass = $"[{nameof(BattlePresenter)}]";

        private readonly BattleUseCase              _useCase;
        private readonly BattleView                 _view;
        private readonly ISceneNavigator            _sceneNavigator;
        private readonly IPopupManager              _popupManager;
        private readonly ISkillMasterDataRepository _skillMasterDataRepo;
        private readonly GameContext                _gameContext;
        private readonly BattleEventHookRunner      _hookRunner;
        private readonly ISpriteLoader              _spriteLoader;

        // 초상화 캐시 — participantId → Sprite
        private readonly Dictionary<int, Sprite> _portraitCache = new Dictionary<int, Sprite>();

        // ── 통합 아군 입력 TCS ──
        private enum AllyInputType { Skill, Target, Wait, Confirm }
        private struct AllyInput { public AllyInputType Type; public int Value; }
        private UniTaskCompletionSource<AllyInput> _allyInputTcs;

        // ── 결과 확인 TCS ──
        private UniTaskCompletionSource _resultConfirmTcs;

        // ── 이벤트 핸들러 캐시 ──
        private Action<int> _skillSelectedHandler;
        private Action<int> _targetSelectedHandler;
        private Action _waitSelectedHandler;
        private Action _confirmHandler;
        private Action _resultConfirmHandler;
        private Action _optionClickedHandler;
        private Action<int> _queueSlotTouchedHandler;
        private Action<int> _enemyLongPressHandler;
        private Action<int> _allyLongPressHandler;
        private Action<int> _skillLongPressHandler;

        // ── 전투 연출 딜레이 상수 ──
        private const float EnemyTurnShowDelay = 1.0f;  // 적 행동 텍스트 표시 후 공격 모션 전 대기
        private const float PostDamageDelay    = 0.5f;  // 데미지 표시 후 플레이어 결과 확인 대기

        public BattlePresenter(
            BattleUseCase              useCase,
            BattleView                 view,
            ISceneNavigator            sceneNavigator,
            IPopupManager              popupManager,
            ISkillMasterDataRepository skillMasterDataRepo,
            GameContext                gameContext,
            BattleEventHookRunner      hookRunner,
            ISpriteLoader              spriteLoader)
        {
            _useCase             = useCase;
            _view                = view;
            _sceneNavigator      = sceneNavigator;
            _popupManager        = popupManager;
            _skillMasterDataRepo = skillMasterDataRepo;
            _gameContext         = gameContext;
            _hookRunner          = hookRunner;
            _spriteLoader        = spriteLoader;
        }

        public void Initialize(PendingBattleContext context)
        {
            _useCase.InitializeBattle(context);

            var runtimeData = _useCase.RuntimeData;

            // 필드 렌더링 (EnemyFieldView에서 displayName 번호 부여 포함)
            _view.AllyField.RenderAllies(runtimeData.Allies.ToArray());
            _view.EnemyField.RenderEnemies(runtimeData.Enemies.ToArray());

            // 초기 UI
            UpdateActionOrderUI();
            _view.TurnNumber.SetTurn(1);
            _view.HideSkillUI();
            _view.EnemyField.EnableTargetSelection(false);
            _view.Confirm.SetInteractable(false);

            // 이벤트 구독
            _skillSelectedHandler   = skillId => _allyInputTcs?.TrySetResult(new AllyInput { Type = AllyInputType.Skill,   Value = skillId });
            _targetSelectedHandler  = id      => _allyInputTcs?.TrySetResult(new AllyInput { Type = AllyInputType.Target,  Value = id });
            _waitSelectedHandler    = ()      => _allyInputTcs?.TrySetResult(new AllyInput { Type = AllyInputType.Wait,    Value = 0 });
            _confirmHandler         = ()      => _allyInputTcs?.TrySetResult(new AllyInput { Type = AllyInputType.Confirm, Value = 0 });
            _resultConfirmHandler   = HandleResultConfirm;
            _optionClickedHandler   = HandleOptionClicked;
            _queueSlotTouchedHandler = HandleQueueSlotTouched;
            _enemyLongPressHandler  = id => HandleCharacterLongPress(id, isAlly: false);
            _allyLongPressHandler   = id => HandleCharacterLongPress(id, isAlly: true);
            _skillLongPressHandler  = HandleSkillLongPress;

            _view.OnSkillSelected     += _skillSelectedHandler;
            _view.OnTargetSelected    += _targetSelectedHandler;
            _view.OnWaitSelected      += _waitSelectedHandler;
            _view.OnConfirmPressed    += _confirmHandler;
            _view.OnResultConfirm     += _resultConfirmHandler;
            _view.OnOptionClicked     += _optionClickedHandler;
            _view.OnQueueSlotTouched  += _queueSlotTouchedHandler;
            _view.EnemyField.OnLongPress += _enemyLongPressHandler;
            _view.AllyField.OnLongPress  += _allyLongPressHandler;
            _view.OnSkillLongPress    += _skillLongPressHandler;

            // 배틀 루프 시작 (스프라이트 로드 포함)
            RunBattleLoopAsync().Forget();

            Debug.Log($"{_logClass} Initialize 완료.");
        }

        public void Dispose()
        {
            _view.OnSkillSelected     -= _skillSelectedHandler;
            _view.OnTargetSelected    -= _targetSelectedHandler;
            _view.OnWaitSelected      -= _waitSelectedHandler;
            _view.OnConfirmPressed    -= _confirmHandler;
            _view.OnResultConfirm     -= _resultConfirmHandler;
            _view.OnOptionClicked     -= _optionClickedHandler;
            _view.OnQueueSlotTouched  -= _queueSlotTouchedHandler;
            _view.EnemyField.OnLongPress -= _enemyLongPressHandler;
            _view.AllyField.OnLongPress  -= _allyLongPressHandler;
            _view.OnSkillLongPress    -= _skillLongPressHandler;

            _allyInputTcs?.TrySetCanceled();
            _resultConfirmTcs?.TrySetCanceled();

            Debug.Log($"{_logClass} Dispose 완료.");
        }

        // ──────────────────────────────────────────────
        // Sprite Preload
        // ──────────────────────────────────────────────

        private async UniTask PreloadSpritesAsync()
        {
            var runtimeData = _useCase.RuntimeData;
            if (runtimeData == null) return;

            // 아군 스프라이트 + 초상화
            foreach (var ally in runtimeData.Allies)
            {
                var unitSprite = await _spriteLoader.LoadSpriteAsync(ally.SpriteKey);
                _view.AllyField.GetUnit(ally.Id).SetSprite(unitSprite);

                var portrait = await _spriteLoader.LoadSpriteAsync(ally.PortraitSpriteKey);
                _portraitCache[ally.Id] = portrait;
            }

            // 적 스프라이트 + 초상화
            foreach (var enemy in runtimeData.Enemies)
            {
                var unitSprite = await _spriteLoader.LoadSpriteAsync(enemy.SpriteKey);
                _view.EnemyField.GetUnit(enemy.Id).SetSprite(unitSprite);

                var portrait = await _spriteLoader.LoadSpriteAsync(enemy.PortraitSpriteKey);
                _portraitCache[enemy.Id] = portrait;
            }

            // 행동 순서 초상화 적용
            ApplyPortraitsToActionOrder();
        }

        private void ApplyPortraitsToActionOrder()
        {
            foreach (var kvp in _portraitCache)
            {
                _view.ActionOrder.SetSlotPortrait(kvp.Key, kvp.Value);
            }
        }

        // ──────────────────────────────────────────────
        // Battle Loop (Plan §6-2)
        // ──────────────────────────────────────────────

        private async UniTaskVoid RunBattleLoopAsync()
        {
            // 스프라이트 사전 로드
            await PreloadSpritesAsync();

            // PreBattle 훅 + 시작 연출
            await _hookRunner.CheckHook(BattleHookType.PreBattle, _useCase.RuntimeData);
            await _view.BattleStart.PlayStartPresentation();

            while (true)
            {
                // ── Tick until someone is ready ──
                List<BattleParticipant> readyQueue;
                do
                {
                    readyQueue = _useCase.ProcessTick();
                    if (readyQueue.Count == 0)
                        await UniTask.Yield();
                } while (readyQueue.Count == 0);

                // PerTick 훅
                await _hookRunner.CheckHook(BattleHookType.PerTick, _useCase.RuntimeData);

                // ── 틱 시작 시 행동 큐 확정 (비어있을 때만) ──
                if (_useCase.IsCommittedQueueEmpty)
                    _useCase.BuildCommittedQueue();

                // ── 각 액터 처리 ──
                for (int i = 0; i < readyQueue.Count; i++)
                {
                    var actor = readyQueue[i];
                    if (actor.IsDead) continue;

                    _useCase.ConsumeGauge(actor);
                    _useCase.ConsumeActorFromCommittedQueue(actor.Id);  // 현재 액터를 큐에서 소비

                    // 액티브 하이라이트 ON + 행동 순서 UI 갱신
                    SetActiveHighlight(actor, true);
                    UpdateActionOrderUI(actor);

                    if (actor.IsAlly)
                        await ProcessAllyTurn(actor);
                    else
                        await ProcessEnemyTurn(actor);

                    // 액티브 하이라이트 OFF
                    SetActiveHighlight(actor, false);

                    _useCase.ReduceCooldowns(actor);
                    UpdateAllUnitHP();
                    _useCase.PruneDeadFromCommittedQueue();  // 사망 항목만 제거, 나머지 순서 유지
                    UpdateActionOrderUI();

                    // PostDamage 훅
                    await _hookRunner.CheckHook(BattleHookType.PostDamage, _useCase.RuntimeData);

                    // 전투 종료 확인
                    var result = _useCase.CheckBattleEnd();
                    if (result.HasValue)
                    {
                        await HandleBattleEnd(result.Value);
                        return;
                    }
                }

                // 라운드 종료
                _useCase.IncrementTurn();
                _view.TurnNumber.SetTurn(_useCase.RuntimeData.TurnNumber);
            }
        }

        // ──────────────────────────────────────────────
        // Ally Turn
        // ──────────────────────────────────────────────

        private async UniTask ProcessAllyTurn(BattleParticipant actor)
        {
            // 스킬 데이터 구성 및 스킬 UI 표시
            var usableSkills = _useCase.GetUsableSkills(actor);
            var skillDisplays = await BuildSkillDisplayDataAsync(actor, usableSkills);
            _view.SkillSelection.SetSkills(skillDisplays);
            _view.ShowSkillUI();
            _view.Confirm.SetInteractable(false);
            _view.EnemyField.EnableTargetSelection(false);

            // ── 입력 루프: 스킬/타겟/Wait/Confirm ──
            int selectedSkillId = -2;   // -2 = 미선택
            int selectedTargetId = -1;  // -1 = 미선택
            bool waitChosen = false;

            while (true)
            {
                var input = await WaitForNextAllyInput();

                if (input.Type == AllyInputType.Wait)
                {
                    waitChosen = true;
                    break;
                }

                if (input.Type == AllyInputType.Skill)
                {
                    selectedSkillId = input.Value;
                    selectedTargetId = -1;  // 스킬 재선택 시 타겟 초기화
                    _view.EnemyField.EnableTargetSelection(true);
                    _view.Confirm.SetInteractable(false);
                    continue;
                }

                if (input.Type == AllyInputType.Target)
                {
                    selectedTargetId = input.Value;
                    bool canConfirm = selectedSkillId >= -1 && selectedTargetId >= 0;
                    _view.Confirm.SetInteractable(canConfirm);
                    continue;
                }

                if (input.Type == AllyInputType.Confirm)
                {
                    if (selectedSkillId != -2 && selectedTargetId != -1)
                        break;  // 확정
                    // 조건 미충족 시 무시
                }
            }

            _view.EnemyField.EnableTargetSelection(false);
            _view.Confirm.SetInteractable(false);
            _view.HideSkillUI();

            // Wait 선택 시 처리 없이 종료 (ConsumeGauge/ReduceCooldowns는 메인 루프에서 처리)
            if (waitChosen)
            {
                await UniTask.Delay(300);
                return;
            }

            // ── 공격 실행 ──
            var target = FindParticipantById(selectedTargetId);

            // QTE 패널 슬라이드인
            await _view.TransitionToQTE(isDefense: false);

            // 공격 모션
            var actorUnit = _view.AllyField.GetUnit(actor.Id);
            var targetUnit = _view.EnemyField.GetUnit(selectedTargetId);
            await actorUnit.PlayAttackMotion(targetUnit.transform.position);

            // QTE → 데미지
            if (selectedSkillId != BattleUseCase.DefaultAttackId)
            {
                var skillSO = _skillMasterDataRepo.GetSkill(selectedSkillId);
                if (skillSO.QtePatternId > 0)
                {
                    var qtePattern = _skillMasterDataRepo.GetQTEPattern(skillSO.QtePatternId);
                    await ExecuteAttackWithQTE(actor, selectedSkillId, target, qtePattern.QteDataList, actorUnit, targetUnit);
                }
                else
                {
                    int damage = _useCase.ExecuteAction(actor, selectedSkillId, target, 1.0f);
                    _view.DamagePopup.ShowHitDamage(damage, true, targetUnit.transform.position);
                }
            }
            else
            {
                // 기본 공격 (QTE 없음)
                int damage = _useCase.ExecuteAction(actor, selectedSkillId, target, 1.0f);
                _view.DamagePopup.ShowHitDamage(damage, true, targetUnit.transform.position);
            }

            await UniTask.Delay((int)(PostDamageDelay * 1000));

            if (target.IsDead)
                targetUnit.SetDead();

            // 귀환 모션
            await actorUnit.PlayReturnMotion();

            // 스킬 UI로 전환
            await _view.TransitionToSkillUI();
        }

        /// <summary>QTE 링 per-hit 실행 + 데미지 계산 + 팝업 표시.</summary>
        private async UniTask ExecuteAttackWithQTE(
            BattleParticipant actor, int skillId, BattleParticipant target,
            QTEData[] qteDataList,
            CharacterUnitView actorUnit,
            CharacterUnitView targetUnit)
        {
            var qteResults = new bool[qteDataList.Length];

            // Per-hit QTE 실행
            for (int j = 0; j < qteDataList.Length; j++)
            {
                qteResults[j] = await _view.BattleQTE.RunSingleRing(qteDataList[j]);

                if (j < qteDataList.Length - 1 && qteDataList[j].IntervalToNext > 0f)
                    await UniTask.Delay((int)(qteDataList[j].IntervalToNext * 1000f));
            }

            // QTE 결과로 최종 데미지 계산
            float qteRate = _useCase.CalculateQTERate(true, qteResults, qteResults.Length);
            int totalDamage = _useCase.ExecuteAction(actor, skillId, target, qteRate);

            // Per-hit 데미지 분배 및 팝업 표시
            int hitCount = Mathf.Max(1, qteDataList.Length);
            int[] perHitDamages = _useCase.CalculatePerHitDamage(totalDamage, hitCount);
            for (int j = 0; j < perHitDamages.Length; j++)
            {
                _view.DamagePopup.ShowHitDamage(perHitDamages[j], qteResults[j], targetUnit.transform.position);
                if (j < perHitDamages.Length - 1)
                    await UniTask.Delay(100);
            }
        }

        // ──────────────────────────────────────────────
        // Enemy Turn
        // ──────────────────────────────────────────────

        private async UniTask ProcessEnemyTurn(BattleParticipant actor)
        {
            string actorName = !string.IsNullOrEmpty(actor.DisplayName) ? actor.DisplayName : actor.SpriteKey;

            // 스킬/타겟 선택 (패널 표시 전에 먼저 결정)
            int skillId = _useCase.SelectEnemySkill(actor);
            var target  = _useCase.SelectEnemyTarget(actor);
            if (target == null) return;

            // 하단 패널: 스킬 UI 숨김 + 적 턴 정보 표시
            string skillName = skillId != BattleUseCase.DefaultAttackId
                ? _skillMasterDataRepo.GetSkill(skillId).SkillName
                : "기본 공격";
            _view.ShowEnemyTurnPanel($"적 {actorName}{KoreanSubjectParticle(actorName)} {skillName} 사용");
            await UniTask.Delay((int)(EnemyTurnShowDelay * 1000));

            // 공격 모션 (적 → 아군)
            var actorUnit  = _view.EnemyField.GetUnit(actor.Id);
            var targetUnit = _view.AllyField.GetUnit(target.Id);
            await actorUnit.PlayAttackMotion(targetUnit.transform.position);

            // 방어 QTE (isDefense=true)
            float qteRate = 1.0f;
            if (skillId != BattleUseCase.DefaultAttackId)
            {
                var skillSO = _skillMasterDataRepo.GetSkill(skillId);
                if (skillSO.QtePatternId > 0)
                {
                    var qtePattern  = _skillMasterDataRepo.GetQTEPattern(skillSO.QtePatternId);
                    var qteDataList = qtePattern.QteDataList;

                    // QTE 전환 전 패널 숨김 (QTE 패널이 같은 영역에 슬라이드인)
                    _view.HideEnemyTurnPanel();
                    await _view.TransitionToQTE(isDefense: true);

                    var qteResults = new bool[qteDataList.Length];
                    for (int j = 0; j < qteDataList.Length; j++)
                    {
                        qteResults[j] = await _view.BattleQTE.RunSingleRing(qteDataList[j]);
                        if (j < qteDataList.Length - 1 && qteDataList[j].IntervalToNext > 0f)
                            await UniTask.Delay((int)(qteDataList[j].IntervalToNext * 1000f));
                    }

                    await _view.BattleQTE.SlideOut();
                    qteRate = _useCase.CalculateQTERate(false, qteResults, qteResults.Length);

                    // 방어 QTE per-hit 데미지
                    int totalDamage = _useCase.ExecuteAction(actor, skillId, target, qteRate);
                    int[] perHitDamages = _useCase.CalculatePerHitDamage(totalDamage, Mathf.Max(1, qteDataList.Length));
                    for (int j = 0; j < perHitDamages.Length; j++)
                    {
                        // 방어 성공 = 감소 데미지(success 시각), 방어 실패 = 일반 데미지(fail 시각)
                        _view.DamagePopup.ShowHitDamage(perHitDamages[j], qteResults[j], targetUnit.transform.position);
                        if (j < perHitDamages.Length - 1)
                            await UniTask.Delay(100);
                    }
                }
                else
                {
                    // QTE 없는 스킬: 무조건 적중
                    int damage = _useCase.ExecuteAction(actor, skillId, target, qteRate);
                    _view.DamagePopup.ShowHitDamage(damage, true, targetUnit.transform.position);
                }
            }
            else
            {
                // 기본 공격: 무조건 적중
                int damage = _useCase.ExecuteAction(actor, skillId, target, qteRate);
                _view.DamagePopup.ShowHitDamage(damage, true, targetUnit.transform.position);
            }

            await UniTask.Delay((int)(PostDamageDelay * 1000));

            if (target.IsDead)
                targetUnit.SetDead();

            // 귀환 모션
            await actorUnit.PlayReturnMotion();

            // 패널 닫기 (QTE 경로는 이미 닫혔으나 non-QTE 경로를 위해 통일 처리)
            _view.HideEnemyTurnPanel();
            await UniTask.Delay(300);
        }

        /// <summary>
        /// 한국어 주격 조사 반환. 이름 마지막 글자의 받침 유무에 따라 "이" 또는 "가".
        /// 한국어 음절 범위(AC00-D7A3) 외 문자(영문 등)는 "이" 반환.
        /// </summary>
        private static string KoreanSubjectParticle(string name)
        {
            if (string.IsNullOrEmpty(name)) return "이";
            char last = name[name.Length - 1];
            if (last >= 0xAC00 && last <= 0xD7A3)
                return (last - 0xAC00) % 28 == 0 ? "가" : "이";
            return "이";
        }

        // ──────────────────────────────────────────────
        // Battle End
        // ──────────────────────────────────────────────

        private async UniTask HandleBattleEnd(BattleResult result)
        {
            await _hookRunner.CheckHook(BattleHookType.PostBattle, _useCase.RuntimeData);

            _useCase.CleanupBattle();

            await _view.BattleResultPopup.ShowEndPresentation(result);
            await WaitForResultConfirm();

            _gameContext.LastBattleResult = result;
            _gameContext.PendingBattleContext = null;

            await _sceneNavigator.NavigateToAsync(SceneKey.Stage);
        }

        // ──────────────────────────────────────────────
        // Await Helpers
        // ──────────────────────────────────────────────

        private UniTask<AllyInput> WaitForNextAllyInput()
        {
            _allyInputTcs = new UniTaskCompletionSource<AllyInput>();
            return _allyInputTcs.Task;
        }

        private UniTask WaitForResultConfirm()
        {
            _resultConfirmTcs = new UniTaskCompletionSource();
            return _resultConfirmTcs.Task;
        }

        // ──────────────────────────────────────────────
        // Event Handlers
        // ──────────────────────────────────────────────

        private void HandleResultConfirm()
        {
            _resultConfirmTcs?.TrySetResult();
        }

        private void HandleOptionClicked()
        {
            Debug.Log($"{_logClass} Option 클릭 — 미구현.");
        }

        private void HandleQueueSlotTouched(int participantId)
        {
            // 큐 슬롯 터치 → 해당 필드 캐릭터 타겟 하이라이트 토글
            var runtimeData = _useCase.RuntimeData;
            if (runtimeData == null) return;

            // 적 필드 탐색
            for (int i = 0; i < runtimeData.Enemies.Count; i++)
            {
                if (runtimeData.Enemies[i].Id == participantId && !runtimeData.Enemies[i].IsDead)
                {
                    var unit = _view.EnemyField.GetUnit(participantId);
                    unit.SetHighlight(true);
                    return;
                }
            }

            // 아군 필드 탐색
            for (int i = 0; i < runtimeData.Allies.Count; i++)
            {
                if (runtimeData.Allies[i].Id == participantId && !runtimeData.Allies[i].IsDead)
                {
                    var unit = _view.AllyField.GetUnit(participantId);
                    unit.SetHighlight(true);
                    return;
                }
            }
        }

        private void HandleCharacterLongPress(int participantId, bool isAlly)
        {
            Debug.Log($"{_logClass} HandleCharacterLongPress id={participantId} isAlly={isAlly}");

            var runtimeData = _useCase.RuntimeData;
            if (runtimeData == null)
            {
                Debug.LogWarning($"{_logClass} HandleCharacterLongPress — RuntimeData is null, returning");
                return;
            }

            BattleParticipant p = FindParticipantByIdSafe(participantId);
            if (p == null)
            {
                Debug.LogWarning($"{_logClass} HandleCharacterLongPress — participant {participantId} not found, returning");
                return;
            }

            string name = !string.IsNullOrEmpty(p.DisplayName) ? p.DisplayName : p.SpriteKey;
            string detail = $"HP: {p.CurrentHp}/{p.MaxHp}\nSTR: {p.Strength}  TGH: {p.Toughness}  AGI: {p.Agility}";

            Debug.Log($"{_logClass} → InfoTooltip.Show({name})");
            // 화면 중앙 근처에 툴팁 표시
            _view.InfoTooltip.Show(name, detail, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        }

        private void HandleSkillLongPress(int skillId)
        {
            Debug.Log($"{_logClass} HandleSkillLongPress skillId={skillId}");
            var skillSO = _skillMasterDataRepo.GetSkill(skillId);
            Debug.Log($"{_logClass} → InfoTooltip.Show({skillSO?.SkillName})");
            _view.InfoTooltip.Show(skillSO.SkillName, skillSO.Description,
                new Vector2(Screen.width * 0.5f, Screen.height * 0.3f));
        }

        // ──────────────────────────────────────────────
        // UI Update Helpers
        // ──────────────────────────────────────────────

        private void SetActiveHighlight(BattleParticipant actor, bool on)
        {
            if (actor.IsAlly)
            {
                var unit = _view.AllyField.GetUnit(actor.Id);
                unit.SetActiveHighlight(on);
            }
            else
            {
                var unit = _view.EnemyField.GetUnit(actor.Id);
                unit.SetActiveHighlight(on);
            }
        }

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

        private void UpdateActionOrderUI(BattleParticipant currentActor = null)
        {
            var runtimeData = _useCase.RuntimeData;
            if (runtimeData == null) return;

            const int maxSlots = 5;
            int lookAhead = currentActor != null ? maxSlots - 1 : maxSlots;

            // 큐가 비어있을 때만 재계산 (전투 초기화 또는 사망 후 무효화 직후)
            // 정상 플레이 중 순서가 바뀌지 않도록 큐는 500개로 선계산됨
            if (_useCase.IsCommittedQueueEmpty)
                _useCase.BuildCommittedQueue();
            var predicted = _useCase.PeekCommittedQueue(lookAhead);

            BattleParticipant[] ordered;
            if (currentActor != null)
            {
                ordered = new BattleParticipant[1 + predicted.Length];
                ordered[0] = currentActor;
                Array.Copy(predicted, 0, ordered, 1, predicted.Length);
            }
            else
            {
                ordered = predicted;
            }

            _view.ActionOrder.SetOrder(ordered);
            if (currentActor != null)
                _view.ActionOrder.HighlightCurrent(currentActor.Id);

            // 캐시된 초상화 적용
            ApplyPortraitsToActionOrder();
        }

        private async UniTask<SkillDisplayData[]> BuildSkillDisplayDataAsync(BattleParticipant actor, List<int> usableSkillIds)
        {
            var displays = new SkillDisplayData[actor.SkillIds.Length];
            var usableSet = new HashSet<int>(usableSkillIds);

            for (int i = 0; i < actor.SkillIds.Length; i++)
            {
                int skillId = actor.SkillIds[i];
                var skillSO = _skillMasterDataRepo.GetSkill(skillId);
                actor.SkillCooldowns.TryGetValue(skillId, out int cooldown);

                SkillState state;
                if (cooldown > 0)
                    state = SkillState.OnCooldown;
                else if (!usableSet.Contains(skillId))
                    state = SkillState.HpInsufficient;
                else
                    state = SkillState.Usable;

                Sprite icon = null;
                if (!string.IsNullOrEmpty(skillSO.IconSpriteKey))
                    icon = await _spriteLoader.LoadSpriteAsync(skillSO.IconSpriteKey);

                displays[i] = new SkillDisplayData
                {
                    SkillId = skillId,
                    SpriteKey = skillSO.IconSpriteKey,
                    Icon = icon,
                    CooldownRemaining = cooldown,
                    IsUsable = state == SkillState.Usable,
                    State = state
                };
            }

            return displays;
        }

        private BattleParticipant FindParticipantById(int id)
        {
            var runtimeData = _useCase.RuntimeData;
            for (int i = 0; i < runtimeData.Allies.Count; i++)
                if (runtimeData.Allies[i].Id == id) return runtimeData.Allies[i];
            for (int i = 0; i < runtimeData.Enemies.Count; i++)
                if (runtimeData.Enemies[i].Id == id) return runtimeData.Enemies[i];
            throw new InvalidOperationException($"{_logClass} Participant not found: {id}");
        }

        private BattleParticipant FindParticipantByIdSafe(int id)
        {
            var runtimeData = _useCase.RuntimeData;
            if (runtimeData == null) return null;
            for (int i = 0; i < runtimeData.Allies.Count; i++)
                if (runtimeData.Allies[i].Id == id) return runtimeData.Allies[i];
            for (int i = 0; i < runtimeData.Enemies.Count; i++)
                if (runtimeData.Enemies[i].Id == id) return runtimeData.Enemies[i];
            return null;
        }
    }
}
