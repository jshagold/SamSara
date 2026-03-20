using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using UnityEngine;

namespace Samsara.Dev.Character
{
    /// <summary>
    /// [V-07] UnlockEvolutionNode() / RegisterCodex() 중복 호출 시 리스트에 항목이 1개만 존재하는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    /// </summary>
    public class V07_NoDuplicateValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V07_NoDuplicateValidation)}]";

        private const string TestNodeId  = "v07_test_node";
        private const string TestCodexId = "v07_test_codex";
        private const int    RepeatCount = 3;

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTask RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-07 검증 시작: UnlockEvolutionNode() / RegisterCodex() 중복 방지 ===");

            var accountSavePath   = Application.persistentDataPath + "/account_save.json";
            var accountBackupPath = accountSavePath + ".v07.bak";

            // ── 기존 파일 백업 → 클린 AccountData 확보 ───────────────────
            bool fileExisted = File.Exists(accountSavePath);
            if (fileExisted) File.Move(accountSavePath, accountBackupPath);

            try
            {
                var accountRepo = new CharacterAccountRepository();
                await accountRepo.LoadDataAsync(); // 저장 파일 없음 → 빈 리스트로 초기화

                // ════════════════════════════════════════════════════════════
                // Case 1 — UnlockEvolutionNode() 동일 ID 반복 호출
                // ════════════════════════════════════════════════════════════
                Debug.Log($"{_logClass} --- [Case 1] UnlockEvolutionNode(\"{TestNodeId}\") × {RepeatCount}회 ---");

                for (int i = 0; i < RepeatCount; i++)
                    accountRepo.UnlockEvolutionNode(TestNodeId);

                var unlockedList = accountRepo.AccountData.UnlockedEvolutionNodeIds;
                int unlockOccurrences = CountOccurrences(unlockedList, TestNodeId);

                LogCheck($"UnlockedEvolutionNodeIds.Count == 1       (실제={unlockedList.Count})",
                    unlockedList.Count == 1);
                LogCheck($"Contains(\"{TestNodeId}\") 정확히 1회     (실제={unlockOccurrences}회)",
                    unlockOccurrences == 1);

                bool unlockPass = unlockedList.Count == 1 && unlockOccurrences == 1;

                // ════════════════════════════════════════════════════════════
                // Case 2 — RegisterCodex() 동일 ID 반복 호출
                // ════════════════════════════════════════════════════════════
                Debug.Log($"{_logClass} --- [Case 2] RegisterCodex(\"{TestCodexId}\") × {RepeatCount}회 ---");

                for (int i = 0; i < RepeatCount; i++)
                    accountRepo.RegisterCodex(TestCodexId);

                var codexList = accountRepo.AccountData.CompletedCodexIds;
                int codexOccurrences = CountOccurrences(codexList, TestCodexId);

                LogCheck($"CompletedCodexIds.Count == 1              (실제={codexList.Count})",
                    codexList.Count == 1);
                LogCheck($"Contains(\"{TestCodexId}\") 정확히 1회   (실제={codexOccurrences}회)",
                    codexOccurrences == 1);

                bool codexPass = codexList.Count == 1 && codexOccurrences == 1;

                // ════════════════════════════════════════════════════════════
                // Case 3 — 서로 다른 ID는 각각 독립적으로 추가됨 (Positive Control)
                // ════════════════════════════════════════════════════════════
                Debug.Log($"{_logClass} --- [Case 3] 서로 다른 ID 추가 — 각각 1개씩 존재해야 함 ---");

                const string anotherNodeId = "v07_another_node";
                accountRepo.UnlockEvolutionNode(anotherNodeId);

                bool positivePass = unlockedList.Count == 2
                                 && CountOccurrences(unlockedList, anotherNodeId) == 1;

                LogCheck($"UnlockedEvolutionNodeIds.Count == 2 after 추가  (실제={unlockedList.Count})",
                    unlockedList.Count == 2);
                LogCheck($"Contains(\"{anotherNodeId}\") 정확히 1회",
                    CountOccurrences(unlockedList, anotherNodeId) == 1);

                // ── 최종 판정 ────────────────────────────────────────────
                Debug.Log($"{_logClass} -------------------------------------------");
                bool allPass = unlockPass && codexPass && positivePass;
                if (allPass)
                    Debug.Log($"{_logClass} [PASS] V-07: 중복 호출 시 리스트에 중복 항목이 추가되지 않음.");
                else
                    Debug.LogError($"{_logClass} [FAIL] V-07: 중복 항목 감지. 위 로그를 확인하세요.");
            }
            finally
            {
                // V-07은 SaveDataAsync 미호출 → account_save.json 생성되지 않음.
                // 혹시 모를 잔류 파일 제거 후 기존 파일 복구.
                if (File.Exists(accountSavePath)) File.Delete(accountSavePath);
                if (fileExisted) File.Move(accountBackupPath, accountSavePath);

                Debug.Log($"{_logClass} === V-07 검증 종료. ===");
            }
        }

        /// <summary>LINQ 없이 리스트 내 target 출현 횟수를 셈.</summary>
        private static int CountOccurrences(List<string> list, string target)
        {
            int count = 0;
            foreach (var item in list)
                if (item == target) count++;
            return count;
        }

        private void LogCheck(string label, bool pass)
        {
            if (pass)
                Debug.Log($"{_logClass}   [PASS] {label}");
            else
                Debug.LogError($"{_logClass}   [FAIL] {label}");
        }
    }
}
