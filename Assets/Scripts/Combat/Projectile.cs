using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour {
        Rigidbody2D _rb;
        float _damage;
        float _knockback;
        Vector2 _direction;
        float _despawnAt;
        SpriteRenderer _sprite;

        void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void Launch(Vector2 velocity, float damage, float lifetime, float knockback = 0f) {
            _damage = damage;
            _knockback = knockback;
            _rb.linearVelocity = velocity;
            _direction = velocity.sqrMagnitude > 0f ? velocity.normalized : Vector2.zero;
            _despawnAt = Time.time + lifetime;
        }

        void Update() {
            if (Time.time >= _despawnAt) Destroy(gameObject);
        }

        void OnCollisionEnter2D(Collision2D collision) {
            if (collision.collider.TryGetComponent<IDamageable>(out var dmg)) {
                Vector2 impulse = _direction * _knockback;
                dmg.TakeDamage(_damage, impulse);
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Overrides projectile appearence.
        /// </summary>
        public void ApplyVisuals(float scaleMultiplier, Color color, Sprite spriteOverride = null) {
            transform.localScale *= scaleMultiplier;

            if (_sprite != null) {
                _sprite.color = color;
                if (spriteOverride != null) {
                    _sprite.sprite = spriteOverride;
                }
            }
        }
    }
}