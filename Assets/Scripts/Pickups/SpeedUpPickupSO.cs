using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Speed Up", fileName = "Pickup_SpeedUp")]
    public class SpeedUpPickupSO : PickupSO {
        [SerializeField] float speedBonus = 1f;

        public override void ApplyTo(GameObject player) {
            if (player.TryGetComponent<PlayerStats>(out var stats)) {
                stats.MoveSpeed += speedBonus;
            }
        }
    }
}