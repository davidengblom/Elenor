using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class WeaponPickup : MonoBehaviour {
        [SerializeField] WeaponSO weapon;
        [SerializeField, Min(0f), Tooltip("Seconds before the pickup can be collected after being dropped.")]
        float armDelaySeconds = 0.2f;

        SpriteRenderer _sprite;
        float _armedAtTime;

        void Awake() {
            _sprite = GetComponent<SpriteRenderer>();
            Rearm();
            ApplyVisuals();
        }

        public void Configure(WeaponSO newWeapon) {
            weapon = newWeapon;
            Rearm();
            ApplyVisuals();
        }

        void Rearm() {
            _armedAtTime = Time.time + armDelaySeconds;
        }

        void ApplyVisuals() {
            if (weapon == null || _sprite == null) return;
            if (weapon.Sprite != null) _sprite.sprite = weapon.Sprite;
            _sprite.color = weapon.DisplayColor;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (weapon == null) return;
            if (Time.time < _armedAtTime) return;
            if (!other.CompareTag("Player")) return;
            if (!other.TryGetComponent<PlayerShooter>(out var shooter)) return;

            if (shooter.SwapWeapon(weapon, other.transform.position)) {
                Destroy(gameObject);
            }
        }
    }
}