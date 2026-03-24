using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using UnityEngine;

namespace Samsara.Dev.MainScene
{
    /// <summary>
    /// [V-02] CharacterRunData에 ActionPoints, MaxActionPoints 필드가 추가되었는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    /// </summary>
    public class V02_RunDataFieldsValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V02_RunDataFieldsValidation)}]";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTaskVoid RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-02 검증 시작: CharacterRunData ActionPoints 필드 존재 확인 ===");

            // ── CharacterRunRepository를 통해 RunData 생성 후 필드 접근 검증 ──
            var runRepo = new CharacterRunRepository();
            await runRepo.LoadDataAsync();

            var runData = runRepo.RunData;

            bool notNull = runData != null;
            bool apDefault = notNull && runData.ActionPoints == 0;
            bool maxApDefault = notNull && runData.MaxActionPoints == 0;

            // ── 필드 쓰기/읽기 검증 ──
            bool apWriteRead = false;
            bool maxApWriteRead = false;

            if (notNull)
            {
                runData.ActionPoints = 3;
                runData.MaxActionPoints = 5;
                apWriteRead = runData.ActionPoints == 3;
                maxApWriteRead = runData.MaxActionPoints == 5;

                // 원래 값으로 복원
                runData.ActionPoints = 0;
                runData.MaxActionPoints = 0;
            }

            // ── 결과 출력 ──
            Debug.Log($"{_logClass} --- CharacterRunData 필드 검증 ---");
            LogCheck("RunData != null", notNull);
            LogCheck("ActionPoints 기본값 == 0", apDefault);
            LogCheck("MaxActionPoints 기본값 == 0", maxApDefault);
            LogCheck("ActionPoints 쓰기/읽기 (3)", apWriteRead);
            LogCheck("MaxActionPoints 쓰기/읽기 (5)", maxApWriteRead);

            bool allPass = notNull && apDefault && maxApDefault && apWriteRead && maxApWriteRead;

            Debug.Log($"{_logClass} -------------------------------------------");
            if (allPass)
                Debug.Log($"{_logClass} [PASS] V-02: ActionPoints, MaxActionPoints 필드 정상 추가됨.");
            else
                Debug.LogError($"{_logClass} [FAIL] V-02: 필드 검증 실패. 위 로그를 확인하세요.");

            Debug.Log($"{_logClass} === V-02 검증 종료. ===");
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
