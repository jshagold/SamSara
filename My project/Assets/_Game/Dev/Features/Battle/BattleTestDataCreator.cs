#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using Samsara.Core.MasterData;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Stage.MasterData;
using UnityEditor;
using UnityEngine;

namespace Samsara.Dev.Features.Battle
{
    public static class BattleTestDataCreator
    {
        private const string MasterDataPath = "Assets/Resources/MasterData";
        private const string SpritePath = "Assets/_Game/Dev/Features/Battle/Sprites";

        [MenuItem("Samsara/Dev/Create Test Battle Data")]
        public static void CreateTestBattleData()
        {
            EnsureFolders();

            // --- Sprites ---
            CreateTestSprites();

            // --- CharacterStatsSO ---
            var statsAlly = CreateCharacterStats("stats_test_ally", 100, 15, 10, 40);
            var statsWeak = CreateCharacterStats("stats_enemy_weak", 50, 8, 5, 20);
            var statsNormal = CreateCharacterStats("stats_enemy_normal", 80, 12, 8, 35);
            var statsBoss = CreateCharacterStats("stats_enemy_boss", 200, 20, 15, 50);

            // --- QTEPatternSO ---
            CreateQTEPatternSingle();
            CreateQTEPatternTriple();

            // --- SkillSO ---
            CreateSkillBasic();
            CreateSkillHeavy();
            CreateSkillDrain();

            // --- EnemySO ---
            CreateEnemy("enemy_test_weak", 9001, "Weak Enemy", statsWeak,
                new[] { 9001 }, "test_enemy_weak");
            CreateEnemy("enemy_test_normal", 9002, "Normal Enemy", statsNormal,
                new[] { 9001, 9002 }, "test_enemy_normal");
            CreateEnemy("enemy_test_boss", 9003, "Boss Enemy", statsBoss,
                new[] { 9001, 9002, 9003 }, "test_enemy_boss");

            // --- BattleNodeDataSO ---
            CreateBattleNodeNormal();
            CreateBattleNodeBoss();

            // --- EvolutionNodeSO ---
            CreateOrUpdateTestEvolutionNode(statsAlly);

            // --- Force update spriteKeys on all existing test SOs ---
            UpdateExistingSpriteKeys();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[BattleTestDataCreator] Test battle data creation complete.");
        }

        // ──────────────────────────────────────────────
        // Folder Setup
        // ──────────────────────────────────────────────

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder(MasterDataPath))
                AssetDatabase.CreateFolder("Assets/Resources", "MasterData");

