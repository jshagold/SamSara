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
            _hpBarView.SetHp(viewModel.CurrentHp, viewModel.MaxHp);
            _actionPointsView.SetActionPoints(viewModel.ActionPoints, viewModel.MaxActionPoints);
            _goldView.SetGold(viewModel.Gold);
        }

        public void SetCharacterSprite(Sprite sprite) => _characterSpriteView.SetSprite(sprite);

        public void UpdateGold(int gold) => _goldView.SetGold(gold);

        private void Reset()
        {
            _characterSpriteView = GetComponentInChildren<CharacterSpriteView>();
            _hpBarView           = GetComponentInChildren<HpBarView>();
            _actionPointsView    = GetComponentInChildren<ActionPointsView>();
            _goldView            = GetComponentInChildren<GoldView>();
        }
    }
}
