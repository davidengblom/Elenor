using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Poison", fileName = "Pickup_Poison")]
    public class PoisonPickupSO : LevelableWeaponModifierSO<PoisonModifier, PoisonPickupSO.LevelData> {
        [System.Serializable]
        public struct LevelData {
            [Tooltip("Damage per second while poisoned.")]
            public float dps;
            [Tooltip("How long poison lasts in seconds.")]
            public float duration;
            [Tooltip("L3 effect: spawn a poison cloud on death.")]
            public bool spawnCloudOnDeath;
        }

        protected override void ApplyLevel(PoisonModifier modifier, LevelData data) {
            modifier.Configure(data.dps, data.duration);
        }
    }
}