using System;
using System.Collections.Generic;
using Samsara.Core.MasterData;
using Samsara.Features.Character.Domain;
using Samsara.Features.Skill.Domain;
using Samsara.Features.Stage.Domain;
using UnityEngine;

namespace Samsara.Features.BattleScene.Domain
{
    public class BattleUseCase
    {
        private readonly string _logClass = $"[{nameof(BattleUseCase)}]";

        private readonly ISkillMasterDataRepository _skillMasterDataRepo;
        private readonly ICharacterRunRepository _characterRunRepo;
        private readonly IStageMasterDataRepository _stageMasterDataRepo;

        private BattleRuntimeData _runtimeData;
        private int _nextParticipantId;

        // Pre-allocated buffers (§8 GC optimization — ProcessTick is called every tick)
        private readonly List<BattleParticipant> _tickReadyBuffer = new List<BattleParticipant>();
        private readonly List<int> _usableSkillsBuffer = new List<int>();

        // ── 행동 순서 큐 설계 상수 ──
        // QueueBuildCount = 참가자 최대 수 × 전투 예상 최대 턴 수
        // 정상 플레이 중 큐가 소진되지 않도록 충분히 크게 설정.
        // 사망/이탈 없이 이 값을 초과하는 행동이 발생하면 그때 재계산 허용.
        private const int MaxParticipants = 6;   // 최대 참가자 수 (1아군 + 5적 기준)
        private const int MaxBattleTurns  = 100;  // 예상 최대 전투 턴 수 (안전 마진 포함)
        private const int QueueBuildCount = MaxParticipants * MaxBattleTurns; // = 600
        private const int MinQueueDisplay = 5;    // ActionOrderView 슬롯 수 (UI 최소 표시량)

        // Pre-allocated simulation buffers for GetPredictedActionOrder (§8 GC)
        private readonly BattleParticipant[] _simParticipants = new BattleParticipant[20];
        private readonly float[] _simGauges = new float[20];
        private readonly BattleParticipant[] _predictedResultBuffer = new BattleParticipant[QueueBuildCount];
        private readonly int[] _simReadyIndices = new int[20];
        private int _simParticipantCount;
        private int _simReadyCount;

        // Committed action queue — 선계산 후 사망/이탈 시 부분 수정만 허용
        private readonly List<BattleParticipant> _committedQueue = new List<BattleParticipant>(QueueBuildCount);

        public bool IsCommittedQueueEmpty => _committedQueue.Count == 0;

        public static readonly int DefaultAttackId = -1;

        public BattleRuntimeData RuntimeData => _runtimeData;

        public BattleUseCase(
            ISkillMasterDataRepository skillMasterDataRepo,
            ICharacterRunRepository characterRunRepo,
            IStageMasterDataRepository stageMasterDataRepo)
        {
            _skillMasterDataRepo = skillMasterDataRepo;
            _characterRunRepo = characterRunRepo;
            _stageMasterDataRepo = stageMasterDataRepo;
        }

        // ──────────────────────────────────────────────
        // InitializeBattle
        // ──────────────────────────────────────────────

