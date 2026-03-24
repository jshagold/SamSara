using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.MasterData;
using Samsara.Features.MainScene.Domain;
using UnityEngine;

namespace Samsara.Dev.MainScene
{
    /// <summary>
    /// [V-03] MainUseCase.GetMainViewModel()이 RunData의 값을 올바르게 매핑하는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 → Inspector에서 _testStats 할당 → Play Mode 진입.
    /// </summary>
    public class V03_MainViewModelValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V03_MainViewModelValidation)}]";

        [SerializeField] private CharacterStatsSO _testStats;

        private const string TestNodeId = "v03_main_node";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTaskVoid RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-03 검증 시작: MainUseCase.GetMainViewModel() 매핑 검증 ===");

            // ── 테스트용 RunData 준비 ──
            var runRepo = new CharacterRunRepository();
            await runRepo.LoadDataAsync();
            runRepo.InitializeNewRun(TestNodeId, _testStats);

            // ActionPoints 수동 설정 (InitializeNewRun에서 설정하지 않으므로)
            runRepo.RunData.ActionPoints = 3;
            runRepo.RunData.MaxActionPoints = 5;

            // ── MainUseCase 생성 및 ViewModel 획득 ──
            var useCase = new MainUseCase(runRepo);
            var vm = useCase.GetMainViewModel();

            // ── 필드별 매핑 검증 ──
            bool dayPass   = vm.Day == runRepo.RunData.Day;
            bool goldPass  = vm.Gold == runRepo.RunData.Gold;
            bool hpPass    = vm.CurrentHp == runRepo.RunData.Hp;
            bool apPass    = vm.ActionPoints == runRepo.RunData.ActionPoints;
            bool maxApPass = vm.MaxActionPoints == runRepo.RunData.MaxActionPoints;
            bool nodePass  = vm.CurrentEvolutionNodeId == runRepo.RunData.EvolutionNodeId;
            bool merchantPass = vm.IsMerchantActive == false;

            Debug.Log($"{_logClass} --- MainViewModel 매핑 검증 ---");
            LogCheck($"Day == {vm.Day} (expected: {runRepo.RunData.Day})",
                dayPass, vm.Day.ToString());
            LogCheck($"Gold == {vm.Gold} (expected: {runRepo.RunData.Gold})",
                goldPass, vm.Gold.ToString());
            LogCheck($"CurrentHp == {vm.CurrentHp} (expected: {runRepo.RunData.Hp})",
                hpPass, vm.CurrentHp.ToString());
            LogCheck($"ActionPoints == {vm.ActionPoints} (expected: {runRepo.RunData.ActionPoints})",
                apPass, vm.ActionPoints.ToString());
            LogCheck($"MaxActionPoints == {vm.MaxActionPoints} (expected: {runRepo.RunData.MaxActionPoints})",
                maxApPass, vm.MaxActionPoints.ToString());
            LogCheck($"EvolutionNodeId == \"{vm.CurrentEvolutionNodeId}\" (expected: \"{runRepo.RunData.EvolutionNodeId}\")",
                nodePass, $"\"{vm.CurrentEvolutionNodeId}\"");
            LogCheck("IsMerchantActive == false (OQ-02 미해결, v1 기본값)",
                merchantPass, vm.IsMerchantActive.ToString());

            bool allPass = dayPass && goldPass && hpPass && apPass
                        && maxApPass && nodePass && merchantPass;

            Debug.Log($"{_logClass} -------------------------------------------");
            if (allPass)
                Debug.Log($"{_logClass} [PASS] V-03: GetMainViewModel()이 RunData를 올바르게 매핑함.");
            else
                Debug.LogError($"{_logClass} [FAIL] V-03: 매핑 불일치 항목 있음. 위 로그를 확인하세요.");

            Debug.Log($"{_logClass} === V-03 검증 종료. ===");
        }

        private void LogCheck(string label, bool pass, string actual)
        {
            if (pass)
                Debug.Log($"{_logClass}   [PASS] {label}");
            else
                Debug.LogError($"{_logClass}   [FAIL] {label} (실제={actual})");
        }
    }
}
