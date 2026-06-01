using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/PowerSize", fileName = "Pickup_PowerSize")]
    public class PowerSizePickupSO : LevelablePlayerModifierSO<PowerSizeModifier, PowerSizePickupSO.LevelData> {
        [System.Serializable]
        public struct LevelData {
            [Tooltip("Damage bonus as a fraction.")] public float damageBonus;
            [Tooltip("Speed bonus as a fraction.")] public float speedBonus;
            [Tooltip("Hitbox/scale bonus as a fraction.")] public float scaleBonus;
        }

        protected override void ApplyLevel(PowerSizeModifier modifier, LevelData data) {
            modifier.Configure(
                1f + data.damageBonus,
                1f + data.speedBonus,
                1f + data.scaleBonus
            );
        }
    }
}