using UnityEngine;

namespace Elenor {
    public abstract class PickupSO : ScriptableObject {
        [SerializeField] string displayName = "Pickup";
        [SerializeField] Color displayColor = Color.white;

        public string DisplayName => displayName;
        public Color DisplayColor => displayColor;

        public abstract void ApplyTo(GameObject player);

        /// <summary>
        /// Returns false when the pickup effect should not be applied to the player.
        /// </summary>
        public virtual bool CanApplyTo(GameObject player) => true;
    }
}