using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(AutoSaveManager)}]";

    private GameContext _gameContext;

    // 안전장치 플래그
    private bool _isInitialized = false;

    // 변경사항이 있는지 체크하는 플래그
    // (Repository에서 데이터를 수정할 때마다 true로 바꿔줘야 함)
    private bool _isDirty = false;

    // 자동저장 주기 (초 단위)
    private const int AUTO_SAVE_INTERVAL_SEC = 180;

    public void Initialize(GameContext context)
    {
        _gameContext = context;
        _isInitialized = true;
        _isDirty = false;

        // 주기적 자동 저장 시작
        StartAutoSaveLoop().Forget();

        Debug.Log($"{_logClass} 가동 시작");
    }

    // Repository나 Presenter에서 데이터가 변경되었을 때 호출해줘야 함
    public void MakeDirty()
    {
        // 초기화 전이라도 변경 사항은 기록해두는 게 안전함 (혹은 무시)
        if (_isInitialized)
        {
            _isDirty = true;
        }
    }

    // [Public] 저장 (비동기) / 저장 실패시 예외던짐
    public async UniTask SaveAllAsync()
    {
        if (!_isInitialized) throw new InvalidOperationException("초기화 안 됨!");

        // 바뀐 게 없으면 파일 I/O를 아예 안 함
        if (!_isDirty) return;

        try
        {
            // Context에 있는 모든 리포지토리 저장
            // 병렬 저장 (하나라도 터지면 즉시 예외 전파)
            await UniTask.WhenAll(
                _gameContext.DailyStateRepo.SaveDataAsync(),
                _gameContext.InventoryRepo.SaveDataAsync()
            );
            
            // 저장 성공시 플래그 초기화
            _isDirty = false;

            Debug.Log($"{_logClass} 비동기 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass} 저장 실패! 데이터 보호를 위해 확인 필요. Error: {e.Message}");
            throw;
        }
    }

    // [Timer] 주기적 저장
    private async UniTaskVoid StartAutoSaveLoop()
    {
        var token = this.GetCancellationTokenOnDestroy();

        while (_isInitialized)
        {
            // 자동저장 주기동안 대기
            await UniTask.Delay(TimeSpan.FromSeconds(AUTO_SAVE_INTERVAL_SEC), cancellationToken: token);

            // 루프 안에서는 에러가 나도 다음 텀을 위해 멈추면 안 됨 (try-catch로 방어)
            try
            {
                if (_isDirty)
                {
                    Debug.Log(">>> [AutoSaveLoop] 주기적 저장 시도...");
                    await SaveAllAsync();
                }
            }
            catch (Exception e)
            {
                // 여기선 throw 하지 않고 로그만 남김 (게임은 계속되어야 하니까)
                Debug.LogError($"[AutoSaveLoop] 이번 주기 저장 실패 (3분 뒤 재시도): {e.Message}");
            }
        }
    }


    // =============================
    // LifeCycle 대응
    // =============================

    // [Mobile] 홈 버튼 눌러서 화면 내려갈 때
    private void OnApplicationPause(bool pauseStatus)
    {
        // pauseStatus == true : 앱이 백그라운드로 들어감 (게임 멈춤)
        // pauseStatus == false : 앱이 다시 켜짐 (게임 재개)

        if (pauseStatus && _isInitialized && _isDirty)
        {
            // 1. 로컬 저장: 무조건 성공해야 하므로 동기(Sync) 방식으로 수행
            SaveAllSync();

            // 2. 서버 동기화: 나중에(Best Effort) 해도 되므로 비동기로 던져둠
            // TODO TrySyncToServer().Forget();

            Debug.Log($"{_logClass} 앱 일시정지 -> 긴급 저장");
        }
    }

    // [PC/Editor] 앱 종료 시
    private void OnApplicationQuit()
    {
        if (_isInitialized)
        {
            // 동기식으로 저장하거나 최대한 빨리 저장해야 함
            SaveAllSync();

            Debug.Log($"{_logClass} 앱 종료 감지 -> 긴급 저장");
        }
    }

    // 비상용 동기 저장 (Main Thread Blocking)
    // Repository에 SaveDataSync() 메서드가 필요함 (이전 대화 참고)
    private void SaveAllSync()
    {
        try
        {
            // 여기서는 async/await를 쓰지 않고 즉시 파일에 씁니다.
            _gameContext.DailyStateRepo.SaveDataSync();
            _gameContext.InventoryRepo.SaveDataSync();

            _isDirty = false;
            Debug.Log(">>> [Emergency Save] 긴급 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"⛔ [Emergency Save] 긴급 저장 실패! (데이터 유실 가능성): {e.Message}");
        }
    }
}