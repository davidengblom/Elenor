using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Collider2D))]
    public class Door : MonoBehaviour {
        [SerializeField] Direction direction;

        public Direction DoorDirection => direction;

        public void Configure(Direction dir) {
            direction = dir;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player")) return;
            if (RoomManager.Instance != null) {
                RoomManager.Instance.GoToNeighborInDirection(direction);
            }
        }
    }
}