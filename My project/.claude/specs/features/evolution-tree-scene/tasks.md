# EvolutionTreeScene — Tasks

**Version:** 1.0.0 | **Date:** 2026-04-06 | **Status:** Ready for Claude Code
**Based on:** Specify v1.0.0 / Plan v1.0.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- Read CLAUDE.md first
- CharacterRunData.IsReincarnationPending (bool) field MUST exist
- Read the following existing files:
    - Assets/_Game/Features/Character/Data/CharacterRunData.cs
    - Assets/_Game/Features/Character/Data/CharacterRunRepository.cs
    - Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs
    - Assets/_Game/Features/Character/MasterData/CharacterStatsSO.cs
    - Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs
    - Assets/_Game/Features/Character/MasterData/StatType.cs
    - Assets/_Game/Features/Character/MasterData/StatCondition.cs
    - Assets/_Game/Features/Skill/Domain/ISkillMasterDataRepository.cs
    - Assets/_Game/Core/MasterData/SkillSO.cs
    - Assets/_Game/App/GameContext.cs
    - Assets/_Game/Core/Navigation/ISceneNavigator.cs
    - Assets/_Game/Core/Navigation/SceneKey.cs
    - Assets/_Game/Core/Popup/IPopupManager.cs
    - Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterInfoSceneBootstrapper.cs (pattern reference)
- Create folder: Assets/_Game/Features/EvolutionTreeScene/
- Create folder: .claude/specs/features/evolution-tree-scene/

---

## Files to Create/Modify

| # | File | Path | Action |
|---|---|---|---|
| 1 | SceneKey.cs | Assets/_Game/Core/Navigation/ | Modify (check first) |
| 2 | TreeLayoutCalculator.cs | Assets/_Game/Features/EvolutionTreeScene/Domain/ | Create |
| 3 | EvolutionTreeUseCase.cs | Assets/_Game/Features/EvolutionTreeScene/Domain/ | Create |
| 4 | BackButtonView.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/TopBar/ | Create |
| 5 | OptionButtonView.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/TopBar/ | Create |
| 6 | EvolutionNodeView.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/TreeArea/ | Create |
| 7 | NodeConnectionView.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/TreeArea/ | Create |
| 8 | TreeScrollView.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/TreeArea/ | Create |
| 9 | NodeDescriptionPopupView.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/Popup/ | Create |
| 10 | ReincarnationButtonView.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/ | Create |
| 11 | EvolutionTreeView.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/ | Create |
| 12 | EvolutionTreePresenter.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/ | Create |
| 13 | EvolutionTreeSceneBootstrapper.cs | Assets/_Game/Features/EvolutionTreeScene/Presentation/ | Create |
| 14 | decisions.md | .claude/specs/features/evolution-tree-scene/ | Create (empty) |

---

## Implementation Tasks

### Task 1 — SceneKey.cs Modification (Check First)

Read SceneKey enum and verify:
- If EvolutionTree already exists, no change needed.
- If not, add EvolutionTree.
- Also check for Splash — add if missing (needed for forced reincarnation scene transition).

---

### Task 2 — TreeLayoutCalculator.cs

- Namespace: Samsara.Features.EvolutionTreeScene.Domain
- Pure C# class (not MonoBehaviour). Complies with section 2 Data/Logic separation.
- _logClass required (section 8).

**Role:** Receives EvolutionNodeSO array, calculates tree layout (per-node position coordinates). No View/Unity dependency.

**Constants:**
- NodeSpacingX (float): horizontal spacing within same depth
- NodeSpacingY (float): vertical spacing between depths

**Return structs (defined in same file):**
- NodeLayoutData: string NodeId, float X, float Y, int Depth
- ConnectionLayoutData: string ParentNodeId, string ChildNodeId
- TreeLayoutResult: NodeLayoutData[] Nodes, ConnectionLayoutData[] Connections, float ContentWidth, float ContentHeight

**Method:**
- CalculateLayout(EvolutionNodeSO[] allNodes) -> TreeLayoutResult

**Layout algorithm:**
- Root node detection: find node not referenced in any other node's NextNodes.
- BFS depth calculation. Depth jumps allowed (e.g., depth 0 -> depth 4). Intermediate empty depths remain as blank space.
- Per-depth horizontal center alignment.
- Calculate ContentWidth/ContentHeight to fit full tree.
- Throw InvalidOperationException if root not found (section 7 Fail Fast).

