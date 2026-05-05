using UnityEngine;

namespace Elenor {
    public abstract class PickupSO : ScriptableObject {
        [SerializeField] string displayName = "Pickup";
        [SerializeField] Color displayColor = Color.white;

        public string DisplayName => displayName;
        public Color DisplayColor => displayColor;

        public abstract void ApplyTo(GameObject player);
    }
}