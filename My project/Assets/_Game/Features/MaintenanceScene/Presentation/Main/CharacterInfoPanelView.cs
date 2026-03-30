using Cysharp.Threading.Tasks;
using Samsara.Features.MaintenanceScene.Domain;
using UnityEngine;

namespace Samsara.Features.MaintenanceScene.Presentation.Main
{
    public class CharacterInfoPanelView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterInfoPanelView)}]";

        [SerializeField] private CharacterSpriteView _characterSpriteView;
        [SerializeField] private HpBarView _hpBarView;
        [SerializeField] private ActionPointsView _actionPointsView;
        [SerializeField] private GoldView _goldView;

        public void SetCharacterInfo(MaintenanceViewModel viewModel)
        {
            // TODO: EvolutionNodeId는 Addressables 키가 아님.
            // EvolutionNodeSO MasterData에서 sprite addressable key를 조회하는 로직 필요.
            // 스프라이트 시스템 구현 전까지 로드 스킵.
            _hpBarView.SetHp(viewModel.CurrentHp, viewModel.MaxHp);
            _actionPointsView.SetActionPoints(viewModel.ActionPoints, viewModel.MaxActionPoints);
            _goldView.SetGold(viewModel.Gold);
        }

        private void Reset()
        {
            _characterSpriteView = GetComponentInChildren<CharacterSpriteView>();
            _hpBarView           = GetComponentInChildren<HpBarView>();
            _actionPointsView    = GetComponentInChildren<ActionPointsView>();
            _goldView            = GetComponentInChildren<GoldView>();
        }
    }
}
