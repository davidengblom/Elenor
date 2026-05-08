using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Heart", fileName = "Pickup_Heart")]
    public class HeartPickupSO : PickupSO {
        [SerializeField] float healAmount = 1f;

        public override bool CanApplyTo(GameObject player) {
            if (!player.TryGetComponent<PlayerHealth>(out var hp)) return false;
            return hp.Current < hp.Max;
        }

        public override void ApplyTo(GameObject player) {
            if (player.TryGetComponent<PlayerHealth>(out var hp)) {
                hp.Heal(healAmount);
            }
        }
    }
}