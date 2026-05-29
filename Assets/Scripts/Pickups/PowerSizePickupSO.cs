using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/PowerSize", fileName = "Pickup_PowerSize")]
    public class PowerSizePickupSO : PlayerModifierSO {
        [System.Serializable]
        public struct LevelData {
            [Tooltip("Damage bonus as a fraction.")]
            public float damageBonus;
            [Tooltip("Speed bonus as a fraction.")]
            public float speedBonus;
            [Tooltip("Hitbox/scale bonus as a fraction.")]
            public float scaleBonus;
        }

        [SerializeField] LevelData[] levels = new LevelData[MaxLevel];

        public override System.Type ModifierComponentType => typeof(PowerSizeModifier);

        public override void ConfigureModifier(IBehaviorModifier modifier, int level) {
            if (modifier is not PowerSizeModifier powerSize) return;
            int index = Mathf.Clamp(level - 1, 0, levels.Length - 1);
            LevelData data = levels[index];
            powerSize.Configure(
                1f + data.damageBonus,
                1f + data.speedBonus,
                1f + data.scaleBonus
            );
        }

        void OnValidate() {
            if (levels == null || levels.Length != MaxLevel) {
                System.Array.Resize(ref levels, MaxLevel);
            }
        }
    }
}