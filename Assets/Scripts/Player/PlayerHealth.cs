using System;
using UnityEngine;

namespace Elenor {
    public class PlayerHealth : MonoBehaviour, IDamageable {
        [SerializeField] float maxHealth = 6f;
        [SerializeField, Tooltip("Seconds of invulnerability after taking damage.")]
        float hitInvulnerability = 0.5f;

        float _current;
        float _invulnUntil;

        public float Max => maxHealth;
        public float Current => _current;
        public bool IsAlive => _current > 0f;
        public bool IsInvulnerable => Time.time < _invulnUntil;

        public event Action<float, float> HealthChanged;
        public event Action Died;
        public event Action<float> Damaged;

        public float MaxHealth {
            get => maxHealth;
            set {
                maxHealth = Mathf.Max(1f, value);
                _current = Mathf.Min(_current, maxHealth);
                HealthChanged?.Invoke(_current, maxHealth);
            }
        }

        void Awake() {
            _current = maxHealth;
        }

        void Start() {
            HealthChanged?.Invoke(_current, maxHealth);
        }

        public void TakeDamage(float amount, Vector2 hitImpulse = default, DamageSource source = DamageSource.Unspecified) {
            if (!IsAlive || IsInvulnerable || amount <= 0f) return;

            _current = Mathf.Max(0f, _current - amount);
            _invulnUntil = Time.time + hitInvulnerability;
            HealthChanged?.Invoke(_current, maxHealth);
            Damaged?.Invoke(amount);

            if (_current <= 0f) Die();
        }

        public void Heal(float amount) {
            if (!IsAlive || amount <= 0f) return;

            _current = Mathf.Min(maxHealth, _current + amount);
            HealthChanged?.Invoke(_current, maxHealth);
        }

        public void GrantInvulnerability(float duration) {
            if (duration <= 0f) return;
            _invulnUntil = Mathf.Max(_invulnUntil, Time.time + duration);
        }

        void Die() {
            Died?.Invoke();
            gameObject.SetActive(false);
        }
#if UNITY_EDITOR
        [ContextMenu("DEBUG: Take 1 Damage")]
        void DebugTakeDamage() => TakeDamage(1f);

        [ContextMenu("DEBUG: Heal to Full")]
        void DebugHealToFull() => Heal(maxHealth);
#endif
    }
}