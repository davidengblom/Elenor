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
        bool _isPedestal;

        void Awake() {
            _sprite = GetComponent<SpriteRenderer>();
            Rearm();
            ApplyVisuals();
        }
        
        /// <param name="instant">Pass true for pedestal spawns. Skips arm delay.</param>
        public void Configure(WeaponSO newWeapon, bool instant = false, bool pedestal = false) {
            weapon = newWeapon;
            _isPedestal = pedestal;
            if (instant || pedestal) {
                _armedAtTime = 0f;
            } else {
                Rearm();
            }
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

            Vector3 pos = transform.position;
            WeaponSO wep = weapon;
            bool wasPedestal = _isPedestal;

            bool swapped = shooter.SwapWeapon(wep, pos, transform.parent);
            if (swapped) {
                if (RoomManager.Instance != null) {
                    if (wasPedestal) RoomManager.Instance.NotifyPickupCollected();
                    else RoomManager.Instance.NotifyDroppedWeaponCollected(wep, pos);
                }
                Destroy(gameObject);
            }
        }
    }
}