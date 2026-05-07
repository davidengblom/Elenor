using System;
using UnityEngine;

namespace Elenor {
    public class RoomManager : MonoBehaviour {
        public static RoomManager Instance { get; private set; }

        [SerializeField] FloorSO floor;

        int _currentRoomIndex;

        RoomController _currentRoom;

        public RoomController CurrentRoom => _currentRoom;

        public event Action<int> RoomsClearedChanged;
        public event Action<int, int> RoomChanged; // (currentIndex, totalRooms)
        public int RoomsCleared { get; private set; }

        public int CurrentRoomIndex => _currentRoomIndex;
        public int FloorRoomCount => floor != null ? floor.RoomCount : 0;

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
            if (floor == null || floor.RoomCount == 0) return;

            _currentRoomIndex++;
            if (_currentRoomIndex >= floor.RoomCount) {
                _currentRoomIndex = 0;
                Debug.Log($"Floor {floor.DisplayName} cleared. Looping back to start.");
            }

            SpawnRoom();
        }

        void SpawnRoom() {
            ClearProjectiles();

            if (_currentRoom != null) {
                _currentRoom.RoomCleared -= OnCurrentRoomCleared;
                Destroy(_currentRoom.gameObject);
            }

            if (floor == null || floor.RoomCount == 0) {
                Debug.LogError("RoomManager: no Floor assigned, or floor has no rooms.", this);
                return;
            }

            GameObject prefab = floor.Rooms[_currentRoomIndex];
            if (prefab == null) {
                Debug.LogError($"RoomManager: floor.Rooms[{_currentRoomIndex}] is null. Check the floor asset.", this);
                return;
            }

            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            _currentRoom = go.GetComponent<RoomController>();
            if (_currentRoom == null) {
                Debug.LogError($"RoomManager: roomPrefab {prefab.name} does not have a RoomController component.", this);
                return;
            }

            _currentRoom.RoomCleared += OnCurrentRoomCleared;

            RoomChanged?.Invoke(_currentRoomIndex, floor.RoomCount);

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