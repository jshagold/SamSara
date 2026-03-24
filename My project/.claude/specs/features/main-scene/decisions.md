# MainScene — Decisions

## D-01: GlobalBootstrapper에 ISceneNavigator 공개 프로퍼티 추가

- **상황:** MainPresenter가 ISceneNavigator를 생성자 주입받아야 하지만, GlobalBootstrapper에 public accessor가 없었음.
- **결정:** `GlobalBootstrapper.SceneNavigator` public 프로퍼티 추가.
- **근거:** Constitution §6에 따르면 ISceneNavigator는 GlobalBootstrapper가 소유. SceneBootstrapper/FeatureBootstrapper에서 접근할 수 있어야 함.

## D-02: CharacterRunData.MaxHp 부재

- **상황:** MainViewModel에 MaxHp가 필요하지만 CharacterRunData에 MaxHp 필드가 없음.
- **결정:** 현재는 Hp 값을 MaxHp로도 사용. CharacterRunData에 MaxHp 필드 추가 시 교체 필요.

## D-03: IsMerchantActive 항상 false (OQ-02)

- **상황:** Merchant NPC 활성 조건이 아직 미정의.
- **결정:** v1에서는 항상 false 반환. OQ-02 해결 후 로직 추가.

## D-04: GlobalBootstrapper에 Main 씬 자동 전환 추가

- **상황:** Bootstrap 씬에서 Play 시 초기화 완료 후 아무 씬으로도 전환되지 않았음. GlobalBootstrapper.InitializeAsync()가 _initTcs.TrySetResult()만 호출하고 종료.
- **결정:** 초기화 완료(Step 5) 직후 `_sceneNavigator.NavigateToAsync(SceneKey.Main)`을 호출하는 Step 6 추가.
- **근거:** Bootstrap 씬은 초기화 전용이며, 완료 후 Main 씬으로 진입하는 것이 자연스러운 앱 흐름.

## D-05: 미구현 씬(Stage, Maintenance, CharacterInfo) 네비게이션 에러

- **상황:** V-06 검증 시 Stage/Maintenance/CharacterInfo 버튼 클릭 후 씬 전환에서 "Scene couldn't be loaded" 에러 발생.
- **결정:** 해당 씬 파일이 아직 미생성 상태이므로 예상된 에러. 빈 씬 파일을 생성하고 Build Settings에 등록하면 해결. 버튼 클릭 → 캐릭터 애니메이션 → 이벤트 발화 흐름 자체는 정상 동작 확인됨.
- **근거:** 각 씬은 별도 Phase에서 구현 예정. SceneKey.ToString()과 씬 파일 이름이 정확히 일치해야 함.
