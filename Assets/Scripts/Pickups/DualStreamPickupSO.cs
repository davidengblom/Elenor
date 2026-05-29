using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/DualStream", fileName = "Pickup_DualStream")]
    public class DualStreamPickupSO : WeaponModifierSO {
        public override System.Type ModifierComponentType => typeof(DualStreamModifier);

        public override void ConfigureModifier(IBehaviorModifier modifier, int level) {
            // DualStream reads Level from the modifier component.
        }
    }
}