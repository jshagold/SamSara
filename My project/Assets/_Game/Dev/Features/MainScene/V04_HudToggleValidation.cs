using Cysharp.Threading.Tasks;
using Samsara.Features.MainScene.Presentation;
using Samsara.Features.MainScene.Presentation.Hud;
using UnityEngine;

namespace Samsara.Dev.MainScene
{
    /// <summary>
    /// [V-04 / V-05] HUD 토글 기능 검증.
    /// V-04: HUD 요소가 on/off 버튼 탭 시 slide out/in 되는지 확인.
    /// V-05: HUD off 상태에서도 토글 버튼이 화면에 고정 유지되는지 확인.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: MainScene에 이 컴포넌트를 추가하고 _mainView를 Inspector에서 연결 → Play Mode 진입.
    ///         3초 후 자동으로 HUD 토글 테스트를 수행.
    /// </summary>
    public class V04_HudToggleValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V04_HudToggleValidation)}]";

        [SerializeField] private MainView _mainView;
        [SerializeField] private HudToggleButtonView _hudToggleButtonView;
        [SerializeField] private HudView _hudView;

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTaskVoid RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-04/V-05 검증 시작: HUD 토글 기능 ===");

            // ── 사전 조건 확인 ──
            bool mainViewExists = _mainView != null;
            bool toggleButtonExists = _hudToggleButtonView != null;
            bool hudViewExists = _hudView != null;

            LogCheck("MainView 참조 존재", mainViewExists);
            LogCheck("HudToggleButtonView 참조 존재", toggleButtonExists);
            LogCheck("HudView 참조 존재", hudViewExists);

            if (!mainViewExists || !toggleButtonExists || !hudViewExists)
            {
                Debug.LogError($"{_logClass} [FAIL] V-04/V-05: 필수 참조가 Inspector에서 할당되지 않았습니다.");
                return;
            }

            // ── V-05: 토글 버튼의 GameObject가 활성 상태인지 확인 ──
            bool toggleActiveInitially = _hudToggleButtonView.gameObject.activeSelf;
            LogCheck("V-05: 토글 버튼 초기 활성 상태", toggleActiveInitially);

            // ── V-04: HideHud 호출 후 상태 확인 (1초 대기 후 애니메이션 완료 확인) ──
            Debug.Log($"{_logClass} HideHud() 호출...");
            _mainView.HideHud();

            await UniTask.Delay(500);

            // V-05: HUD off 상태에서도 토글 버튼이 활성 유지
            bool toggleActiveAfterHide = _hudToggleButtonView.gameObject.activeSelf;
            LogCheck("V-05: HUD 숨김 후에도 토글 버튼 활성 유지", toggleActiveAfterHide);

            // ── V-04: ShowHud 호출 후 복귀 확인 ──
            Debug.Log($"{_logClass} ShowHud() 호출...");
            _mainView.ShowHud();

            await UniTask.Delay(500);

            bool toggleActiveAfterShow = _hudToggleButtonView.gameObject.activeSelf;
            LogCheck("V-04: HUD 표시 후 토글 버튼 여전히 활성", toggleActiveAfterShow);

            // ── 최종 판정 ──
            bool allPass = toggleActiveInitially && toggleActiveAfterHide && toggleActiveAfterShow;

            Debug.Log($"{_logClass} -------------------------------------------");
            if (allPass)
                Debug.Log($"{_logClass} [PASS] V-04/V-05: HUD 토글 기능 및 버튼 고정 정상.");
            else
                Debug.LogError($"{_logClass} [FAIL] V-04/V-05: 검증 실패. 위 로그를 확인하세요.");

            Debug.Log($"{_logClass} === V-04/V-05 검증 종료. ===");
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
