using System;
using Cysharp.Threading.Tasks;
using Samsara.Features.BattleScene.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samsara.Features.BattleScene.Presentation
{
    public class BattleSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BattleSceneBootstrapper)}]";

        [SerializeField] private BattleView _battleView;

        private BattlePresenter _battlePresenter;

#if UNITY_EDITOR
        // BattleScene 직접 Play 시 Bootstrap 우회용 플래그.
        // GlobalBootstrapper step 6에서 읽어 Main 대신 Battle로 복귀.
        public static bool IsDirectTestMode;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RedirectToBootstrapIfNeeded()
        {
            if (GlobalBootstrapper.Instance != null) return;
            if (SceneManager.GetActiveScene().name != "Battle") return;

            // GlobalBootstrapper가 없는 상태에서 Battle씬이 실행됐음.
            // Bootstrap씬을 먼저 로드해 초기화를 완료한 뒤 Battle로 돌아온다.
            IsDirectTestMode = true;
            SceneManager.LoadScene("Bootstrap");
        }
#endif

        private void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext = GlobalBootstrapper.Instance.GameContext;
            var sceneNavigator = GlobalBootstrapper.Instance.SceneNavigator;
            var popupManager = gameContext.PopupManager;

            var characterRunRepo = gameContext.CharacterRunRepo;
            var skillMasterDataRepo = gameContext.SkillMasterDataRepo;
            var stageMasterDataRepo = gameContext.StageMasterDataRepo;

            var pendingContext = gameContext.PendingBattleContext;

#if UNITY_EDITOR
            // PendingBattleContext가 없으면 테스트 데이터로 대체한다. (Editor only)
            if (pendingContext == null)
            {
                var testNode = Resources.Load<Samsara.Features.Stage.MasterData.BattleNodeDataSO>("MasterData/battlenode_test_normal");
                if (testNode == null)
                {
                    throw new InvalidOperationException(
                        $"{_logClass} PendingBattleContext가 null이고 테스트 데이터(battlenode_test_normal)도 없습니다. Samsara > Dev > Create Test Battle Data를 먼저 실행하세요.");
                }
                pendingContext = new PendingBattleContext(testNode);
                Debug.LogWarning($"{_logClass} PendingBattleContext가 null — 테스트 데이터로 대체합니다. " +
                                 $"EnemySpawns={testNode.EnemySpawns?.Length} (Editor only)");
            }
#else
            if (pendingContext == null)
                throw new InvalidOperationException(
                    $"{_logClass} PendingBattleContext가 null입니다. BattleScene은 StageScene에서만 진입 가능합니다.");
#endif

            // BattleEventHookRunner 생성 (Constitution §3: new는 Bootstrapper에서만)
            var battleEvents = pendingContext.BattleEvents;
            var hookRunner = new BattleEventHookRunner(battleEvents);

            // BattleUseCase 생성
            var battleUseCase = new BattleUseCase(
                skillMasterDataRepo,
                characterRunRepo,
                stageMasterDataRepo
            );

            // BattlePresenter 생성 — v2.0.0: hookRunner 주입
            _battlePresenter = new BattlePresenter(
                battleUseCase,
                _battleView,
                sceneNavigator,
                popupManager,
                skillMasterDataRepo,
                gameContext,
                hookRunner
            );

            _battlePresenter.Initialize(pendingContext);

            Debug.Log($"{_logClass} BattleScene 초기화 완료.");
        }

        private void OnDestroy()
        {
            _battlePresenter?.Dispose();
        }
    }
}