        public void InitializeBattle(PendingBattleContext context)
        {
            _nextParticipantId = 1;
            _runtimeData = new BattleRuntimeData
            {
                Allies = new List<BattleParticipant>(),
                Enemies = new List<BattleParticipant>(),
                TurnNumber = 1,
                CurrentPhase = BattlePhase.SkillSelect
            };

            // --- Ally setup ---
            var runData = _characterRunRepo.RunData;
            var evolutionNode = _stageMasterDataRepo.GetEvolutionNodeById(runData.EvolutionNodeId);

            var ally = new BattleParticipant
            {
                Id = _nextParticipantId++,
                IsAlly = true,
                CurrentHp = runData.Hp,
                MaxHp = runData.MaxHp,
                Strength = runData.Strength,
                Toughness = runData.Toughness,
                Agility = runData.Agility,
                ActionGauge = 0f,
                SkillIds = evolutionNode.SkillIds,
                SkillCooldowns = new Dictionary<int, int>(),
                IsDead = false,
                SpriteKey = evolutionNode.BattleSpriteKeyHp100,
                PortraitSpriteKey = evolutionNode.PortraitSpriteKey,
                DisplayName = evolutionNode.CharacterName
            };

            foreach (var skillId in ally.SkillIds)
                ally.SkillCooldowns[skillId] = 0;

            _runtimeData.Allies.Add(ally);

            // --- Enemy setup ---
            var enemySpawns = context.BattleNodeData.EnemySpawns;
            Debug.Log($"{_logClass} [DEBUG] EnemySpawns={(enemySpawns == null ? "null" : enemySpawns.Length.ToString())}");
            if (enemySpawns != null)
            {
                for (int d = 0; d < enemySpawns.Length; d++)
                    Debug.Log($"{_logClass} [DEBUG] Spawn[{d}]: EnemyId={enemySpawns[d].EnemyId}, Count={enemySpawns[d].Count}");
            }
            foreach (var spawn in enemySpawns)
            {
                var enemySO = _stageMasterDataRepo.GetEnemyById(spawn.EnemyId);
                for (int i = 0; i < spawn.Count; i++)
                {
                    var enemy = new BattleParticipant
                    {
                        Id = _nextParticipantId++,
                        IsAlly = false,
                        CurrentHp = enemySO.BaseStats.Hp,
                        MaxHp = enemySO.BaseStats.Hp,
                        Strength = enemySO.BaseStats.Strength,
                        Toughness = enemySO.BaseStats.Toughness,
                        Agility = enemySO.BaseStats.Agility,
                        ActionGauge = 0f,
                        SkillIds = enemySO.SkillIds,
                        SkillCooldowns = new Dictionary<int, int>(),
                        IsDead = false,
                        SpriteKey = enemySO.BattleSpriteKeyHp100,
                        PortraitSpriteKey = enemySO.BattleSpriteKeyHp100,  // D-13: EnemySO has no portrait key
                        DisplayName = enemySO.EnemyName
                    };

                    foreach (var skillId in enemy.SkillIds)
                        enemy.SkillCooldowns[skillId] = 0;

                    _runtimeData.Enemies.Add(enemy);
                }
            }

            Debug.Log($"{_logClass} Battle initialized — Allies:{_runtimeData.Allies.Count}, Enemies:{_runtimeData.Enemies.Count}");
        }

        // ──────────────────────────────────────────────
        // ProcessTick
        // ──────────────────────────────────────────────

        public List<BattleParticipant> ProcessTick()
        {
            _tickReadyBuffer.Clear();

            for (int i = 0; i < _runtimeData.Allies.Count; i++)
            {
                var p = _runtimeData.Allies[i];
                if (p.IsDead) continue;
                p.ActionGauge += p.Agility;
                if (p.ActionGauge >= 100f)
                    _tickReadyBuffer.Add(p);
            }

            for (int i = 0; i < _runtimeData.Enemies.Count; i++)
            {
                var p = _runtimeData.Enemies[i];
                if (p.IsDead) continue;
                p.ActionGauge += p.Agility;
                if (p.ActionGauge >= 100f)
                    _tickReadyBuffer.Add(p);
            }

            // Sort by actionGauge descending; deterministic tiebreak by Id (lower Id = earlier)
            // Random tiebreak was removed: committed queue uses insertion-order tiebreak
            // (allies added before enemies → lower Id first), so both sorts must agree.
            _tickReadyBuffer.Sort((a, b) =>
            {
                int cmp = b.ActionGauge.CompareTo(a.ActionGauge);
                if (cmp != 0) return cmp;
                return a.Id.CompareTo(b.Id);
            });

            return _tickReadyBuffer;
        }

        // ──────────────────────────────────────────────
        // ExecuteAction
        // ──────────────────────────────────────────────

