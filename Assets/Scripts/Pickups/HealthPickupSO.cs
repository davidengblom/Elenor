using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Health Pickup Config", fileName = "HealthPickup_")]
    public class HealthPickupSO : ScriptableObject {
        [Header("Visuals")]
        [SerializeField] Sprite sprite;
        [SerializeField] Color displayColor = Color.white;

        [Header("Heal")]
        [Tooltip("HP restored on pickup. Clamps to max hp.")]
        [SerializeField, Min(0.01f)] float healAmount = 2f;

        public Sprite Sprite => sprite;
        public Color DisplayColor => displayColor;
        public float HealAmount => healAmount;
    }
}