using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Collider2D))]
    public abstract class DoorBase : MonoBehaviour {
        [SerializeField] protected Direction direction;
        [SerializeField, Min(0.05f), Tooltip("Time after Configure before this door accepts the player. Prevents unwanted teleportation.")]
        protected float armDelay = 0.25f;

        float _armedAt;

        public Direction DoorDirection => direction;
        
        public void Configure(Direction dir) {
            direction = dir;
            _armedAt = Time.time + armDelay;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (Time.time < _armedAt) return;
            if (!other.CompareTag("Player")) return;
            OnPlayerEntered();
        }

        protected abstract void OnPlayerEntered();
    }
}