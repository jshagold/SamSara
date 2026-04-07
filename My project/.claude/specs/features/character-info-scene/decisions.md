# CharacterInfoScene — Decisions

D-01 [SPEC-GAP] `EvolutionNodeSO[]` 접근 방법이 `GameContext`에 정의되지 않음.
`GameContext` 생성자가 `ScriptableObject[] masterData`를 받지만 이를 저장하거나 노출하는 프로퍼티가 없음.
`GlobalBootstrapper`가 사용하는 동일한 방식(`Resources.LoadAll<EvolutionNodeSO>("MasterData")`)을
`CharacterInfoSceneBootstrapper`에서 직접 호출하는 것으로 결정.
추후 GameContext에 `EvolutionNodeSO[]` 프로퍼티를 추가하는 방향으로 리팩토링 권장.

D-02 [DECISION] `tasks.md`에서는 Bootstrapper의 async 초기화를 `Start()`에서 실행하라고 명시하였으나,
`MainSceneBootstrapper` 기존 패턴이 `Awake()` + `InitializeAsync().Forget()`을 사용하므로
일관성을 위해 `Awake()` 방식을 채택함.

D-03 [DECISION] `StatListView.Reset()`에서 4개의 `TMP_Text` 자식을 `GetComponentsInChildren<TMP_Text>()`로
자동 할당 시 HP/Strength/Toughness/Agility 순서를 보장할 수 없음.
각 텍스트 필드는 Inspector에서 수동으로 연결해야 함. `Reset()` 메서드를 선언하지 않고 주석으로 안내.

D-04 [DECISION] `InventoryView`의 `_inventorySlots`는 `GameObject[]` 타입으로
공통 컴포넌트가 없어 `GetComponentsInChildren<>`으로 자동 할당 불가.
Inspector에서 수동 연결 필요. `Reset()` 본문을 빈 상태로 유지.

D-05 [DECISION] `IPopupManager`가 `GlobalBootstrapper`에 직접 노출되지 않음.
`GameContext.PopupManager` 프로퍼티를 통해 획득하는 것으로 결정.

D-06 [DECISION] `CharacterInfoPresenter.Initialize()`에서 캐릭터 스프라이트 및 스킬 아이콘은
Addressables 로딩이 Phase 1 범위 밖이므로 `null`로 처리.
`CharacterSpriteView.SetSprite(null)` 호출 시 Image.sprite는 null이 되어 기본 흰색 사각형으로 표시됨.
Phase 2에서 Addressables 비동기 로딩으로 교체 예정.

D-07 [DECISION] Patch-001: `GameContext` 생성자가 이미 `ScriptableObject[] masterData`를 파라미터로 받으므로
`Resources.LoadAll<EvolutionNodeSO>()` 재호출 없이 해당 배열을 순회해 `EvolutionNodeSO` 항목만 필터링함.
Resources 캐시를 통해 동일한 결과지만, 불필요한 재호출을 피하고 생성자 내부에서 일관되게 처리.

D-08 [BACKLOG] Patch-002 이후 `CharacterInfoPresenter`의 `_popupManager` 필드 및 생성자 파라미터가 미사용 상태.
패치 범위가 `CharacterInfoPresenter.cs` 단일 파일이므로 `CharacterInfoSceneBootstrapper` 수정은 제외함.
추후 `_popupManager` 의존성을 생성자와 Bootstrapper에서 제거하는 정리 작업 필요.