        public int ExecuteAction(BattleParticipant actor, int skillId, BattleParticipant target, float qteRate)
        {
            int finalDamage;

            if (skillId == DefaultAttackId)
            {
                // Default attack (no skill): strength vs toughness, multiplier 1.0
                float baseDamage = actor.Strength;
                float defense = target.Toughness;
                finalDamage = Mathf.Max(1, Mathf.FloorToInt((baseDamage - defense) * qteRate));
            }
            else
            {
                var skillSO = _skillMasterDataRepo.GetSkill(skillId);
                float baseDamage = actor.Strength * skillSO.Damage;
                float defense = target.Toughness;
                finalDamage = Mathf.Max(1, Mathf.FloorToInt((baseDamage - defense) * qteRate));

                // Apply cooldown from CostType.CoolDown
                if (skillSO.Costs != null)
                {
                    foreach (var cost in skillSO.Costs)
                    {
                        if (cost.CostType == CostType.CoolDown)
                        {
                            actor.SkillCooldowns[skillId] = (int)cost.Value;
                        }
                        else if (cost.CostType == CostType.Hp)
                        {
                            actor.CurrentHp -= (int)cost.Value;
                            if (actor.CurrentHp <= 0)
                            {
                                actor.CurrentHp = 0;
                                actor.IsDead = true;
                            }
                        }
                    }
                }
            }

            target.CurrentHp -= finalDamage;
            if (target.CurrentHp <= 0)
            {
                target.CurrentHp = 0;
                target.IsDead = true;
            }

            return finalDamage;
        }

        // ──────────────────────────────────────────────
        // ReduceCooldowns
        // ──────────────────────────────────────────────

        public void ReduceCooldowns(BattleParticipant actor)
        {
            var keys = new List<int>(actor.SkillCooldowns.Keys);
            foreach (var key in keys)
            {
                int current = actor.SkillCooldowns[key];
                actor.SkillCooldowns[key] = Mathf.Max(0, current - 1);
            }
        }

        // ──────────────────────────────────────────────
        // ConsumeGauge
        // ──────────────────────────────────────────────

        public void ConsumeGauge(BattleParticipant actor)
        {
            actor.ActionGauge -= 100f;
        }

        // ──────────────────────────────────────────────
        // ExecuteWait
        // ──────────────────────────────────────────────

        public void ExecuteWait(BattleParticipant actor)
        {
            ReduceCooldowns(actor);
            ConsumeGauge(actor);
            IncrementTurn();
        }

        // ──────────────────────────────────────────────
        // CalculatePerHitDamage
        // ──────────────────────────────────────────────

        public int[] CalculatePerHitDamage(int totalDamage, int hitCount)
        {
            if (hitCount <= 0) return System.Array.Empty<int>();

            var damages = new int[hitCount];
            int perHit = Mathf.FloorToInt((float)totalDamage / hitCount);
            for (int i = 0; i < hitCount - 1; i++)
                damages[i] = perHit;
            damages[hitCount - 1] = totalDamage - (perHit * (hitCount - 1));
            return damages;
        }

        // ──────────────────────────────────────────────
        // GetUsableSkills
        // ──────────────────────────────────────────────

        public List<int> GetUsableSkills(BattleParticipant actor)
        {
            _usableSkillsBuffer.Clear();

            foreach (var skillId in actor.SkillIds)
            {
                // Check cooldown
                if (actor.SkillCooldowns.TryGetValue(skillId, out int cooldown) && cooldown > 0)
                    continue;

                // Check HP cost payable (must survive after paying)
                var skillSO = _skillMasterDataRepo.GetSkill(skillId);
                bool canPay = true;
                if (skillSO.Costs != null)
                {
                    foreach (var cost in skillSO.Costs)
                    {
                        if (cost.CostType == CostType.Hp && actor.CurrentHp <= (int)cost.Value)
                        {
                            canPay = false;
                            break;
                        }
                    }
                }

                if (canPay)
                    _usableSkillsBuffer.Add(skillId);
            }

            return _usableSkillsBuffer;
        }

        // ──────────────────────────────────────────────
        // SelectEnemySkill
        // ──────────────────────────────────────────────

        public int SelectEnemySkill(BattleParticipant enemy)
        {
            var usable = GetUsableSkills(enemy);
            if (usable.Count == 0)
                return DefaultAttackId;

            return usable[UnityEngine.Random.Range(0, usable.Count)];
        }

