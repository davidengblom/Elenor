using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/DashMod", fileName = "Pickup_DashMod")]
    public class DashModPickupSO : LevelablePlayerModifierSO<DashModModifier, DashModPickupSO.LevelData> {
        [System.Serializable]
        public struct LevelData {
            [Tooltip("Dash damage = effective shot damage * this.")]
            public float dashDamageMultiplier;
            [Tooltip("Dash duration multiplier.")]
            public float durationMultiplier;
            [Tooltip("L3 effect: refund dash cooldown on direct kill with dash.")]
            public bool refundOnKill;
        }

        protected override void ApplyLevel(DashModModifier modifier, LevelData data) {
            modifier.Configure(data.dashDamageMultiplier, data.durationMultiplier, data.refundOnKill);
        }
    }
}