using Cysharp.Threading.Tasks;
using Samsara.Core.Popup;
using UnityEditor.VersionControl;
using UnityEngine;

/// <summary>
/// PopupManager Play Mode 검증 스크립트 (TASK-08: V-05 ~ V-09).
/// 사용법: 씬에 빈 GameObject를 생성하고 이 컴포넌트를 추가.
///         GlobalBootstrapper가 씬에 존재해야 한다.
/// </summary>
public class PopupManagerTest : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(PopupManagerTest)}]";

    private IPopupManager _popupManager;
    private string        _lastResult = "—";
    private bool          _isBusy;

    // ──────────────────────────────────────────────
    // Init
    // ──────────────────────────────────────────────

    private void Start() => InitAsync().Forget();

    private async UniTaskVoid InitAsync()
    {
        await GlobalBootstrapper.Instance.InitializationTask;
        _popupManager = GlobalBootstrapper.Instance.GameContext.PopupManager;
        Debug.Log($"{_logClass} IPopupManager 참조 획득 완료. 테스트 준비됨.");
    }

    // ──────────────────────────────────────────────
    // GUI
    // ──────────────────────────────────────────────

    private void OnGUI()
    {
        var labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
        var btnStyle   = new GUIStyle(GUI.skin.button) { fontSize = 13 };

        if (_popupManager == null)
        {
            GUI.Label(new Rect(10, 10, 500, 30), "GlobalBootstrapper 초기화 대기 중...", labelStyle);
            return;
        }

        var y = 10;

        GUI.Label(new Rect(10, y, 600, 28), $"결과: {_lastResult}", labelStyle);
        y += 35;

        GUI.enabled = !_isBusy;

        if (GUI.Button(new Rect(10, y, 320, 40), "[V-06] ShowConfirmAsync (확인 버튼만)", btnStyle))
            TestV06ConfirmAsync().Forget();
        y += 48;

        if (GUI.Button(new Rect(10, y, 320, 40), "[V-07] ShowYesNoAsync (예/아니오 순서대로)", btnStyle))
            TestV07YesNoAsync().Forget();
        y += 48;

        if (GUI.Button(new Rect(10, y, 320, 40), "[V-05] Pool 검증 (3회 Show → DismissAll)", btnStyle))
            TestV05PoolAsync().Forget();
        y += 48;

        if (GUI.Button(new Rect(10, y, 320, 40), "[V-08] 팝업 열고 2초 후 DismissAll", btnStyle))
            TestV08DismissAllAsync().Forget();
        y += 48;

        if (GUI.Button(new Rect(10, y, 320, 40), "[V-09] Dim Raycast 수동 확인", btnStyle))
            TestV09DimRaycastAsync().Forget();

        GUI.enabled = true;
    }

    // ──────────────────────────────────────────────
    // V-06: ShowConfirmAsync → 확인 클릭 시 true
    // ──────────────────────────────────────────────

    private async UniTaskVoid TestV06ConfirmAsync()
    {
        _isBusy = true;
        Debug.Log($"{_logClass} [V-06] 시작 — 팝업의 '확인'을 클릭하세요.");

        var request = new PopupRequest(title: "V-06 Confirm", message: "확인 버튼을 클릭하세요.");
        var result  = await _popupManager.ShowConfirmAsync(request);

        var pass    = result == true;
        _lastResult = pass ? "[V-06] PASS — true 반환" : $"[V-06] FAIL — 기대: true, 실제: {result}";
        Debug.Log($"{_logClass} {_lastResult}");
        _isBusy = false;
    }

    // ──────────────────────────────────────────────
    // V-07: ShowYesNoAsync → 예=true, 아니오=false
    // ──────────────────────────────────────────────

    private async UniTaskVoid TestV07YesNoAsync()
    {
        _isBusy = true;

        // 1단계 — 예 버튼 클릭 기대
        Debug.Log($"{_logClass} [V-07-A] 시작 — '예'(왼쪽)를 클릭하세요.");
        var requestA = new PopupRequest(title: "V-07 예/아니오 (1/2)", message: "'예' 버튼을 클릭하세요. → true 기대");
        var resultA  = await _popupManager.ShowYesNoAsync(requestA);
        var passA    = resultA == true;
        Debug.Log($"{_logClass} [V-07-A] 결과: {resultA} → {(passA ? "PASS" : "FAIL")}");

        // 2단계 — 아니오 버튼 클릭 기대
        Debug.Log($"{_logClass} [V-07-B] 시작 — '아니오'(오른쪽)를 클릭하세요.");
        var requestB = new PopupRequest(title: "V-07 예/아니오 (2/2)", message: "'아니오' 버튼을 클릭하세요. → false 기대");
        var resultB  = await _popupManager.ShowYesNoAsync(requestB);
        var passB    = resultB == false;
        Debug.Log($"{_logClass} [V-07-B] 결과: {resultB} → {(passB ? "PASS" : "FAIL")}");

        _lastResult = (passA && passB) ? "[V-07] PASS" : $"[V-07] FAIL — A:{passA} B:{passB}";
        Debug.Log($"{_logClass} {_lastResult}");
        _isBusy = false;
    }

    // ──────────────────────────────────────────────
    // V-05: Pool Get/Release 3회 반복
    // ──────────────────────────────────────────────

    private async UniTaskVoid TestV05PoolAsync()
    {
        _isBusy = true;
        Debug.Log($"{_logClass} [V-05] Pool 검증 시작 — 3회 Show → DismissAll → false 기대.");

        var allPass = true;

        for (var i = 1; i <= 3; i++)
        {
            Debug.Log($"{_logClass} [V-05] 팝업 #{i} Get");
            var request = new PopupRequest(title: $"Pool 테스트 ({i}/3)", message: $"0.8초 후 자동으로 닫힙니다.");

            var popupTask = _popupManager.ShowConfirmAsync(request);
            await UniTask.Delay(800);

            _popupManager.DismissAll();
            var result = await popupTask;

            var pass = result == false; // DismissAll이므로 false 기대
            allPass &= pass;
            Debug.Log($"{_logClass} [V-05] 팝업 #{i} Release — 결과: {result} → {(pass ? "PASS" : "FAIL")}");
        }

        _lastResult = allPass ? "[V-05] PASS — Pool Get/Release 정상" : "[V-05] FAIL — Console 로그 확인";
        Debug.Log($"{_logClass} {_lastResult}");
        _isBusy = false;
    }

    // ──────────────────────────────────────────────
    // V-08: DismissAll → Dim 비활성화 확인
    // ──────────────────────────────────────────────

    private async UniTaskVoid TestV08DismissAllAsync()
    {
        _isBusy = true;
        Debug.Log($"{_logClass} [V-08] 팝업 표시 — 2초 후 DismissAll 자동 호출.");

        var request = new PopupRequest(
            title:   "V-08 DismissAll",
            message: "2초 후 자동으로 닫힙니다.\nDimBackground가 비활성화되면 PASS."
        );

        var popupTask = _popupManager.ShowConfirmAsync(request);
        await UniTask.Delay(2000);

        _popupManager.DismissAll();
        var result = await popupTask;

        var pass    = result == false; // DismissAll → false 기대
        _lastResult = pass ? "[V-08] PASS — DismissAll 정상 동작" : $"[V-08] FAIL — 기대: false, 실제: {result}";
        Debug.Log($"{_logClass} {_lastResult}");
        _isBusy = false;
    }

    // ──────────────────────────────────────────────
    // V-09: Dim 배경이 하단 UI 터치 차단하는지 수동 확인
    // ──────────────────────────────────────────────

    private async UniTaskVoid TestV09DimRaycastAsync()
    {
        _isBusy = true;
        Debug.Log($"{_logClass} [V-09] Dim Raycast 확인 — 팝업 뒤의 UI 버튼이 클릭되지 않아야 합니다.");

        var request = new PopupRequest(
            title: "V-09 Dim Raycast",
            message: "팝업 뒤쪽 UI(이 테스트 버튼들)를 클릭해 보세요.\n클릭이 차단되면 PASS.\n'확인'을 눌러 닫으세요."
        );
        await _popupManager.ShowConfirmAsync(request);

        _lastResult = "[V-09] 팝업 닫힘 — Dim Raycast 차단 수동 확인 필요";
        Debug.Log($"{_logClass} {_lastResult}");
        _isBusy = false;
    }
}