        // ──────────────────────────────────────────────
        // SelectEnemyTarget
        // ──────────────────────────────────────────────

        public BattleParticipant SelectEnemyTarget(BattleParticipant enemy)
        {
            // Phase 1: 1 ally, auto-select first non-dead ally
            for (int i = 0; i < _runtimeData.Allies.Count; i++)
            {
                if (!_runtimeData.Allies[i].IsDead)
                    return _runtimeData.Allies[i];
            }

            return null;
        }

        // ──────────────────────────────────────────────
        // CalculateQTERate
        // ──────────────────────────────────────────────

        public float CalculateQTERate(bool isAttack, bool[] inputResults, int inputCount)
        {
            int successCount = 0;
            for (int i = 0; i < inputResults.Length; i++)
            {
                if (inputResults[i]) successCount++;
            }

            float successRate = (float)successCount / inputCount;

            if (isAttack)
                return 1.0f + (0.5f * successRate);
            else
                return 1.0f - (0.5f * successRate);
        }

        // ──────────────────────────────────────────────
        // CheckBattleEnd
        // ──────────────────────────────────────────────

        public BattleResult? CheckBattleEnd()
        {
            bool allEnemiesDead = true;
            for (int i = 0; i < _runtimeData.Enemies.Count; i++)
            {
                if (!_runtimeData.Enemies[i].IsDead)
                {
                    allEnemiesDead = false;
                    break;
                }
            }
            if (allEnemiesDead) return BattleResult.Victory;

            bool allAlliesDead = true;
            for (int i = 0; i < _runtimeData.Allies.Count; i++)
            {
                if (!_runtimeData.Allies[i].IsDead)
                {
                    allAlliesDead = false;
                    break;
                }
            }
            if (allAlliesDead) return BattleResult.Defeat;

            return null;
        }

        // ──────────────────────────────────────────────
        // GetPredictedActionOrder
        // ──────────────────────────────────────────────

        /// <summary>
        /// 다음 lookAhead명의 행동 순서를 시뮬레이션하여 반환.
        /// 동일 틱 내 동점: actionGauge 높은 순.
        /// Pre-allocated 버퍼 사용 (§8 GC 최적화).
        /// </summary>
        public BattleParticipant[] GetPredictedActionOrder(int lookAhead = 4)
        {
            int resultCount = 0;
            _simParticipantCount = 0;

            for (int i = 0; i < _runtimeData.Allies.Count; i++)
            {
                if (!_runtimeData.Allies[i].IsDead && _simParticipantCount < _simParticipants.Length)
                {
                    _simParticipants[_simParticipantCount] = _runtimeData.Allies[i];
                    _simGauges[_simParticipantCount] = _runtimeData.Allies[i].ActionGauge;
                    _simParticipantCount++;
                }
            }
            for (int i = 0; i < _runtimeData.Enemies.Count; i++)
            {
                if (!_runtimeData.Enemies[i].IsDead && _simParticipantCount < _simParticipants.Length)
                {
                    _simParticipants[_simParticipantCount] = _runtimeData.Enemies[i];
                    _simGauges[_simParticipantCount] = _runtimeData.Enemies[i].ActionGauge;
                    _simParticipantCount++;
                }
            }

            if (_simParticipantCount == 0) return System.Array.Empty<BattleParticipant>();

            int maxResults = Mathf.Min(lookAhead, _predictedResultBuffer.Length);
            int safetyLimit = maxResults * 1000;
            int iterations = 0;

            while (resultCount < maxResults && iterations < safetyLimit)
            {
                iterations++;

                // Tick all gauges
                for (int i = 0; i < _simParticipantCount; i++)
                    _simGauges[i] += _simParticipants[i].Agility;

                // Collect ready indices
                _simReadyCount = 0;
                for (int i = 0; i < _simParticipantCount; i++)
                {
                    if (_simGauges[i] >= 100f)
                        _simReadyIndices[_simReadyCount++] = i;
                }

                if (_simReadyCount == 0) continue;

                // Insertion sort by gauge descending (small count — allocation-free)
                for (int i = 1; i < _simReadyCount; i++)
                {
                    int key = _simReadyIndices[i];
                    float keyGauge = _simGauges[key];
                    int j = i - 1;
                    while (j >= 0 && _simGauges[_simReadyIndices[j]] < keyGauge)
                    {
                        _simReadyIndices[j + 1] = _simReadyIndices[j];
                        j--;
                    }
                    _simReadyIndices[j + 1] = key;
                }

                // Add to result, consume gauge
                for (int i = 0; i < _simReadyCount && resultCount < maxResults; i++)
                {
                    int idx = _simReadyIndices[i];
                    _predictedResultBuffer[resultCount++] = _simParticipants[idx];
                    _simGauges[idx] -= 100f;
                }
            }

            var result = new BattleParticipant[resultCount];
            System.Array.Copy(_predictedResultBuffer, result, resultCount);
            return result;
        }

