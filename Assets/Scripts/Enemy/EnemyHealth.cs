using UnityEngine;

namespace Elenor {
    public class EnemyHealth : MonoBehaviour, IDamageable {
        [SerializeField] EnemySO data;
        [SerializeField, Tooltip("Fallback if no EnemySO is assigned.")]
        float fallbackMaxHealth = 3f;

        float _current;
        RoomController _room;

        public float Current => _current;
        public bool IsAlive => _current > 0f;

        void Awake() {
            _current = data != null ? data.MaxHealth : fallbackMaxHealth;
        }

        void OnEnable() {
            _room = GetComponentInParent<RoomController>();
            if (_room != null) _room.RegisterEnemy(gameObject);
        }

        void OnDisable() {
            if (_room != null) _room.UnregisterEnemy(gameObject);
        }

        public void TakeDamage(float amount) {
            if (!IsAlive || amount <= 0f) return;
            _current = Mathf.Max(0f, _current - amount);
            if (_current <= 0f) Die();
        }

        void Die() {
            // Destroy is deferred to end of frame. OnDisable will then fire and unregister enemy.
            Destroy(gameObject);
        }
    }
}