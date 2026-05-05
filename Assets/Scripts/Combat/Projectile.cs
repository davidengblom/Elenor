using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour {
        Rigidbody2D _rb;
        float _damage;
        float _despawnAt;

        void Awake() {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 velocity, float damage, float lifetime) {
            _damage = damage;
            _rb.linearVelocity = velocity;
            _despawnAt = Time.time + lifetime;
        }

        void Update() {
            if (Time.time >= _despawnAt) Destroy(gameObject);
        }

        void OnCollisionEnter2D(Collision2D collision) {
            if (collision.collider.TryGetComponent<IDamageable>(out var dmg)) {
                dmg.TakeDamage(_damage);
            }

            Destroy(gameObject);
        }
    }
}