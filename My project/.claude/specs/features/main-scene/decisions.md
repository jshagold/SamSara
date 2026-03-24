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
