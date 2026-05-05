using System;
using UnityEngine;

namespace Elenor {
    public class RoomManager : MonoBehaviour {
        public static RoomManager Instance { get; private set; }

        [SerializeField] GameObject roomPrefab;

        RoomController _currentRoom;

        public RoomController CurrentRoom => _currentRoom;

        public event Action<int> RoomsClearedChanged;
        public int RoomsCleared { get; private set; }

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy() {
            if (_currentRoom != null) {
                _currentRoom.RoomCleared -= OnCurrentRoomCleared;
            }
            if (Instance == this) Instance = null;
        }

        void Start() {
            SpawnRoom();
        }

        public void GoToNextRoom() {
            SpawnRoom();
        }

        void SpawnRoom() {
            ClearProjectiles();

            if (_currentRoom != null) {
                _currentRoom.RoomCleared -= OnCurrentRoomCleared;
                Destroy(_currentRoom.gameObject);
            }

            if (roomPrefab == null) {
                Debug.LogError("RoomManager: roomPrefab not assigned.", this);
                return;
            }

            GameObject go = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
            _currentRoom = go.GetComponent<RoomController>();
            if (_currentRoom == null) {
                Debug.LogError($"RoomManager: roomPrefab {roomPrefab.name} does not have a RoomController component.", this);
                return;
            }

            _currentRoom.RoomCleared += OnCurrentRoomCleared;

            PlacePlayerAtSpawn();
        }

        void PlacePlayerAtSpawn() {
            Transform player = PlayerLocator.Player;
            if (player == null || _currentRoom == null) return;

            SpawnPoint spawn = _currentRoom.GetPlayerSpawn();
            if (spawn == null) {
                Debug.LogWarning($"RoomManager: room '{_currentRoom.name}' has no player spawn marker.", this);
                return;
            }

            player.position = spawn.transform.position;
            if (player.TryGetComponent(out Rigidbody2D rb)) rb.linearVelocity = Vector2.zero;
        }

        void ClearProjectiles() {
            Projectile[] projectiles = FindObjectsByType<Projectile>();
            for (int i = 0; i < projectiles.Length; i++) {
                if (projectiles[i] != null) Destroy(projectiles[i].gameObject);
            }
        }

        void OnCurrentRoomCleared() {
            RoomsCleared++;
            RoomsClearedChanged?.Invoke(RoomsCleared);
        }
    }
}