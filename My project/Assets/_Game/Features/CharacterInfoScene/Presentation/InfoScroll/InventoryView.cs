using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Presentation.InfoScroll
{
    public class InventoryView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(InventoryView)}]";

        [SerializeField] private GameObject[] _inventorySlots;

        private void Reset()
        {
            // Slots are plain GameObjects; no shared component type to auto-detect.
            // Assign _inventorySlots manually in Inspector. See decisions.md D-04.
        }
    }
}
