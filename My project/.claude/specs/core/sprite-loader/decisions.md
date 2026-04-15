# SpriteLoader — Decisions

D-01 [DECISION] ActionOrderSlotView에서 AssetDatabase.LoadAssetAtPath 제거
     → SetPortrait(Sprite) 메서드로 대체. BattlePresenter에서 PreloadSpritesAsync 시
       portrait 캐싱 후 ApplyPortraitsToActionOrder() 호출.
     이유: 에디터 전용 AssetDatabase는 빌드에서 동작하지 않으며 ISpriteLoader로 일원화.

D-02 [DECISION] DialogueView.ShowDialogue 시그니처 변경 (portrait 파라미터 추가)
     → ShowDialogue(EventDialogue dialogue, Sprite portrait) 로 변경.
       EventPresenter.ShowCurrentDialogueAsync()에서 대사별 portrait를 async 로드 후 전달.
     이유: Resources.Load 제거 — 동기 I/O + Resources 폴더 의존성 제거.

D-03 [DECISION] ShopItemSlotView에서 Potion.Sprite 직접 참조 제거
     → SetIcon(Sprite sprite) 메서드 추가. MaintenancePresenter.ShowShopPanelAsync()에서
       ISpriteLoader로 potion sprite 로드 후 SetShopSlotIcon() 체인으로 전달.
     이유: SO의 Sprite 직접 필드를 SpriteKey(string)로 교체한 것과 일관성 유지.

D-04 [DECISION] StagePresenter 초기화 시 노드 아이콘을 RenderNodes 호출 전에 로드
     → InitializeAsync()에서 GetNodeTypeIconKeys() → LoadSpriteAsync 루프 →
       SetNodeTypeIcons() 완료 후 RenderNodes() 호출 순서 보장.
     이유: 아이콘 로드 전에 RenderNodes를 호출하면 빈 아이콘으로 렌더링됨.

D-05 [DECISION] EventPresenter 대사 portrait를 대사 진행마다 on-demand 로드
     → ShowCurrentDialogueAsync()에서 dialogue.PortraitSpriteKey 로드.
       AddressableSpriteLoader 내부 캐시로 중복 로드 비용 없음.
     이유: 전체 대사 pre-load는 UseCase API 변경(GetAllDialogues 등) 필요 — 최소 침투.

D-06 [BACKLOG] HP 단계별 스프라이트 교체 미구현
     → 현재 BattlePresenter.PreloadSpritesAsync는 기본 unitSprite만 로드.
       HP 50%, HP 0% 전환용 별도 SpriteKey가 BattleParticipant에 없음.
     추후 Patch에서 SpriteKey 필드 추가 및 교체 로직 구현 필요.

D-07 [BACKLOG] MainScene 배경 SpriteKey 미정의
     → EvolutionNodeSO에 배경 전용 SpriteKey 필드가 없음.
       현재 MainPresenter는 배경 로딩 코드 없이 넘어감.
     추후 Spec 개정 또는 별도 SO에서 배경 키 정의 필요.

D-08 [DECISION] EvolutionTreePresenter.HandleNodeClicked를 async fire-and-forget으로 변환
     → HandleNodeClicked(string) → HandleNodeClickedAsync(string)로 분리.
       노드 클릭 시 _nodeIconCache(Dictionary<string,Sprite>)에서 아이콘 조회,
       GetSkillsForNode()로 스킬 목록을 받아 각 스킬 아이콘을 LoadSpriteAsync로 로드 후
       NodeDescriptionData.NodeIcon / SkillIcons에 주입.
     이유: InitializeAsync에서 이미 로드한 아이콘을 캐시로 재사용 — 중복 I/O 없음.

D-09 [DECISION] EventPresenter 배경 키 소스를 EventSO 우선으로 변경
     → 기존: PendingEventContext.BackgroundSpriteKey만 참조.
       변경: EventUseCase.GetBackgroundSpriteKey()로 EventSO 키 우선 사용,
       없으면 PendingEventContext 키 fallback.
     이유: StagePresenter가 PendingEventContext 생성 시 BackgroundSpriteKey를 세팅하지
       않아 EventSO에 설정한 배경이 무시되던 버그 수정.

D-10 [DECISION] CharacterInfoPresenter에 _skillIconCache 추가 및 진화 노드 아이콘 로드
     → _skillIconCache(Sprite[]) 도입 — InitializeAsync의 스킬 로드 루프에서 캐싱.
       HandleSkillSlotClicked: null → _skillIconCache[index] 전달.
       EvolutionStageButton: SetEvolutionInfo(null, name) →
       evolutionNode.NodeIconSpriteKey 로드 후 SetEvolutionInfo(icon, name).
     이유: 스프라이트 시스템 구현 전 스킵된 TODO를 일괄 해소.

D-11 [DECISION] BattlePresenter.BuildSkillDisplayData를 async로 변환 및 스킬 아이콘 로드
     → BuildSkillDisplayData(sync) → BuildSkillDisplayDataAsync(UniTask<SkillDisplayData[]>).
       SkillDisplayData에 Sprite Icon 필드 추가.
       ProcessAllyTurn에서 await로 호출, 각 스킬 IconSpriteKey를 LoadSpriteAsync로 로드.
       SkillSelectionView.SetSkills에서 btn.GetComponent<Image>().sprite = skill.Icon 적용
       (color tint와 병용 — Usable=white, OnCooldown=gray, HpInsufficient=red).
     이유: SkillSelectionView는 ISpriteLoader 미보유 — Presenter에서 사전 로드 후 전달하는
       패턴 일관성 유지.
