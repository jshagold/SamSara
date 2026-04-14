using UnityEngine;

[CreateAssetMenu(fileName = "NewRunConfig", menuName = "Samsara/RunConfigSO")]
public class RunConfigSO : ScriptableObject
{
    private readonly string _logClass = $"[{nameof(RunConfigSO)}]";

    // ── Character ──────────────────────────────────────────────
    [SerializeField] private int _defaultEvolutionNodeId  = 1;
    [SerializeField] private int _initialGold             = 0;
    [SerializeField] private int _initialDay              = 1;
    [SerializeField] private int _initialActionPoints     = 3;
    [SerializeField] private int _initialMaxActionPoints  = 3;

    // ── Stage ───────────────────────────────────────────────────
    [SerializeField] private int _startStageId   = 1;
    [SerializeField] private int _startNodeIndex = 0;

    // ── Shop ────────────────────────────────────────────────────
    [SerializeField] private bool _initialMerchantAvailable = false;

    // ── Inventory ───────────────────────────────────────────────
    [SerializeField] private int _inventorySlotCount = 3;

    // ── Public Getters ──────────────────────────────────────────
    public int  DefaultEvolutionNodeId  => _defaultEvolutionNodeId;
    public int  InitialGold             => _initialGold;
    public int  InitialDay              => _initialDay;
    public int  InitialActionPoints     => _initialActionPoints;
    public int  InitialMaxActionPoints  => _initialMaxActionPoints;
    public int  StartStageId            => _startStageId;
    public int  StartNodeIndex          => _startNodeIndex;
    public bool InitialMerchantAvailable => _initialMerchantAvailable;
    public int  InventorySlotCount      => _inventorySlotCount;
}
