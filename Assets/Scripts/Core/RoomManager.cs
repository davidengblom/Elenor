using UnityEngine;

namespace Elenor {
    public class RoomManager : MonoBehaviour {
        public static RoomManager Instance { get; private set; }

        [SerializeField] GameObject roomPrefab;

        RoomController _currentRoom;
        Transform _player;
        Rigidbody2D _playerBody;

        public RoomController CurrentRoom => _currentRoom;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) {
                _player = p.transform;
                _player.TryGetComponent(out _playerBody);
            }
        }

        void OnDestroy() {
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

            PlacePlayerAtSpawn();
        }

        void PlacePlayerAtSpawn() {
            if (_player == null || _currentRoom == null) return;

            SpawnPoint spawn = _currentRoom.GetPlayerSpawns();
            if (spawn == null) {
                Debug.LogWarning($"RoomManager: room '{_currentRoom.name}' has no player spawn marker.", this);
                return;
            }

            _player.position = spawn.transform.position;
            if (_playerBody != null) _playerBody.linearVelocity = Vector2.zero;
        }

        void ClearProjectiles() {
            Projectile[] projectiles = FindObjectsByType<Projectile>();
            for (int i = 0; i < projectiles.Length; i++) {
                if (projectiles[i] != null) Destroy(projectiles[i].gameObject);
            }
        }
    }
}