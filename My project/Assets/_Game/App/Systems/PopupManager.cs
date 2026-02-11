using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class PopupManager : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(PopupManager)}]";

    [Header("Settings")]
    [SerializeField] private CommonPopupView _popupPrefab;
    [SerializeField] private Transform _canvasRoot;

    private IObjectPool<CommonPopupView> _pool;

    private void Awake()
    {
        if (_popupPrefab == null)
            throw new InvalidOperationException($"{_logClass} _popupPrefab must be assigned in Inspector.");
        if (_canvasRoot == null)
            throw new InvalidOperationException($"{_logClass} _canvasRoot must be assigned in Inspector.");

        _pool = new ObjectPool<CommonPopupView>(
            createFunc: CreatePopup,            // 없으면 어떻게 만들래?
            actionOnGet: OnGetPopup,            // 빌려줄 때 뭐 해줄까?
            actionOnRelease: OnReleasePopup,    // 반납받을 때 뭐 해줄까?
            actionOnDestroy: OnDestroyPopup,    // 진짜로 버릴 땐 어떻게 해?
            maxSize: 5                          // 최대 몇 개까지 보관할래? (팝업은 5개면 충분)
        );
    }

    // --- 풀링 규칙 정의 (콜백 methods) ---

    // 1. 생성 로직 (Instantiate)
    private CommonPopupView CreatePopup()
    {
        CommonPopupView popup = Instantiate(_popupPrefab, _canvasRoot);
        return popup;
    }
    
    // 2. 대여 로직 (Get)
    private void OnGetPopup(CommonPopupView popup)
    {
        popup.gameObject.SetActive(true);   
        popup.transform.SetAsLastSibling(); // 맨 앞으로 가져오기 (가장 위에 보임)
    }

    // 3. 반납 로직 (Release)
    private void OnReleasePopup(CommonPopupView popup)
    {
        popup.gameObject.SetActive(false);
    }

    // 4. 파괴 로직 (풀이 꽉 차서 버리는 경우)
    private void OnDestroyPopup(CommonPopupView popup)
    {
        Destroy(popup.gameObject);
    }
    // -------------


    public async UniTask<bool> ShowCommonPopup(string title, string desc, string firstText, string secondText)
    {
        // 1. 창고에서 하나 꺼내옴 (없으면 알아서 CreatePopup 실행함)
        CommonPopupView popup = _pool.Get();

        // 2. 로직 실행 (유저 응답 대기)
        // * 주의: View 내부에서 gameObject.SetActive(false)를 해도 되지만,
        //   풀링에서는 Release 할 때 꺼주는 게 정석입니다.
        bool result = await popup.ShowPopupAsync(title, desc, firstText, secondText);

        // 3. 다 썼으니 반납 (Destroy 대신 Release 사용!)
        _pool.Release(popup);

        return result;
    }
}