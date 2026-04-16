#if UNITY_EDITOR
using Samsara.Features.Stage.MasterData;
using UnityEditor;
using UnityEngine;

namespace Samsara.Dev.Features.Battle
{
    public static class BattleDataLoadTest
    {
        [MenuItem("Samsara/Dev/Test Load BattleNodeData")]
        public static void TestLoad()
        {
            // Test 1: Resources.Load
            var fromResources = Resources.Load<BattleNodeDataSO>("MasterData/battlenode_test_normal");
            Debug.Log($"[Test] Resources.Load result: {(fromResources == null ? "NULL" : "loaded")}");
            if (fromResources != null)
            {
                Debug.Log($"[Test] Resources.Load — EnemySpawns={(fromResources.EnemySpawns == null ? "null" : fromResources.EnemySpawns.Length.ToString())}");
                if (fromResources.EnemySpawns != null)
                    for (int i = 0; i < fromResources.EnemySpawns.Length; i++)
                        Debug.Log($"[Test]   Spawn[{i}]: EnemyId={fromResources.EnemySpawns[i].EnemyId}, Count={fromResources.EnemySpawns[i].Count}");
            }

            // Test 2: AssetDatabase.LoadAssetAtPath
            var fromAssetDB = AssetDatabase.LoadAssetAtPath<BattleNodeDataSO>("Assets/Resources/MasterData/battlenode_test_normal.asset");
            Debug.Log($"[Test] AssetDatabase.Load result: {(fromAssetDB == null ? "NULL" : "loaded")}");
            if (fromAssetDB != null)
            {
                Debug.Log($"[Test] AssetDatabase.Load — EnemySpawns={(fromAssetDB.EnemySpawns == null ? "null" : fromAssetDB.EnemySpawns.Length.ToString())}");
                if (fromAssetDB.EnemySpawns != null)
                    for (int i = 0; i < fromAssetDB.EnemySpawns.Length; i++)
                        Debug.Log($"[Test]   Spawn[{i}]: EnemyId={fromAssetDB.EnemySpawns[i].EnemyId}, Count={fromAssetDB.EnemySpawns[i].Count}");
            }

            // Test 3: Same instance?
            if (fromResources != null && fromAssetDB != null)
                Debug.Log($"[Test] Same instance? {ReferenceEquals(fromResources, fromAssetDB)}");
        }
    }
}
#endif
