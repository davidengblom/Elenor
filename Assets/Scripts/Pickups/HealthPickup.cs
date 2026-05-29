using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class HealthPickup : MonoBehaviour {
        [SerializeField] HealthPickupSO config;

        SpriteRenderer _sprite;

        void Awake() {
            _sprite = GetComponent<SpriteRenderer>();
            ApplyVisuals();
        }

        public void Configure(HealthPickupSO data) {
            config = data;
            ApplyVisuals();
        }

        void ApplyVisuals() {
            if (config == null || _sprite == null) return;
            if (config.Sprite != null) _sprite.sprite = config.Sprite;
            _sprite.color = config.DisplayColor;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (config == null) return;
            if (!other.CompareTag("Player")) return;
            if (!other.TryGetComponent<PlayerHealth>(out var health)) return;
            if (!health.IsAlive) return;

            health.Heal(config.HealAmount);
            Destroy(gameObject);
        }
    }
}