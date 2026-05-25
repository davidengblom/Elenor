using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Poison", fileName = "Pickup_Poison")]
    public class PoisonPickupSO : WeaponModifierSO {
        [System.Serializable]
        public struct LevelData {
            [Tooltip("Damage per second while poisoned.")]
            public float dps;
            [Tooltip("How long poison lasts in seconds.")]
            public float duration;
            [Tooltip("L3 effect: spawn a poison cloud on death.")]
            public bool spawnCloudOnDeath;
        }

        [SerializeField] LevelData[] levels = new LevelData[MaxLevel];

        public override System.Type ModifierComponentType => typeof(PoisonModifier);

        public override void ConfigureModifier(IBehaviorModifier modifier, int level) {
            if (modifier is not PoisonModifier poison) return;
            int index = Mathf.Clamp(level - 1, 0, levels.Length - 1);
            LevelData data = levels[index];
            poison.Configure(data.dps, data.duration);
        }

        void OnValidate() {
            if (levels == null || levels.Length != MaxLevel) {
                System.Array.Resize(ref levels, MaxLevel);
            }
        }
    }
}