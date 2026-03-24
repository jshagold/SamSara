using System.Threading;
using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.MasterData;
using Samsara.Features.MainScene.Domain;
using Samsara.Features.MainScene.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Dev.MainScene
{
    /// <summary>
    /// [V-06] 네비게이션 버튼 클릭 → 캐릭터 애니메이션 → Presenter → ISceneNavigator 흐름 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: MainScene에 이 컴포넌트 추가 → Inspector에서 참조 할당 → Play Mode 진입.
    ///
    /// 실제 씬 전환 대신 MockSceneNavigator를 사용하여 NavigateToAsync 호출 여부만 검증.
    /// 버튼의 onClick.Invoke()로 실제 클릭을 시뮬레이션하여 전체 이벤트 체인을 테스트.
    /// </summary>
    public class V06_NavigationEventValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V06_NavigationEventValidation)}]";

        [SerializeField] private MainView _mainView;
        [SerializeField] private CharacterStatsSO _testStats;

        [Header("실제 UI 버튼 참조 (이벤트 시뮬레이션용)")]
        [SerializeField] private Button _stageButton;
        [SerializeField] private Button _maintenanceButton;
        [SerializeField] private Button _characterInfoButton;

        private const string TestNodeId = "v06_nav_node";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTaskVoid RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-06 검증 시작: 네비게이션 버튼 → 애니메이션 → SceneNavigator 호출 검증 ===");

            if (_mainView == null || _testStats == null)
            {
                Debug.LogError($"{_logClass} [FAIL] _mainView 또는 _testStats가 Inspector에서 할당되지 않았습니다.");
                return;
            }

            if (_stageButton == null || _maintenanceButton == null || _characterInfoButton == null)
            {
                Debug.LogError($"{_logClass} [FAIL] 버튼 참조가 Inspector에서 할당되지 않았습니다.");
                return;
            }

            // ── 테스트용 RunData 및 MockNavigator 준비 ──
            var runRepo = new CharacterRunRepository();
            await runRepo.LoadDataAsync();
            runRepo.InitializeNewRun(TestNodeId, _testStats);
            runRepo.RunData.ActionPoints = 3;
            runRepo.RunData.MaxActionPoints = 5;

            var mockNavigator = new MockSceneNavigator();
            var useCase = new MainUseCase(runRepo);
            var presenter = new MainPresenter(useCase, _mainView, mockNavigator);
            presenter.Initialize();

            // ── Stage 버튼 클릭 시뮬레이션 (애니메이션 완료 대기) ──
            Debug.Log($"{_logClass} Stage 버튼 클릭 시뮬레이션...");
            mockNavigator.Reset();
            _stageButton.onClick.Invoke();
            await UniTask.Delay(1000); // 애니메이션 완료 대기

            bool stageNav = mockNavigator.LastRequestedKey == SceneKey.Stage;
            LogCheck("Stage 버튼 → NavigateToAsync(SceneKey.Stage) 호출됨",
                stageNav, mockNavigator.LastRequestedKey?.ToString() ?? "null");

            // ── Maintenance 버튼 클릭 시뮬레이션 ──
            Debug.Log($"{_logClass} Maintenance 버튼 클릭 시뮬레이션...");
            mockNavigator.Reset();
            _maintenanceButton.onClick.Invoke();
            await UniTask.Delay(1000);

            bool maintNav = mockNavigator.LastRequestedKey == SceneKey.Maintenance;
            LogCheck("Maintenance 버튼 → NavigateToAsync(SceneKey.Maintenance) 호출됨",
                maintNav, mockNavigator.LastRequestedKey?.ToString() ?? "null");

            // ── CharacterInfo 버튼 클릭 시뮬레이션 ──
            Debug.Log($"{_logClass} CharacterInfo 버튼 클릭 시뮬레이션...");
            mockNavigator.Reset();
            _characterInfoButton.onClick.Invoke();
            await UniTask.Delay(1000);

            bool charInfoNav = mockNavigator.LastRequestedKey == SceneKey.CharacterInfo;
            LogCheck("CharacterInfo 버튼 → NavigateToAsync(SceneKey.CharacterInfo) 호출됨",
                charInfoNav, mockNavigator.LastRequestedKey?.ToString() ?? "null");

            // ── 최종 판정 ──
            bool allPass = stageNav && maintNav && charInfoNav;

            Debug.Log($"{_logClass} -------------------------------------------");
            if (allPass)
                Debug.Log($"{_logClass} [PASS] V-06: 모든 네비게이션 버튼이 애니메이션 후 SceneNavigator를 올바르게 호출함.");
            else
                Debug.LogError($"{_logClass} [FAIL] V-06: 네비게이션 이벤트 검증 실패. 위 로그를 확인하세요.");

            Debug.Log($"{_logClass} === V-06 검증 종료. ===");
        }

        private void LogCheck(string label, bool pass, string actual)
        {
            if (pass)
                Debug.Log($"{_logClass}   [PASS] {label}");
            else
                Debug.LogError($"{_logClass}   [FAIL] {label} (실제={actual})");
        }
    }

    /// <summary>
    /// 테스트 전용 Mock ISceneNavigator. 실제 씬 전환 없이 호출 여부만 기록.
    /// </summary>
    public class MockSceneNavigator : ISceneNavigator
    {
        public SceneKey? LastRequestedKey { get; private set; }
        public int CallCount { get; private set; }

        public UniTask NavigateToAsync(SceneKey key)
        {
            LastRequestedKey = key;
            CallCount++;
            Debug.Log($"[MockSceneNavigator] NavigateToAsync({key}) 호출됨 (#{CallCount})");
            return UniTask.CompletedTask;
        }

        public UniTask NavigateToAsync(SceneKey key, CancellationToken ct)
        {
            return NavigateToAsync(key);
        }

        public void Reset()
        {
            LastRequestedKey = null;
            CallCount = 0;
        }
    }
}
