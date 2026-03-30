using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using UnityEngine;

namespace Samsara.Dev.Character
{
    /// <summary>
    /// [DEV ONLY] CharacterRunData를 Inspector에서 직접 수정하는 임시 디버그 도구.
    /// Phase 6 정식 게임 시작 흐름 구현 전까지 테스트용으로 사용.
    /// 사용법:
    ///   1. 빈 GameObject에 컴포넌트 추가
    ///   2. Play Mode 진입
    ///   3. Inspector에서 원하는 값 수정 후 컨텍스트 메뉴 → Apply / Load 호출
    /// </summary>
    public class DebugRunDataEditor : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(DebugRunDataEditor)}]";

        [Header("적용할 값")]
        [SerializeField] private int    _hp             = 100;
        [SerializeField] private int    _maxHp          = 100;
        [SerializeField] private int    _strength       = 10;
        [SerializeField] private int    _toughness      = 10;
        [SerializeField] private int    _agility        = 10;
        [SerializeField] private int    _gold           = 0;
        [SerializeField] private int    _actionPoints   = 3;
        [SerializeField] private int    _maxActionPoints = 3;
        [SerializeField] private int    _day            = 1;
        [SerializeField] private string _evolutionNodeId = "test_node_id";

        [Header("옵션")]
        [SerializeField] private bool _loadOnStart = true;
        [SerializeField] private bool _applyOnStart = false;

        private void Start()
        {
            if (_loadOnStart) LoadFromRepo();
            if (_applyOnStart) ApplyToRepo();
        }

        /// <summary>현재 저장된 데이터를 Inspector 필드에 불러온다.</summary>
        [ContextMenu("Load from Repo")]
        private void LoadFromRepo()
        {
            var repo = GetRepo();
            if (repo == null) return;

            var d = repo.RunData;
            _hp              = d.Hp;
            _maxHp           = d.MaxHp;
            _strength        = d.Strength;
            _toughness       = d.Toughness;
            _agility         = d.Agility;
            _gold            = d.Gold;
            _actionPoints    = d.ActionPoints;
            _maxActionPoints = d.MaxActionPoints;
            _day             = d.Day;
            _evolutionNodeId = d.EvolutionNodeId;

            Debug.Log($"{_logClass} Repo 데이터 로드 완료.");
        }

        /// <summary>Inspector 필드의 값을 Repo에 즉시 반영하고 저장한다.</summary>
        [ContextMenu("Apply to Repo")]
        private void ApplyToRepo()
        {
            var repo = GetRepo();
            if (repo == null) return;

            var d = repo.RunData;
            d.Hp              = _hp;
            d.MaxHp           = _maxHp;
            d.Strength        = _strength;
            d.Toughness       = _toughness;
            d.Agility         = _agility;
            d.Gold            = _gold;
            d.ActionPoints    = _actionPoints;
            d.MaxActionPoints = _maxActionPoints;
            d.Day             = _day;
            d.EvolutionNodeId = _evolutionNodeId;

            ((CharacterRunRepository)repo).MarkDirty();
            repo.SaveDataAsync().Forget();

            Debug.Log($"{_logClass} Repo에 적용 및 저장 완료. AP={_actionPoints}/{_maxActionPoints}");
        }

        private Samsara.Features.Character.Domain.ICharacterRunRepository GetRepo()
        {
            if (GlobalBootstrapper.Instance == null)
            {
                Debug.LogWarning($"{_logClass} GlobalBootstrapper.Instance가 null. Play Mode에서 실행하세요.");
                return null;
            }

            var repo = GlobalBootstrapper.Instance.GameContext.CharacterRunRepo;
            if (repo.RunData == null)
            {
                Debug.LogWarning($"{_logClass} RunData가 null. LoadAllDataAsync 완료 후 사용하세요.");
                return null;
            }

            return repo;
        }
    }
}
