using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Pickup : MonoBehaviour {
        [SerializeField] PickupSO data;

        SpriteRenderer _renderer;

        public PickupSO Data => data;

        public void Configure(PickupSO so) {
            data = so;
            ApplyVisuals();
        }

        void Awake() {
            _renderer = GetComponent<SpriteRenderer>();
        }

        void Start() {
            ApplyVisuals();
        }

        void ApplyVisuals() {
            if (data == null) return;
            _renderer.color = data.DisplayColor;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (data == null) return;
            if (!other.CompareTag("Player")) return;

            data.ApplyTo(other.gameObject);
            Destroy(gameObject);
        }
    }
}