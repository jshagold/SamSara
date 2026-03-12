using Core.ErrorHandling;

namespace App.Systems.ErrorHandling
{
    /// <summary>
    /// Implements IStabilityFlag. Block() sets save disallowed permanently for this process (FR-07).
    /// </summary>
    public class StabilityFlag : IStabilityFlag
    {
        private bool _isSaveAllowed = true;

        public bool IsSaveAllowed => _isSaveAllowed;

        public void Block()
        {
            _isSaveAllowed = false;
        }
    }
}
