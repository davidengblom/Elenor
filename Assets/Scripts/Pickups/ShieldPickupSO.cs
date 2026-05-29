using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Shield", fileName = "Pickup_Shield")]
    public class ShieldPickupSO : PlayerModifierSO {
        [System.Serializable]
        public struct LevelData {
            [Tooltip("Hits absorbed before the shield fully breaks.")]
            public int maxHits;
            [Tooltip("Seconds to fully regenerate after breaking.")]
            public float regenSeconds;
        }

        [SerializeField] LevelData[] levels = new LevelData[MaxLevel];

        public override System.Type ModifierComponentType => typeof(ShieldModifier);

        public override void ConfigureModifier(IBehaviorModifier modifier, int level) {
            if (modifier is not ShieldModifier shield) return;
            int index = Mathf.Clamp(level - 1, 0, levels.Length - 1);
            LevelData data = levels[index];
            shield.Configure(data.maxHits, data.regenSeconds);
        }

        void OnValidate() {
            if (levels == null || levels.Length != MaxLevel) {
                System.Array.Resize(ref levels, MaxLevel);
            }
        }
    }
}