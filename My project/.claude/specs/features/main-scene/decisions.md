# MainScene — Decisions

## D-01: GlobalBootstrapper에 ISceneNavigator 공개 프로퍼티 추가

- **상황:** MainPresenter가 ISceneNavigator를 생성자 주입받아야 하지만, GlobalBootstrapper에 public accessor가 없었음.
- **결정:** `GlobalBootstrapper.SceneNavigator` public 프로퍼티 추가.
- **근거:** Constitution §6에 따르면 ISceneNavigator는 GlobalBootstrapper가 소유. SceneBootstrapper/FeatureBootstrapper에서 접근할 수 있어야 함.

## D-02: CharacterRunData.MaxHp 부재

- **상황:** MainViewModel에 MaxHp가 필요하지만 CharacterRunData에 MaxHp 필드가 없음.
- **결정:** MaxHp는 캐릭터 스탯의 Hp 스탯 수치를 초기값으로 하되, 이벤트/정비 등을 통해 가변되는 수치. 현재 Hp는 전투/이벤트로 증감하는 실시간 수치(0 ~ MaxHp 범위). 두 값 모두 CharacterRunData에 별도 필드로 관리 필요.

## D-03: IsMerchantActive 항상 false (OQ-02)

- **상황:** Merchant NPC 활성 조건이 아직 미정의.
- **결정:** v1에서는 항상 false 반환. OQ-02 해결 후 로직 추가.

## D-04: GlobalBootstrapper에 Main 씬 자동 전환 추가

- **상황:** Bootstrap 씬에서 Play 시 초기화 완료 후 아무 씬으로도 전환되지 않았음. GlobalBootstrapper.InitializeAsync()가 _initTcs.TrySetResult()만 호출하고 종료.
- **결정:** 초기화 완료(Step 5) 직후 `_sceneNavigator.NavigateToAsync(SceneKey.Main)`을 호출하는 Step 6 추가.
- **근거:** Bootstrap 씬은 초기화 전용이며, 완료 후 Main 씬으로 진입하는 것이 자연스러운 앱 흐름.

## D-06: Patch-001 — Merchant NPC 코드 전체 제거 (Specify v1.2.0)

[DECISION] Specify v1.2.0에서 Merchant NPC가 MaintenanceScene으로 이동함에 따라 MainScene의 모든 Merchant 관련 코드를 제거.
- 삭제: `MerchantButtonView.cs`, `V07_MerchantDefaultStateValidation.cs`
- 수정: `MainViewModel.IsMerchantActive` 필드 제거, `MainUseCase.GetMainViewModel()` OQ-02 하드코딩 제거
- 수정: `MainView` — `_merchantButtonView` 필드, `OnMerchantClicked` 이벤트, `SetMerchantVisible()` 제거
- 수정: `MainPresenter` — `HandleMerchantClicked()`, `SetMerchantVisible()` 호출, `OnMerchantClicked` 구독 제거
- 수정: `V03_MainViewModelValidation` — `merchantPass` 검증 라인 제거

## D-07: Patch-001 — Main.unity 씬에서 MerchantButtonView GameObject 수동 제거 필요

[BACKLOG] `Main.unity` 씬 파일에 MerchantButtonView 컴포넌트 참조가 남아 있음. `MerchantButtonView.cs`가 삭제되었으므로 Unity Editor에서 "Missing Script" 경고가 발생할 수 있음.
- Unity Editor에서 Main.unity 열기 → MerchantButtonView 컴포넌트가 붙은 GameObject 삭제 필요.
- 씬 YAML 직접 편집은 파일 손상 위험이 있으므로 Editor에서 수행.

## D-05: 미구현 씬(Stage, Maintenance, CharacterInfo) 네비게이션 에러

- **상황:** V-06 검증 시 Stage/Maintenance/CharacterInfo 버튼 클릭 후 씬 전환에서 "Scene couldn't be loaded" 에러 발생.
- **결정:** 해당 씬 파일이 아직 미생성 상태이므로 예상된 에러. 빈 씬 파일을 생성하고 Build Settings에 등록하면 해결. 버튼 클릭 → 캐릭터 애니메이션 → 이벤트 발화 흐름 자체는 정상 동작 확인됨.
- **근거:** 각 씬은 별도 Phase에서 구현 예정. SceneKey.ToString()과 씬 파일 이름이 정확히 일치해야 함.
