using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.MasterData;
using Samsara.Features.MainScene.Domain;
using Samsara.Features.MainScene.Presentation;
using Samsara.Features.MainScene.Presentation.Main;
using UnityEngine;

namespace Samsara.Dev.MainScene
{
    /// <summary>
    /// [V-07] MerchantButtonView가 기본적으로 비활성 상태인지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: MainScene에 이 컴포넌트 추가 → Inspector에서 참조 할당 → Play Mode 진입.
    ///
    /// 검증 항목:
    /// 1. MainUseCase.GetMainViewModel().IsMerchantActive == false (로직 검증)
    /// 2. Presenter.Initialize() 후 MerchantButtonView의 GameObject가 비활성 (UI 검증)
    /// </summary>
    public class V07_MerchantDefaultStateValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V07_MerchantDefaultStateValidation)}]";

        [SerializeField] private MainView _mainView;
        [SerializeField] private MerchantButtonView _merchantButtonView;
        [SerializeField] private CharacterStatsSO _testStats;

        private const string TestNodeId = "v07_merchant_node";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTaskVoid RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-07 검증 시작: MerchantButtonView 기본 비활성 상태 ===");

            if (_mainView == null || _merchantButtonView == null || _testStats == null)
            {
                Debug.LogError($"{_logClass} [FAIL] 필수 참조가 Inspector에서 할당되지 않았습니다.");
                return;
            }

            // ── Step 1: UseCase 로직 검증 ──
            var runRepo = new CharacterRunRepository();
            await runRepo.LoadDataAsync();
            runRepo.InitializeNewRun(TestNodeId, _testStats);

            var useCase = new MainUseCase(runRepo);
            var vm = useCase.GetMainViewModel();

            bool logicPass = vm.IsMerchantActive == false;
            LogCheck("MainViewModel.IsMerchantActive == false (OQ-02 미해결)", logicPass);

            // ── Step 2: Presenter.Initialize() 후 UI 상태 검증 ──
            var mockNavigator = new MockSceneNavigator();
            var presenter = new MainPresenter(useCase, _mainView, mockNavigator);
            presenter.Initialize();

            await UniTask.DelayFrame(1);

            bool uiPass = !_merchantButtonView.gameObject.activeSelf;
            LogCheck("MerchantButtonView.gameObject.activeSelf == false", uiPass);

            // ── 최종 판정 ──
            bool allPass = logicPass && uiPass;

            Debug.Log($"{_logClass} -------------------------------------------");
            if (allPass)
                Debug.Log($"{_logClass} [PASS] V-07: MerchantButtonView가 기본 비활성 상태로 올바르게 설정됨.");
            else
                Debug.LogError($"{_logClass} [FAIL] V-07: Merchant 기본 상태 검증 실패. 위 로그를 확인하세요.");

            Debug.Log($"{_logClass} === V-07 검증 종료. ===");
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
