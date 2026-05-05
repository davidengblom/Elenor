using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Heart", fileName = "Pickup_Heart")]
    public class HeartPickupSO : PickupSO {
        [SerializeField] float healAmount = 1f;

        public override void ApplyTo(GameObject player) {
            if (player.TryGetComponent<PlayerHealth>(out var hp)) {
                hp.Heal(healAmount);
            }
        }
    }
}