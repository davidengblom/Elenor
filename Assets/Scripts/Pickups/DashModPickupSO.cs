using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/DashMod", fileName = "Pickup_DashMod")]
    public class DashModPickupSO : PlayerModifierSO {
        [System.Serializable]
        public struct LevelData {
            [Tooltip("Dash damage = effective shot damage * this.")]
            public float dashDamageMultiplier;
            [Tooltip("Dash duration multiplier.")]
            public float durationMultiplier;
            [Tooltip("L3 effect: refund dash cooldown on direct kill with dash.")]
            public bool refundOnKill;
        }

        [SerializeField] LevelData[] levels = new LevelData[MaxLevel];

        public override System.Type ModifierComponentType => typeof(DashModModifier);

        public override void ConfigureModifier(IBehaviorModifier modifier, int level) {
            if (modifier is not DashModModifier dashMod) return;
            int index = Mathf.Clamp(level - 1, 0, levels.Length - 1);
            LevelData data = levels[index];
            dashMod.Configure(data.dashDamageMultiplier, data.durationMultiplier, data.refundOnKill);
        }

        void OnValidate() {
            if (levels == null || levels.Length != MaxLevel) {
                System.Array.Resize(ref levels, MaxLevel);
            }
        }
    }
}