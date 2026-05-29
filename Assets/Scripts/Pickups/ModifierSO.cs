using UnityEngine;

namespace Elenor {
    public abstract class ModifierSO : PickupSO {
        public override PickupCategory Category => PickupCategory.Modifier;
        public abstract System.Type ModifierComponentType { get; }
        public abstract void ConfigureModifier(IBehaviorModifier modifier, int level);
    }
}