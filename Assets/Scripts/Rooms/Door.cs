using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Collider2D))]
    public class Door : MonoBehaviour {
        bool _used;

        void OnTriggerEnter2D(Collider2D other) {
            if (_used) return;
            if (!other.CompareTag("Player")) return;

            _used = true;

            if (RoomManager.Instance != null) {
                RoomManager.Instance.GoToNextRoom();
            } else {
                Debug.LogWarning("Door triggered but no RoomManager.Instance found.", this);
            }
        }
    }
}