            // Sprite folder chain
            EnsureFolder("Assets/_Game/Dev", "Features");
            EnsureFolder("Assets/_Game/Dev/Features", "Battle");
            EnsureFolder("Assets/_Game/Dev/Features/Battle", "Sprites");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string full = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(full))
                AssetDatabase.CreateFolder(parent, child);
        }

        // ──────────────────────────────────────────────
        // Sprite Creation
        // ──────────────────────────────────────────────

        private static void CreateTestSprites()
        {
            // Characters — squares
            CreateSquareSprite("test_ally", 64, new Color(0.2f, 0.4f, 0.9f));
            CreateSquareSprite("test_enemy_weak", 64, new Color(0.9f, 0.2f, 0.2f));
            CreateSquareSprite("test_enemy_normal", 64, new Color(1.0f, 0.6f, 0.1f));
            CreateSquareSprite("test_enemy_boss", 96, new Color(0.6f, 0.1f, 0.8f));

            // Skills — circles
            CreateCircleSprite("test_skill_basic", 32, new Color(0.6f, 0.6f, 0.6f));
            CreateCircleSprite("test_skill_heavy", 32, new Color(1.0f, 0.9f, 0.1f));
            CreateCircleSprite("test_skill_drain", 32, new Color(0.2f, 0.8f, 0.3f));

            // QTE — circle
            CreateCircleSprite("test_qte_tap", 48, Color.white);
        }

        private static void CreateSquareSprite(string name, int size, Color color)
        {
            string path = $"{SpritePath}/{name}.png";
            if (File.Exists(path))
            {
                Debug.Log($"[BattleTestDataCreator] Skipped sprite: {path} already exists");
                return;
            }

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();

            WriteSpriteAsset(path, tex, size);
        }

        private static void CreateCircleSprite(string name, int size, Color color)
        {
            string path = $"{SpritePath}/{name}.png";
            if (File.Exists(path))
            {
                Debug.Log($"[BattleTestDataCreator] Skipped sprite: {path} already exists");
                return;
            }

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size * 0.5f;
            float radius = center - 1f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center + 0.5f;
                    float dy = y - center + 0.5f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    tex.SetPixel(x, y, dist <= radius ? color : Color.clear);
                }
            }
            tex.Apply();

            WriteSpriteAsset(path, tex, size);
        }

        private static void WriteSpriteAsset(string path, Texture2D tex, int size)
        {
            byte[] png = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);

            File.WriteAllBytes(path, png);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = size;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();

            Debug.Log($"[BattleTestDataCreator] Created sprite: {path}");
        }

        // ──────────────────────────────────────────────
        // Sprite Key Helper
        // ──────────────────────────────────────────────

        private static string SpriteKey(string name) => $"{SpritePath}/{name}.png";

        // ──────────────────────────────────────────────
        // CharacterStatsSO
        // ──────────────────────────────────────────────

        private static CharacterStatsSO CreateCharacterStats(string name, int hp, int str, int tough, int agi)
        {
            string path = $"{MasterDataPath}/{name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<CharacterStatsSO>(path);
            if (existing != null)
            {
                Debug.Log($"[BattleTestDataCreator] Skipped: {path} already exists");
                return existing;
            }

            var so = ScriptableObject.CreateInstance<CharacterStatsSO>();
            var serialized = new SerializedObject(so);
            serialized.FindProperty("_hp").intValue = hp;
            serialized.FindProperty("_strength").intValue = str;
            serialized.FindProperty("_toughness").intValue = tough;
            serialized.FindProperty("_agility").intValue = agi;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(so, path);
            Debug.Log($"[BattleTestDataCreator] Created: {path}");
            return so;
        }

        // ──────────────────────────────────────────────
        // QTEPatternSO
        // ──────────────────────────────────────────────

        private static void CreateQTEPatternSingle()
        {
            string path = $"{MasterDataPath}/qte_test_single.asset";
            if (AssetDatabase.LoadAssetAtPath<QTEPatternSO>(path) != null)
            {
                Debug.Log($"[BattleTestDataCreator] Skipped: {path} already exists");
                return;
            }

            var so = ScriptableObject.CreateInstance<QTEPatternSO>();
            var serialized = new SerializedObject(so);
            serialized.FindProperty("_patternId").intValue = 9001;

            var list = serialized.FindProperty("_qteDataList");
            list.arraySize = 1;
            SetQTEData(list.GetArrayElementAtIndex(0), SpriteKey("test_qte_tap"),
                new Vector2(0.5f, 0.5f), 1.0f, 0f);

            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(so, path);
            Debug.Log($"[BattleTestDataCreator] Created: {path}");
        }

        private static void CreateQTEPatternTriple()
        {
            string path = $"{MasterDataPath}/qte_test_triple.asset";
            if (AssetDatabase.LoadAssetAtPath<QTEPatternSO>(path) != null)
            {
                Debug.Log($"[BattleTestDataCreator] Skipped: {path} already exists");
                return;
            }

            var so = ScriptableObject.CreateInstance<QTEPatternSO>();
            var serialized = new SerializedObject(so);
            serialized.FindProperty("_patternId").intValue = 9002;

            var list = serialized.FindProperty("_qteDataList");
            list.arraySize = 3;
            SetQTEData(list.GetArrayElementAtIndex(0), SpriteKey("test_qte_tap"),
                new Vector2(0.3f, 0.5f), 0.8f, 0.3f);
            SetQTEData(list.GetArrayElementAtIndex(1), SpriteKey("test_qte_tap"),
                new Vector2(0.5f, 0.5f), 0.7f, 0.3f);
            SetQTEData(list.GetArrayElementAtIndex(2), SpriteKey("test_qte_tap"),
                new Vector2(0.7f, 0.5f), 0.6f, 0f);

            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(so, path);
            Debug.Log($"[BattleTestDataCreator] Created: {path}");
        }

        private static void SetQTEData(SerializedProperty element, string spriteKey, Vector2 coord, float duration, float interval)
        {
            element.FindPropertyRelative("_spriteKey").stringValue = spriteKey;
            element.FindPropertyRelative("_coordinate").vector2Value = coord;
            element.FindPropertyRelative("_duration").floatValue = duration;
            element.FindPropertyRelative("_intervalToNext").floatValue = interval;
        }

        // ──────────────────────────────────────────────
        // SkillSO
        // ──────────────────────────────────────────────

        private static void CreateSkillBasic()
        {
            string path = $"{MasterDataPath}/skill_test_basic.asset";
            if (AssetDatabase.LoadAssetAtPath<SkillSO>(path) != null)
            {
                Debug.Log($"[BattleTestDataCreator] Skipped: {path} already exists");
                return;
            }

            var so = ScriptableObject.CreateInstance<SkillSO>();
            var serialized = new SerializedObject(so);
            serialized.FindProperty("_skillId").intValue = 9001;
            serialized.FindProperty("_skillName").stringValue = "Basic Attack";
            serialized.FindProperty("_description").stringValue = "A basic attack.";
            serialized.FindProperty("_damage").floatValue = 1.0f;
            serialized.FindProperty("_qtePatternId").intValue = 0;
            serialized.FindProperty("_costs").arraySize = 0;
            serialized.FindProperty("_effects").arraySize = 0;
            serialized.FindProperty("_iconSpriteKey").stringValue = SpriteKey("test_skill_basic");
            serialized.FindProperty("_effectSpriteKey").stringValue = "";
            serialized.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(so, path);
            Debug.Log($"[BattleTestDataCreator] Created: {path}");
        }

        private static void CreateSkillHeavy()
        {
            string path = $"{MasterDataPath}/skill_test_heavy.asset";
            if (AssetDatabase.LoadAssetAtPath<SkillSO>(path) != null)
            {
                Debug.Log($"[BattleTestDataCreator] Skipped: {path} already exists");
                return;
            }

            var so = ScriptableObject.CreateInstance<SkillSO>();
            var serialized = new SerializedObject(so);
            serialized.FindProperty("_skillId").intValue = 9002;
            serialized.FindProperty("_skillName").stringValue = "Heavy Strike";
            serialized.FindProperty("_description").stringValue = "A heavy attack with QTE.";
            serialized.FindProperty("_damage").floatValue = 1.8f;
            serialized.FindProperty("_qtePatternId").intValue = 9001;
            serialized.FindProperty("_iconSpriteKey").stringValue = SpriteKey("test_skill_heavy");
            serialized.FindProperty("_effectSpriteKey").stringValue = "";
            serialized.FindProperty("_effects").arraySize = 0;

            var costs = serialized.FindProperty("_costs");
            costs.arraySize = 1;
            var cooldownCost = costs.GetArrayElementAtIndex(0);
            cooldownCost.FindPropertyRelative("_costType").enumValueIndex = (int)CostType.CoolDown;
            cooldownCost.FindPropertyRelative("_value").floatValue = 2f;

            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(so, path);
            Debug.Log($"[BattleTestDataCreator] Created: {path}");
        }

        private static void CreateSkillDrain()
        {
            string path = $"{MasterDataPath}/skill_test_drain.asset";
            if (AssetDatabase.LoadAssetAtPath<SkillSO>(path) != null)
            {
                Debug.Log($"[BattleTestDataCreator] Skipped: {path} already exists");
                return;
            }

            var so = ScriptableObject.CreateInstance<SkillSO>();
            var serialized = new SerializedObject(so);
            serialized.FindProperty("_skillId").intValue = 9003;
            serialized.FindProperty("_skillName").stringValue = "Drain Strike";
            serialized.FindProperty("_description").stringValue = "A powerful attack that costs HP.";
            serialized.FindProperty("_damage").floatValue = 2.5f;
            serialized.FindProperty("_qtePatternId").intValue = 9002;
            serialized.FindProperty("_iconSpriteKey").stringValue = SpriteKey("test_skill_drain");
            serialized.FindProperty("_effectSpriteKey").stringValue = "";
            serialized.FindProperty("_effects").arraySize = 0;

            var costs = serialized.FindProperty("_costs");
            costs.arraySize = 2;
            var cooldownCost = costs.GetArrayElementAtIndex(0);
            cooldownCost.FindPropertyRelative("_costType").enumValueIndex = (int)CostType.CoolDown;
            cooldownCost.FindPropertyRelative("_value").floatValue = 3f;
            var hpCost = costs.GetArrayElementAtIndex(1);
            hpCost.FindPropertyRelative("_costType").enumValueIndex = (int)CostType.Hp;
            hpCost.FindPropertyRelative("_value").floatValue = 10f;

            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(so, path);
            Debug.Log($"[BattleTestDataCreator] Created: {path}");
        }

        // ──────────────────────────────────────────────
        // EnemySO
        // ──────────────────────────────────────────────

        private static void CreateEnemy(string assetName, int enemyId, string enemyName,
            CharacterStatsSO stats, int[] skillIds, string spriteName)
        {
            string path = $"{MasterDataPath}/{assetName}.asset";
            if (AssetDatabase.LoadAssetAtPath<EnemySO>(path) != null)
            {
                Debug.Log($"[BattleTestDataCreator] Skipped: {path} already exists");
                return;
            }

            var so = ScriptableObject.CreateInstance<EnemySO>();
            var serialized = new SerializedObject(so);
            serialized.FindProperty("_enemyId").intValue = enemyId;
            serialized.FindProperty("_enemyName").stringValue = enemyName;
            serialized.FindProperty("_baseStats").objectReferenceValue = stats;

            var skillIdsProp = serialized.FindProperty("_skillIds");
            skillIdsProp.arraySize = skillIds.Length;
            for (int i = 0; i < skillIds.Length; i++)
                skillIdsProp.GetArrayElementAtIndex(i).intValue = skillIds[i];

            string key = SpriteKey(spriteName);
            serialized.FindProperty("_battleSpriteKeyHp100").stringValue = key;
            serialized.FindProperty("_battleSpriteKeyHp50").stringValue = key;
            serialized.FindProperty("_battleSpriteKeyHp0").stringValue = key;

            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(so, path);
            Debug.Log($"[BattleTestDataCreator] Created: {path}");
        }

        // ──────────────────────────────────────────────
        // BattleNodeDataSO
        // ──────────────────────────────────────────────

        private static void CreateBattleNodeNormal()
        {
            string path = $"{MasterDataPath}/battlenode_test_normal.asset";
            if (AssetDatabase.LoadAssetAtPath<BattleNodeDataSO>(path) != null)
                AssetDatabase.DeleteAsset(path);

            var so = ScriptableObject.CreateInstance<BattleNodeDataSO>();
            JsonUtility.FromJsonOverwrite(
                "{\"_isBoss\":false,\"_enemySpawns\":[{\"_enemyId\":9001,\"_count\":1},{\"_enemyId\":9002,\"_count\":1}]}",
                so);
            AssetDatabase.CreateAsset(so, path);
            EditorUtility.SetDirty(so);

            Debug.Log($"[BattleTestDataCreator] Created: {path} — Spawns={so.EnemySpawns?.Length}, " +
                      $"[0]Id={so.EnemySpawns?[0]?.EnemyId} Count={so.EnemySpawns?[0]?.Count}, " +
                      $"[1]Id={so.EnemySpawns?[1]?.EnemyId} Count={so.EnemySpawns?[1]?.Count}");
        }

        private static void CreateBattleNodeBoss()
        {
            string path = $"{MasterDataPath}/battlenode_test_boss.asset";
            if (AssetDatabase.LoadAssetAtPath<BattleNodeDataSO>(path) != null)
                AssetDatabase.DeleteAsset(path);

            var so = ScriptableObject.CreateInstance<BattleNodeDataSO>();
            JsonUtility.FromJsonOverwrite(
                "{\"_isBoss\":true,\"_enemySpawns\":[{\"_enemyId\":9003,\"_count\":1}]}",
                so);
            AssetDatabase.CreateAsset(so, path);
            EditorUtility.SetDirty(so);

            Debug.Log($"[BattleTestDataCreator] Created: {path} — Spawns={so.EnemySpawns?.Length}, " +
                      $"[0]Id={so.EnemySpawns?[0]?.EnemyId} Count={so.EnemySpawns?[0]?.Count}");
        }

        // ──────────────────────────────────────────────
        // EvolutionNodeSO
        // ──────────────────────────────────────────────

        private static void CreateOrUpdateTestEvolutionNode(CharacterStatsSO allyStats)
        {
            var allNodes = Resources.LoadAll<EvolutionNodeSO>("MasterData");
            EvolutionNodeSO targetNode = null;

            foreach (var node in allNodes)
            {
                if (node.NodeId == "test_node_id")
                {
                    targetNode = node;
                    break;
                }
            }

            if (targetNode == null)
            {
                string path = $"{MasterDataPath}/evo_test_node.asset";
                if (AssetDatabase.LoadAssetAtPath<EvolutionNodeSO>(path) != null)
                    targetNode = AssetDatabase.LoadAssetAtPath<EvolutionNodeSO>(path);
                else
                {
                    targetNode = ScriptableObject.CreateInstance<EvolutionNodeSO>();
                    AssetDatabase.CreateAsset(targetNode, path);
                    Debug.Log($"[BattleTestDataCreator] Created: {path}");
                }
            }

            string allyKey = SpriteKey("test_ally");

            var serialized = new SerializedObject(targetNode);
            serialized.FindProperty("_nodeId").stringValue = "test_node_id";
            serialized.FindProperty("_characterName").stringValue = "Test Hero";
            serialized.FindProperty("_baseStats").objectReferenceValue = allyStats;
            serialized.FindProperty("_maxActionPoints").intValue = 3;
            serialized.FindProperty("_isHidden").boolValue = false;
            serialized.FindProperty("_nodeIconSpriteKey").stringValue = allyKey;
            serialized.FindProperty("_mainStandingSpriteKey").stringValue = allyKey;
            serialized.FindProperty("_portraitSpriteKey").stringValue = allyKey;
            serialized.FindProperty("_battleSpriteKeyHp100").stringValue = allyKey;
            serialized.FindProperty("_battleSpriteKeyHp50").stringValue = allyKey;
            serialized.FindProperty("_battleSpriteKeyHp0").stringValue = allyKey;
            serialized.FindProperty("_attackAnimSpriteKey").stringValue = allyKey;
            serialized.FindProperty("_stageMoveSpriteKey").stringValue = allyKey;

            var skillIds = serialized.FindProperty("_skillIds");
            skillIds.arraySize = 3;
            skillIds.GetArrayElementAtIndex(0).intValue = 9001;
            skillIds.GetArrayElementAtIndex(1).intValue = 9002;
            skillIds.GetArrayElementAtIndex(2).intValue = 9003;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(targetNode);
            Debug.Log($"[BattleTestDataCreator] Updated EvolutionNodeSO '{targetNode.NodeId}'");
        }

        // ──────────────────────────────────────────────
        // Force Update SpriteKeys on Existing SOs
        // ──────────────────────────────────────────────

        private static void UpdateExistingSpriteKeys()
        {
            // EnemySO
            UpdateEnemySpriteKey("enemy_test_weak", "test_enemy_weak");
            UpdateEnemySpriteKey("enemy_test_normal", "test_enemy_normal");
            UpdateEnemySpriteKey("enemy_test_boss", "test_enemy_boss");

            // SkillSO
            UpdateSkillSpriteKey("skill_test_basic", "test_skill_basic");
            UpdateSkillSpriteKey("skill_test_heavy", "test_skill_heavy");
            UpdateSkillSpriteKey("skill_test_drain", "test_skill_drain");

            // QTEPatternSO
            UpdateQTESpriteKey("qte_test_single");
            UpdateQTESpriteKey("qte_test_triple");

            Debug.Log("[BattleTestDataCreator] SpriteKey force-update complete.");
        }

        private static void UpdateEnemySpriteKey(string assetName, string spriteName)
        {
            string path = $"{MasterDataPath}/{assetName}.asset";
            var so = AssetDatabase.LoadAssetAtPath<EnemySO>(path);
            if (so == null) return;

            string key = SpriteKey(spriteName);
            var serialized = new SerializedObject(so);
            serialized.FindProperty("_battleSpriteKeyHp100").stringValue = key;
            serialized.FindProperty("_battleSpriteKeyHp50").stringValue = key;
            serialized.FindProperty("_battleSpriteKeyHp0").stringValue = key;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(so);
        }

        private static void UpdateSkillSpriteKey(string assetName, string spriteName)
        {
            string path = $"{MasterDataPath}/{assetName}.asset";
            var so = AssetDatabase.LoadAssetAtPath<SkillSO>(path);
            if (so == null) return;

            var serialized = new SerializedObject(so);
            serialized.FindProperty("_iconSpriteKey").stringValue = SpriteKey(spriteName);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(so);
        }

        private static void UpdateQTESpriteKey(string assetName)
        {
            string path = $"{MasterDataPath}/{assetName}.asset";
            var so = AssetDatabase.LoadAssetAtPath<QTEPatternSO>(path);
            if (so == null) return;

            string key = SpriteKey("test_qte_tap");
            var serialized = new SerializedObject(so);
            var list = serialized.FindProperty("_qteDataList");
            for (int i = 0; i < list.arraySize; i++)
                list.GetArrayElementAtIndex(i).FindPropertyRelative("_spriteKey").stringValue = key;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(so);
        }
    }
}
#endif
