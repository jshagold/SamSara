using UnityEngine;

public class DevSceneLoader : MonoBehaviour
{
    [Header("테스트 환경 설정")]
    [Tooltip("IntroScene에 있는 @App 프리팹을 여기에")]
    [SerializeField] private GameObject _globalAppPrefab;

    private void Awake()
    {
        // 1. 이미 GlobalBootstrapper가 있는지 확인 (Intro에서 넘어온 경우)
        if (GlobalBootstrapper.Instance != null)
        {
            // 이미 있으면 나는 필요 없으니 사라짐 (중복 생성 방지)
            Destroy(gameObject);
            return;
        }

        // 2. 없다면 생성 (MainScene 바로 실행한 경우)
        if (_globalAppPrefab != null)
        {
            Debug.Log("<color=yellow>[Dev] 개발 모드: @App을 강제로 생성합니다.</color>");
            Instantiate(_globalAppPrefab);
        }
        else
        {
            Debug.LogError("[Dev] @App 프리팹이 연결되지 않았습니다!");
        }
    }
}
