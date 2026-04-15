using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.Domain;
using Samsara.Features.Inventory.Data;
using Samsara.Features.Inventory.Domain;
using Samsara.Features.Shop.Domain;
using Samsara.Features.Shop.MasterData;
using UnityEngine;

/// <summary>
/// [DEV ONLY] InventorySystem 단위 테스트 (V-01 ~ V-10).
/// Bootstrap 씬 빈 GameObject에 부착 후 Play.
/// Console에서 [InventorySystemTest] === ALL RESULTS === 이후 결과 확인.
/// </summary>
public class InventorySystemTest : MonoBehaviour
{
    private readonly string _logClass = "[InventorySystemTest]";
    private string _savePath;
    private GameContext _gameContext;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);   // 씬 전환 생존
        RunAllTestsAsync().Forget();
    }

    private async UniTaskVoid RunAllTestsAsync()
    {
        // GlobalBootstrapper.Instance null 가드 (Start 실행 순서 불보장)
        while (GlobalBootstrapper.Instance == null)
            await UniTask.Yield();

        Debug.Log($"{_logClass} Bootstrap 완료 대기 중...");
        await GlobalBootstrapper.Instance.InitializationTask;

        _gameContext = GlobalBootstrapper.Instance.GameContext;
        _savePath    = Application.persistentDataPath + "/inventory_run_data.json";

        Debug.Log($"{_logClass} === 테스트 시작 === (savePath: {_savePath})");

        var results = new List<string>();

        results.Add(await TestV01Async());
        results.Add(await TestV02Async());
        results.Add(await TestV03Async());
        results.Add(await TestV04Async());
        results.Add(await TestV05Async());
        results.Add(await TestV06Async());
        results.Add(await TestV07Async());
        results.Add(TestV08());
        results.Add(TestV09());
        results.Add(await TestV10Async());

        // 테스트 후 파일 정리
        CleanupFile();

        // 최종 결과 출력
        Debug.Log($"{_logClass} === ALL RESULTS ===");
        foreach (var r in results)
            Debug.Log($"{_logClass} {r}");
    }

    // ─────────────────────────────────────────────────────────────────
    // V-01 — LoadDataAsync 기본값 초기화
    // 저장 파일 없는 상태에서 LoadDataAsync → 3슬롯 모두 IsEmpty 확인
    // ─────────────────────────────────────────────────────────────────
    private async UniTask<string> TestV01Async()
    {
        const string label = "V-01";
        try
        {
            CleanupFile();
            var repo = new InventoryRepository();
            await repo.LoadDataAsync();

            var slots = repo.GetSlots();
            if (slots == null || slots.Length != 3)
                return $"{label} FAIL — GetSlots() 슬롯 수={slots?.Length} (기대: 3)";

            foreach (var s in slots)
            {
                if (!s.IsEmpty || s.ItemId != -1 || s.Quantity != 0)
                    return $"{label} FAIL — 슬롯이 빈 상태가 아님: ItemId={s.ItemId}, Quantity={s.Quantity}";
            }

            return $"{label} PASS";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // V-02 — Dirty Flag: 로드 직후 SaveDataAsync 호출 시 파일 미생성
    // _isDirty=false이면 SaveDataAsync는 파일 쓰기를 건너뜀
    // ─────────────────────────────────────────────────────────────────
    private async UniTask<string> TestV02Async()
    {
        const string label = "V-02";
        try
        {
            CleanupFile();
            var repo = new InventoryRepository();
            await repo.LoadDataAsync();  // 파일 없음 → 기본값, _isDirty=false

            await repo.SaveDataAsync();  // _isDirty=false → 건너뜀

            // 파일이 생성되지 않았으면 Dirty Flag가 올바르게 동작한 것
            bool fileNotCreated = !File.Exists(_savePath);
            return fileNotCreated
                ? $"{label} PASS"
                : $"{label} FAIL — Dirty Flag=false인데 파일이 생성됨";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // V-03 — 같은 아이템 스택: AddItem(1,2) 후 AddItem(1,3) → Quantity=5
    // ─────────────────────────────────────────────────────────────────
    private async UniTask<string> TestV03Async()
    {
        const string label = "V-03";
        try
        {
            CleanupFile();
            var repo = new InventoryRepository();
            await repo.LoadDataAsync();
            var uc = CreateUseCase(repo);

            await uc.AddItem(1, 2);
            await uc.AddItem(1, 3);

            foreach (var s in repo.GetSlots())
                if (s.ItemId == 1 && s.Quantity == 5)
                    return $"{label} PASS";

            int found = -1;
            foreach (var s in repo.GetSlots()) if (s.ItemId == 1) { found = s.Quantity; break; }
            return $"{label} FAIL — Quantity={found} (기대: 5)";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // V-04 — 빈 슬롯 신규 등록: AddItem(1,1) → slot[0] ItemId=1, Quantity=1
    // ─────────────────────────────────────────────────────────────────
    private async UniTask<string> TestV04Async()
    {
        const string label = "V-04";
        try
        {
            CleanupFile();
            var repo = new InventoryRepository();
            await repo.LoadDataAsync();
            var uc = CreateUseCase(repo);

            var result = await uc.AddItem(1, 1);
            if (result != AddItemResult.Success)
                return $"{label} FAIL — AddItemResult={result} (기대: Success)";

            var slot0 = repo.GetSlots()[0];
            bool pass = slot0.ItemId == 1 && slot0.Quantity == 1;
            return pass
                ? $"{label} PASS"
                : $"{label} FAIL — slot[0]: ItemId={slot0.ItemId}, Quantity={slot0.Quantity}";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // V-05 — InventoryFull: 3종 등록 후 4번째 AddItem → InventoryFull
    // ─────────────────────────────────────────────────────────────────
    private async UniTask<string> TestV05Async()
    {
        const string label = "V-05";
        try
        {
            CleanupFile();
            var repo = new InventoryRepository();
            await repo.LoadDataAsync();
            var uc = CreateUseCase(repo);

            await uc.AddItem(1, 1);
            await uc.AddItem(2, 1);
            await uc.AddItem(3, 1);

            var result = await uc.AddItem(4, 1);
            return result == AddItemResult.InventoryFull
                ? $"{label} PASS"
                : $"{label} FAIL — AddItemResult={result} (기대: InventoryFull)";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // V-06 — UseItem: Quantity=1 사용 → 슬롯 비워짐
    // PotionSO .asset이 없으면 SKIP
    // ─────────────────────────────────────────────────────────────────
    private async UniTask<string> TestV06Async()
    {
        const string label = "V-06";
        try
        {
            // 실제 PotionSO 필요 — GameContext의 ShopMasterDataRepo 사용
            PotionSO testPotion = null;
            try
            {
                var potions = _gameContext.ShopMasterDataRepo.GetAllPotions();
                if (potions != null && potions.Length > 0)
                    testPotion = potions[0];
            }
            catch (Exception e)
            {
                return $"{label} SKIP — GetAllPotions 실패: {e.Message}";
            }

            if (testPotion == null)
                return $"{label} SKIP — Resources/MasterData에 PotionSO .asset 없음 (V-06 조건 미충족)";

            CleanupFile();
            var repo = new InventoryRepository();
            await repo.LoadDataAsync();

            // 실제 ShopMasterDataRepo + Stub 캐릭터 레포 조합
            var uc = new InventoryUseCase(repo, new StubCharacterRepo(), _gameContext.ShopMasterDataRepo);

            await uc.AddItem(testPotion.Id, 1);

            // 슬롯 인덱스 탐색
            var slots  = repo.GetSlots();
            int slotIdx = -1;
            for (int i = 0; i < slots.Length; i++)
                if (slots[i].ItemId == testPotion.Id) { slotIdx = i; break; }

            if (slotIdx == -1)
                return $"{label} FAIL — AddItem 후 슬롯에서 아이템(id={testPotion.Id}) 미발견";

            var useResult = await uc.UseItem(slotIdx);
            if (!useResult.IsSuccess)
                return $"{label} FAIL — UseItem 실패: FailReason={useResult.FailReason}";

            bool slotEmpty = repo.GetSlots()[slotIdx].IsEmpty;
            return slotEmpty
                ? $"{label} PASS (포션: {testPotion.PotionName}, stat={useResult.AffectedStat}+{useResult.EffectValue})"
                : $"{label} FAIL — UseItem 후 슬롯이 비어있지 않음";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // V-07 — DiscardItem: Quantity=5 에서 3 버리기 → Quantity=2
    // ─────────────────────────────────────────────────────────────────
    private async UniTask<string> TestV07Async()
    {
        const string label = "V-07";
        try
        {
            CleanupFile();
            var repo = new InventoryRepository();
            await repo.LoadDataAsync();
            var uc = CreateUseCase(repo);

            await uc.AddItem(1, 5);

            var slots   = repo.GetSlots();
            int slotIdx = -1;
            for (int i = 0; i < slots.Length; i++)
                if (slots[i].ItemId == 1) { slotIdx = i; break; }

            if (slotIdx == -1)
                return $"{label} FAIL — AddItem 후 슬롯 미발견";

            bool ok = await uc.DiscardItem(slotIdx, 3);
            if (!ok)
                return $"{label} FAIL — DiscardItem 반환값 false";

            int qty = repo.GetSlots()[slotIdx].Quantity;
            return qty == 2
                ? $"{label} PASS"
                : $"{label} FAIL — Quantity={qty} (기대: 2)";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // V-08 — GameContext 접근: InventoryRepo, InventoryUseCase != null
    // ─────────────────────────────────────────────────────────────────
    private string TestV08()
    {
        const string label = "V-08";
        try
        {
            bool repoOk = _gameContext.InventoryRepo    != null;
            bool ucOk   = _gameContext.InventoryUseCase != null;

            if (repoOk && ucOk)
                return $"{label} PASS";

            return $"{label} FAIL — InventoryRepo={repoOk}, InventoryUseCase={ucOk}";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // V-09 — SaveAllDataSync 에러 없이 완료
    // ─────────────────────────────────────────────────────────────────
    private string TestV09()
    {
        const string label = "V-09";
        try
        {
            _gameContext.SaveAllDataSync();
            return $"{label} PASS";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // V-10 — ResetRunData: 아이템 추가 후 Reset → 3슬롯 모두 빈 상태
    // ─────────────────────────────────────────────────────────────────
    private async UniTask<string> TestV10Async()
    {
        const string label = "V-10";
        try
        {
            CleanupFile();
            var repo = new InventoryRepository();
            await repo.LoadDataAsync();
            var uc = CreateUseCase(repo);

            await uc.AddItem(1, 2);
            await uc.AddItem(2, 3);

            repo.ResetRunData();

            var slots = repo.GetSlots();
            if (slots == null || slots.Length != 3)
                return $"{label} FAIL — ResetRunData 후 슬롯 수={slots?.Length}";

            foreach (var s in slots)
            {
                if (!s.IsEmpty || s.ItemId != -1 || s.Quantity != 0)
                    return $"{label} FAIL — 슬롯이 비어있지 않음: ItemId={s.ItemId}, Quantity={s.Quantity}";
            }

            return $"{label} PASS";
        }
        catch (Exception e)
        {
            return $"{label} FAIL — Exception: {e.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────

    /// <summary>Stub 의존성으로 InventoryUseCase 생성 (V-06 제외 공통 사용).</summary>
    private InventoryUseCase CreateUseCase(InventoryRepository repo)
        => new InventoryUseCase(repo, new StubCharacterRepo(), new StubShopMasterDataRepo());

    private void CleanupFile()
    {
        if (_savePath != null && File.Exists(_savePath))
            File.Delete(_savePath);
    }

    // ─────────────────────────────────────────────────────────────────
    // Stubs — Dev/ 전용. 의존성이 필요없는 테스트용 최소 구현.
    // ─────────────────────────────────────────────────────────────────

    private class StubCharacterRepo : ICharacterRunRepository
    {
        public CharacterRunData RunData { get; } = new CharacterRunData
        {
            Hp = 50, MaxHp = 100,
            Strength = 10, Toughness = 10, Agility = 10
        };
        public void   InitializeNewRun(RunConfigSO config) { }
        public void   MarkDirty()                          { }
        public UniTask SaveDataAsync()                     => UniTask.CompletedTask;
        public void   SaveDataSync()                       { }
        public UniTask LoadDataAsync()                     => UniTask.CompletedTask;
    }

    private class StubShopMasterDataRepo : IShopMasterDataRepository
    {
        public MerchantSO   GetMerchant(int id)  => throw new InvalidOperationException("Stub: 상인 없음");
        public MerchantSO[] GetAllMerchants()    => new MerchantSO[0];
        public PotionSO     GetPotion(int id)    => throw new InvalidOperationException("Stub: 포션 없음");
        public PotionSO[]   GetAllPotions()      => new PotionSO[0];
    }
}
