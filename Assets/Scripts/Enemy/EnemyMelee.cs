using UnityEngine;

namespace Elenor {
    public class EnemyMelee : MonoBehaviour {
        [SerializeField] EnemySO data;

        void OnTriggerStay2D(Collider2D other) {
            if (data == null || data.ContactDamage <= 0f) return;
            if (!other.CompareTag("Player")) return;

            if (other.TryGetComponent<IDamageable>(out var dmg)) {
                dmg.TakeDamage(data.ContactDamage);
            }
        }
    }
}