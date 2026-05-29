using UnityEngine;

namespace Elenor {
    public class EnemyMelee : MonoBehaviour {
        [SerializeField] EnemySO data;

        void Awake() {
            Init(data);
        }

        public void Init(EnemySO so) {
            data = so;
            if (so == null || !so.IsMelee) {
                enabled = false;
                return;
            }
            enabled = true;
        }

        void OnCollisionStay2D(Collision2D collision) {
            if (data == null || !data.IsMelee) return;
            if (!collision.collider.CompareTag("Player")) return;

            if (collision.collider.TryGetComponent<IDamageable>(out var dmg)) {
                dmg.TakeDamage(data.ContactDamage, default, DamageSource.Enemy);
            }
        }
    }
}