        // ──────────────────────────────────────────────
        // Committed Queue API
        // ──────────────────────────────────────────────

        /// <summary>현재 게이지 상태로 QueueBuildCount명분 행동 순서를 확정 큐에 저장.</summary>
        public void BuildCommittedQueue(int count = QueueBuildCount)
        {
            _committedQueue.Clear();
            var predicted = GetPredictedActionOrder(count);
            for (int i = 0; i < predicted.Length; i++)
                _committedQueue.Add(predicted[i]);
            Debug.Log($"{_logClass} 행동 큐 확정 ({_committedQueue.Count}명)");
        }

        /// <summary>행동을 완료한 액터를 큐에서 제거.</summary>
        public void ConsumeActorFromCommittedQueue(int actorId)
        {
            for (int i = 0; i < _committedQueue.Count; i++)
            {
                if (_committedQueue[i].Id == actorId)
                {
                    _committedQueue.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>다음 count명을 미리 보기 (UI 전용, 큐 소비 없음).</summary>
        public BattleParticipant[] PeekCommittedQueue(int count)
        {
            int take = Mathf.Min(count, _committedQueue.Count);
            var result = new BattleParticipant[take];
            for (int i = 0; i < take; i++)
                result[i] = _committedQueue[i];
            return result;
        }

        /// <summary>
        /// 사망/이탈한 참가자의 큐 항목만 제거한다.
        /// 남은 참가자들의 상대적 순서는 유지.
        /// 제거 후 MinQueueDisplay 미만이 되면 큐 끝에 추가 예측을 덧붙인다.
        ///   - fresh[0..existing-1] ≈ 기존 큐 항목 (동일 게이지 상태 기반 예측)
        ///   - fresh[existing..] 만 새로 덧붙여 기존 순서 불변 보장
        /// </summary>
        public void PruneDeadFromCommittedQueue()
        {
            int before = _committedQueue.Count;
            _committedQueue.RemoveAll(p => p.IsDead);
            int after = _committedQueue.Count;

            if (after == before) return; // 사망자 없음

            Debug.Log($"{_logClass} 사망 참가자 제거 — {before} → {after}");

            if (_committedQueue.Count < MinQueueDisplay)
            {
                int existing = _committedQueue.Count;
                // 현재 게이지 기준으로 QueueBuildCount개 예측.
                // fresh[0..existing-1] ≈ 기존 큐 → fresh[existing..] 만 끝에 덧붙임.
                var fresh = GetPredictedActionOrder(QueueBuildCount);
                for (int i = existing; i < fresh.Length; i++)
                    _committedQueue.Add(fresh[i]);

                Debug.Log($"{_logClass} 큐 보충 — {existing} → {_committedQueue.Count}");
            }
        }

        // ──────────────────────────────────────────────
        // CleanupBattle
        // ──────────────────────────────────────────────

        public void CleanupBattle()
        {
            _runtimeData = null;
            _committedQueue.Clear();
        }

        // ──────────────────────────────────────────────
        // IncrementTurn
        // ──────────────────────────────────────────────

        public void IncrementTurn()
        {
            _runtimeData.TurnNumber++;
        }
    }
}
