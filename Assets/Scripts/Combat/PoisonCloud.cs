using UnityEngine;

namespace Elenor {
    public class PoisonCloud : MonoBehaviour {
        float _dps;
        float _expiresAt;
        float _nextTick;
        float _radius;

        public void Init(float dps, float duration, float radius) {
            _dps = dps;
            _expiresAt = Time.time + duration;
            _nextTick = Time.time;
            _radius = radius;
        }

        void Update() {
            if (Time.time >= _expiresAt) {
                Destroy(gameObject);
                return;
            }
            if (Time.time < _nextTick) return;

            _nextTick = Time.time + 1f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _radius);
            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].TryGetComponent<EnemyHealth>(out var health) && health.IsAlive) {
                    health.TakeDamage(_dps);
                }
            }
        }
    }
}