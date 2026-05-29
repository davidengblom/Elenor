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
        ProjectileConfigSnapshot _snapshot;
        int _enemiesPierced;
        Collider2D _collider;
        float _speed;

        void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
        }

        public void Configure(ProjectileConfigSnapshot snapshot) {
            _snapshot = snapshot;

            if (_sprite != null && snapshot.Sprite != null) {
                _sprite.sprite = snapshot.Sprite;
                _sprite.color = snapshot.Color;
            }

            transform.localScale = new Vector3(snapshot.Scale, snapshot.Scale, 1f);
        }

        public void Launch(Vector2 velocity, float damage, float lifetime, float knockback = 0f) {
            _damage = damage;
            _knockback = knockback;
            _speed = velocity.magnitude;
            _rb.linearVelocity = velocity;
            _direction = velocity.sqrMagnitude > 0f ? velocity.normalized : Vector2.zero;
            _despawnAt = Time.time + lifetime;
        }

        public void Configure(ProjectileConfigSO config) {
            if (config == null) return;
            Configure(config.ToSnapshot());
            _enemiesPierced = 0;
        }

        void Update() {
            if (Time.time >= _despawnAt) Destroy(gameObject);
        }

        void OnCollisionEnter2D(Collision2D collision) {
            if (collision.collider.TryGetComponent<IDamageable>(out var dmg)) {
                Vector2 impulse = _direction * _knockback;
                dmg.TakeDamage(_damage, impulse, DamageSource.PlayerProjectile);

                if (_snapshot.ApplyPoison && collision.collider.TryGetComponent<EnemyHealth>(out var health)) {
                    health.ApplyPoison(_snapshot.PoisonDps, _snapshot.PoisonDuration);
                }

                if (_snapshot.Pierce && _enemiesPierced < _snapshot.MaxPierceCount) {
                    _enemiesPierced++;
                    Physics2D.IgnoreCollision(_collider, collision.collider);
                    _rb.linearVelocity = _direction * _speed;
                    return;
                }
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