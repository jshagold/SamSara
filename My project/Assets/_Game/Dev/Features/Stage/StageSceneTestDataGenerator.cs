using System.IO;
using Samsara.Features.Stage.MasterData;
using UnityEditor;
using UnityEngine;

namespace Samsara.Dev.Stage
{
    /// <summary>
    /// StageScene 테스트용 MasterData SO 에셋 생성 에디터 스크립트.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 메뉴 → Samsara/Dev/Generate StageScene Test Data
    /// </summary>
    public static class StageSceneTestDataGenerator
    {
        private const string OutputPath = "Assets/Resources/MasterData/Stage";

        [MenuItem("Samsara/Dev/Generate StageScene Test Data")]
        public static void Generate()
        {
            EnsureDirectory(OutputPath);

            // ── BattleNodeData ──
            var battleData = CreateAsset<BattleNodeDataSO>($"{OutputPath}/BattleNodeData_Test.asset");
            SetPrivateField(battleData, "_isBoss", false);
            SetPrivateField(battleData, "_enemySpawns", new EnemySpawn[0]);

            var bossData = CreateAsset<BattleNodeDataSO>($"{OutputPath}/BattleNodeData_Boss_Test.asset");
            SetPrivateField(bossData, "_isBoss", true);
            SetPrivateField(bossData, "_enemySpawns", new EnemySpawn[0]);

            // ── EventNodeData ──
            var eventData = CreateAsset<EventNodeDataSO>($"{OutputPath}/EventNodeData_Test.asset");
            SetPrivateField(eventData, "_eventId", 1);

            // ── StageNode SOs ──
            var nodeStart = CreateNode("node_start", NodeType.Start, null, null, true);
            var nodeBattle = CreateNode("node_battle_01", NodeType.Battle, battleData, null, true);
            var nodeEvent = CreateNode("node_event_01", NodeType.Event, null, eventData, true);
            var nodeBoss = CreateNode("node_boss_01", NodeType.Boss, bossData, null, false);

            // ── StageSO ──
            var stageSO = CreateAsset<StageSO>($"{OutputPath}/Stage_Test_01.asset");
            SetPrivateField(stageSO, "_stageId", "stage_test_01");
            SetPrivateField(stageSO, "_stageName", "Test Stage 01");
            SetPrivateField(stageSO, "_backgroundSpriteKey", "bg_forest");
            SetPrivateField(stageSO, "_backgroundMusicKey", "bgm_forest");
            SetPrivateField(stageSO, "_isFixed", true);
            SetPrivateField(stageSO, "_fixedNodes", new[] { nodeStart, nodeBattle, nodeEvent, nodeBoss });
            SetPrivateField(stageSO, "_nextStageIds", new System.Collections.Generic.List<string>());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[StageSceneTestDataGenerator] 테스트 MasterData 생성 완료 → {OutputPath}");
        }

        private static StageNodeSO CreateNode(
            string nodeId, NodeType nodeType,
            BattleNodeDataSO battleData, EventNodeDataSO eventData,
            bool canReturnToMain)
        {
            var node = CreateAsset<StageNodeSO>($"{OutputPath}/StageNode_{nodeId}.asset");
            SetPrivateField(node, "_nodeId", nodeId);
            SetPrivateField(node, "_nodeType", nodeType);
            SetPrivateField(node, "_nodeSpriteKey", $"sprite_{nodeId}");
            SetPrivateField(node, "_battleData", battleData);
            SetPrivateField(node, "_eventData", eventData);
            SetPrivateField(node, "_canReturnToMain", canReturnToMain);
            return node;
        }

        private static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                Debug.Log($"[StageSceneTestDataGenerator] 기존 에셋 덮어쓰기: {path}");
                return existing;
            }

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogWarning($"[StageSceneTestDataGenerator] 필드를 찾을 수 없음: {fieldName}");
                return;
            }

            field.SetValue(target, value);
            EditorUtility.SetDirty((Object)target);
        }

        private static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parts = path.Split('/');
                var current = parts[0];
                for (var i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
            }
        }
    }
}
