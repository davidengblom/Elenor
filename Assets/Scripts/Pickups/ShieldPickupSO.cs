using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Shield", fileName = "Pickup_Shield")]
    public class ShieldPickupSO : LevelablePlayerModifierSO<ShieldModifier, ShieldPickupSO.LevelData> {
        [System.Serializable]
        public struct LevelData {
            [Tooltip("Hits absorbed before the shield fully breaks.")]
            public int maxHits;
            [Tooltip("Seconds to fully regenerate after breaking.")]
            public float regenSeconds;
        }

        protected override void ApplyLevel(ShieldModifier modifier, LevelData data) {
            modifier.Configure(data.maxHits, data.regenSeconds);
        }
    }
}