---

### Task 3 — EvolutionTreeUseCase.cs

- Namespace: Samsara.Features.EvolutionTreeScene.Domain
- Pure C# class (not MonoBehaviour). Complies with section 2.
- Constructor injection: ICharacterRunRepository, ISkillMasterDataRepository, EvolutionNodeSO[] (MasterData cache array).
- _logClass required (section 8).

**Methods:**

- GetCurrentNodeId(): Returns CharacterRunData.currentEvolutionNodeId.
- GetCurrentStats(): Returns HP, Strength, Toughness, Agility from CharacterRunData. Return type EvolutionTreeStatsData struct (defined in same file).
- GetAllNodes(): Returns EvolutionNodeSO[] from constructor.
- GetSkillsForNode(int[] skillIds): Returns SkillSO[] from ISkillMasterDataRepository.
- FindCurrentNode(): Searches EvolutionNodeSO[] by currentEvolutionNodeId. Throws InvalidOperationException if not found (section 7).
- CheckUnlockConditions(EvolutionNodeSO node): Compares current stats with node.UnlockConditions. Returns true if all conditions met.
- ClassifyNodeState(EvolutionNodeSO node, EvolutionNodeSO currentNode): Returns NodeState enum (defined in same file: Current, Evolvable, Reachable, Locked, Hidden).
    - node == currentNode -> Current
    - In currentNode.NextNodes + isHidden and not unlocked -> Hidden
    - In currentNode.NextNodes + CheckUnlockConditions == true -> Evolvable
    - In currentNode.NextNodes + CheckUnlockConditions == false -> Reachable
    - None of the above -> Locked
- ExecuteEvolutionAsync(EvolutionNodeSO targetNode): Returns UniTask.
    - Set CharacterRunData.currentEvolutionNodeId = targetNode.NodeId
    - Reset CharacterRunData stats to targetNode.BaseStats values
    - Set CharacterRunData.maxActionPoints = targetNode.MaxActionPoints
    - Set ICharacterRunRepository dirty flag + call SaveDataAsync() (section 9)
- ExecuteReincarnationAsync(): Returns UniTask.
    - Set CharacterRunData.IsReincarnationPending = true
    - Set ICharacterRunRepository dirty flag + call SaveDataAsync() (section 9)

---

### Task 4 — BackButtonView.cs

- MonoBehaviour. Uses Button component.
- [SerializeField] private Button _button.
- OnBackClicked event (System.Action).
- Reset(): _button = GetComponentInChildren<Button>() (section 7).
- _logClass required (section 8).

---

### Task 5 — OptionButtonView.cs

- MonoBehaviour. Uses Button component.
- [SerializeField] private Button _button.
- OnOptionClicked event (System.Action).
- Reset(): _button = GetComponentInChildren<Button>() (section 7).
- _logClass required (section 8).

---

### Task 6 — EvolutionNodeView.cs

- MonoBehaviour. Individual tree node UI.
- [SerializeField] private Button _button.
- [SerializeField] private Image _nodeIcon.
- [SerializeField] private Image _highlightBorder: current node emphasis.
- [SerializeField] private Image _dimOverlay: locked node dim.
- [SerializeField] private GameObject _evolvableIndicator: evolvable state indicator.
- NodeId property (string).
- Setup(string nodeId, Sprite icon) method.
- SetNodeState(NodeState state) method: toggle visual elements by state.
    - Current: highlightBorder active, dimOverlay inactive
    - Evolvable: evolvableIndicator active, dimOverlay inactive
    - Reachable: all default
    - Locked: dimOverlay active
    - Hidden: replace nodeIcon with question mark sprite, dimOverlay active
- SetQuestionMarkSprite(Sprite questionMark) method: for hidden nodes.
- OnNodeClicked(string nodeId) event (System.Action<string>).
- Reset(): auto-assign child components (section 7).
- _logClass required (section 8).

---

### Task 7 — NodeConnectionView.cs

- MonoBehaviour. Node connection line UI.
- [SerializeField] private Image _lineImage.
- SetConnection(Vector2 startPos, Vector2 endPos) method: RectTransform-based position/rotation/size.
- Reset(): _lineImage = GetComponentInChildren<Image>() (section 7).
- _logClass required (section 8).

---

### Task 8 — TreeScrollView.cs

