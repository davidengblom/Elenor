using UnityEngine;

namespace Elenor {
    public class EnemyMelee : MonoBehaviour, IEnemyComponent {
        [SerializeField] EnemySO data;

        float _contactDamage;

        void Awake() {
            Init(data);
        }

        public void Init(EnemySO so) {
            data = so;
            if (so == null || !so.IsMelee) {
                _contactDamage = 0f;
                enabled = false;
                return;
            }
            _contactDamage = EnemyStatApplicator.GetContactDamage(so);
            enabled = true;
        }

        void OnCollisionStay2D(Collision2D collision) {
            if (data == null || !data.IsMelee || _contactDamage <= 0f) return;
            if (!collision.collider.CompareTag("Player")) return;

            if (collision.collider.TryGetComponent<IDamageable>(out var dmg)) {
                dmg.TakeDamage(_contactDamage, default, DamageSource.Enemy);
            }
        }
    }
}