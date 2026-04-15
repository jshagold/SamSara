using Cysharp.Threading.Tasks;
using Samsara.Features.MainScene.Domain;
using UnityEngine;

namespace Samsara.Features.MainScene.Presentation
{
    public class MainSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MainSceneBootstrapper)}]";

        [SerializeField] private MainView _mainView;

        private MainPresenter _mainPresenter;

        private void Awake()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext      = GlobalBootstrapper.Instance.GameContext;
            var characterRunRepo = gameContext.CharacterRunRepo;
            var sceneNavigator   = GlobalBootstrapper.Instance.SceneNavigator;
            var spriteLoader     = gameContext.SpriteLoader;

            // 현재 캐릭터 EvolutionNodeSO 조회
            var nodeId = characterRunRepo.RunData.EvolutionNodeId;
            Samsara.Features.Character.MasterData.EvolutionNodeSO evolutionNode = null;
            foreach (var node in gameContext.EvolutionNodes)
            {
                if (node.NodeId == nodeId) { evolutionNode = node; break; }
            }

            var mainUseCase = new MainUseCase(characterRunRepo);

            _mainPresenter = new MainPresenter(mainUseCase, _mainView, sceneNavigator, spriteLoader, evolutionNode);
            _mainPresenter.Initialize();

            Debug.Log($"{_logClass} MainScene 초기화 완료.");
        }
    }
}
