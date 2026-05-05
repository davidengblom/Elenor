using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Damage Up", fileName = "Pickup_DamageUp")]
    public class DamageUpPickupSO : PickupSO {
        [SerializeField] float multiplierBonus = 0.5f;

        public override void ApplyTo(GameObject player) {
            if (player.TryGetComponent<PlayerStats>(out var stats)) {
                stats.DamageMultiplier += multiplierBonus;
            }
        }
    }
}