- MonoBehaviour. ScrollRect-based tree container.
- [SerializeField] private ScrollRect _scrollRect.
- [SerializeField] private RectTransform _content.
- [SerializeField] private EvolutionNodeView _nodePrefab.
- [SerializeField] private NodeConnectionView _connectionPrefab.
- **No layout logic** — receives calculated results from TreeLayoutCalculator.

**Methods:**
- BuildTree(TreeLayoutResult layoutResult): place nodes and connections from calculated layout.
    - Set content size to layoutResult.ContentWidth/ContentHeight.
    - For each NodeLayoutData: Instantiate _nodePrefab, set position, store in list.
    - For each ConnectionLayoutData: Instantiate _connectionPrefab, call SetConnection.
- GetNodeView(string nodeId): return EvolutionNodeView by nodeId.
- ScrollToNode(string nodeId): auto-scroll viewport to node position.
- GetAllNodeViews(): return all created EvolutionNodeView array.
- Reset(): _scrollRect = GetComponentInChildren<ScrollRect>() (section 7).
- _logClass required (section 8).

---

### Task 9 — NodeDescriptionPopupView.cs

- MonoBehaviour. Node description popup.
- [SerializeField] private GameObject _popupRoot.
- [SerializeField] private Image _nodeIconImage.
- [SerializeField] private TMP_Text _characterNameText.
- [SerializeField] private TMP_Text _statsText.
- [SerializeField] private TMP_Text _unlockConditionsText.
- [SerializeField] private Transform _skillIconContainer.
- [SerializeField] private Image[] _skillIcons: max 3.
- [SerializeField] private Button _evolveButton.
- [SerializeField] private Button _closeButton.
- Show(NodeDescriptionData data) method. NodeDescriptionData struct (defined in same file):
    - Sprite nodeIcon, string characterName, string statsText, string conditionsText, Sprite[] skillIcons, bool canEvolve, bool isHiddenLocked
- Hide() method.
- OnEvolveClicked event (System.Action).
- OnCloseClicked event (System.Action).
- Reset(): auto-assign child components (section 7).
- _logClass required (section 8).

---

### Task 10 — ReincarnationButtonView.cs

- MonoBehaviour. Uses Button component.
- [SerializeField] private Button _button.
- OnReincarnationClicked event (System.Action).
- Reset(): _button = GetComponentInChildren<Button>() (section 7).
- _logClass required (section 8).

---

### Task 11 — EvolutionTreeView.cs

- MonoBehaviour. Scene root View.
- All child Views as [SerializeField] private:
    - BackButtonView _backButtonView
    - OptionButtonView _optionButtonView
    - TreeScrollView _treeScrollView
    - NodeDescriptionPopupView _nodeDescriptionPopupView
    - ReincarnationButtonView _reincarnationButtonView
- Public properties for each child View.
- Reset(): auto-assign child components (section 7).
- _logClass required (section 8).

---

### Task 12 — EvolutionTreePresenter.cs

- Pure C# class (not MonoBehaviour).
- Constructor injection: EvolutionTreeUseCase, TreeLayoutCalculator, EvolutionTreeView, ISceneNavigator, IPopupManager.
- _logClass required (section 8).
- _allNodes, _currentNode cache fields.

**Initialize() method:**
- _allNodes = UseCase.GetAllNodes()
- _currentNode = UseCase.FindCurrentNode()
- TreeLayoutCalculator.CalculateLayout(_allNodes) -> TreeLayoutResult
- EvolutionTreeView.TreeScrollView.BuildTree(layoutResult)
- For each EvolutionNodeView: UseCase.ClassifyNodeState() -> SetNodeState()
- TreeScrollView.ScrollToNode(_currentNode.NodeId)
- Subscribe events: all EvolutionNodeView OnNodeClicked, EvolveButton, CloseButton, ReincarnationButton, BackButton

**Event handlers:**
- Node tap(nodeId) -> find EvolutionNodeSO -> compose NodeDescriptionData (handle hidden locked) -> NodeDescriptionPopupView.Show()
- Evolve button tap -> IPopupManager confirmation popup -> on confirm: UseCase.ExecuteEvolutionAsync(targetNode).Forget() -> ISceneNavigator.LoadScene(SceneKey.Main)
- Reincarnation button tap -> IPopupManager confirmation popup -> on confirm: UseCase.ExecuteReincarnationAsync().Forget() -> ISceneNavigator.LoadScene(SceneKey.Splash)
- Popup close -> NodeDescriptionPopupView.Hide()
- Back button -> ISceneNavigator.LoadScene(SceneKey.CharacterInfo)

