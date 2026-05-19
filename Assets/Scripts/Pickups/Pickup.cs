using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Pickup : MonoBehaviour {
        PickupSO _data;
        SpriteRenderer _sprite;

        void Awake() {
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void Configure(PickupSO data) {
            _data = data;
            ApplyVisuals();
        }

        void ApplyVisuals() {
            if (_data == null || _sprite == null) return;
            if (_data.Sprite != null) _sprite.sprite = _data.Sprite;
            _sprite.color = _data.DisplayColor;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (_data == null) return;
            if (!other.CompareTag("Player")) return;

            if (other.TryGetComponent<PlayerPickupInventory>(out var inventory)) {
                if (inventory.TryAcquire(_data)) {
                    if (RoomManager.Instance != null) {
                        RoomManager.Instance.NotifyPickupCollected();
                    }
                    Destroy(gameObject);
                }
            }
            // If TryAcquire returns false, the pickup is maxed
        }
    }
}