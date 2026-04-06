D-01 [DECISION] CharacterRunData field is `EvolutionNodeId` (not `currentEvolutionNodeId` as tasks.md referenced) — used actual field name from CharacterRunData.cs
D-02 [DECISION] EvolutionNodeView uses NodeState enum from Domain layer (EvolutionTreeUseCase.cs) — NodeState is a domain concept shared across Domain and Presentation
D-03 [DECISION] Presenter passes null for Sprite parameters (nodeIcon, skillIcons) — actual sprite loading requires Addressables integration which is Manual Work scope (M-11)
D-04 [DECISION] Used `Awake()` + `InitializeAsync().Forget()` pattern matching CharacterInfoSceneBootstrapper, not `Start()` as tasks.md mentioned — consistency with existing codebase pattern