**Dispose() method:** Unsubscribe all events. Use ?. operator (section 8 Safe Cleanup).

---

### Task 13 — EvolutionTreeSceneBootstrapper.cs

- MonoBehaviour. Start() async initialization (section 3).
- [SerializeField] private EvolutionTreeView _evolutionTreeView.
- await GlobalBootstrapper.Instance.InitializationTask.
- Acquire ICharacterRunRepository, ISkillMasterDataRepository, EvolutionNodeSO[] from GameContext.
- Create TreeLayoutCalculator instance.
- Create EvolutionTreeUseCase instance.
- Acquire ISceneNavigator, IPopupManager from GlobalBootstrapper.
- Create EvolutionTreePresenter instance with injection.
- Call EvolutionTreePresenter.Initialize().
- OnDestroy(): _presenter?.Dispose() (section 8 Safe Cleanup).
- _logClass required (section 8).

---

### Task 14 — decisions.md

Create empty file at .claude/specs/features/evolution-tree-scene/decisions.md

---

## Validation

| # | Item | How to Check |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | SceneKey enum contains EvolutionTree entry | File check |
| V-03 | EvolutionTreeSceneBootstrapper placed in scene, no errors on Play | Editor check |
| V-04 | Nodes arranged in tree structure in tree view | Editor Play check |
| V-05 | 4-directional drag navigation works | Editor Play check |
| V-06 | Current node shows highlight | Editor Play check |
| V-07 | Node state visual distinction applied | Editor Play check |
| V-08 | Node tap shows description popup, close works | Editor Play check |
| V-09 | Hidden locked node shows "???", stats/skills hidden, conditions only | Editor Play check |
| V-10 | Evolvable node popup shows active evolve button | Editor Play check |
| V-11 | Evolve button -> confirm popup -> confirm -> MainScene transition | Editor Play check |
| V-12 | After evolution, stats reset to new node's baseStats | Save file check |
| V-13 | Reincarnation -> confirm popup -> confirm -> SplashScene transition | Editor Play check |
| V-14 | After reincarnation, isReincarnationPending saved as true | Save file check |
| V-15 | Back button -> CharacterInfoScene transition | Editor Play check |
| V-16 | Connection lines display correctly between nodes | Editor Play check |

---

## Manual Work (After Claude Code Implementation)

| # | Task |
|---|---|
| M-01 | Create Assets/_Game/Scenes/EvolutionTree.unity scene file |
| M-02 | Place Main Camera + Canvas (Screen Space - Camera) |
| M-03 | Place EvolutionTreeSceneBootstrapper at scene root, connect EvolutionTreeView in Inspector |
| M-04 | Connect all View [SerializeField] fields in Inspector |
| M-05 | Add EvolutionTree scene to Build Settings |
| M-06 | Configure ScrollRect (Horizontal + Vertical enabled, Content RectTransform placed) |
| M-07 | Create EvolutionNodeView Prefab — Structure: Root(Button) > NodeIcon(Image) + HighlightBorder(Image, default inactive) + DimOverlay(Image, default inactive) + EvolvableIndicator(GameObject, default inactive) |
| M-08 | Create NodeConnectionView Prefab — Structure: Root > LineImage(Image, Stretch approach) |
| M-09 | Compose NodeDescriptionPopupView popup UI — Structure: PopupRoot > NodeIcon(Image) + NameText(TMP) + StatsText(TMP) + ConditionsText(TMP) + SkillIconContainer(Transform, 3 child Images) + EvolveButton(Button) + CloseButton(Button) |
| M-10 | Place ReincarnationButton at screen corner |
| M-11 | Prepare placeholder Sprites (node icon, question mark icon, skill icons) |
| M-12 | Test SO assets: evolution tree structure (root -> 2-3 children, including depth jump) — editor script or manual creation |

---

## Claude Code Delivery Guide

- Run claude from project root
- CLAUDE.md auto-loaded
- Pass .claude/specs/features/evolution-tree-scene/tasks.md for sequential implementation
- Record judgment calls in .claude/specs/features/evolution-tree-scene/decisions.md
- DO NOT create files outside Assets/_Game/ (except decisions.md)