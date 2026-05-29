using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elenor {
    public class RoomManager : MonoBehaviour {
        public static RoomManager Instance { get; private set; }

        [SerializeField, Tooltip("Distance the player is pushed inward when spawning at a door anchor.")]
        float doorEntryPushIn = 2.5f;
        [SerializeField, Tooltip("Prefab used to re-spawn player-dropped weapons on room re-entry.")]
        WeaponPickup weaponPickupPrefab;

        FloorSO _floor;
        Vector2Int _currentGridPos;
        Direction? _enteredFrom;
        readonly HashSet<Vector2Int> _clearedRooms = new();
        public IReadOnlyCollection<Vector2Int> ClearedRooms => _clearedRooms;

        RoomController _currentRoom;
        public RoomController CurrentRoom => _currentRoom;

        public event Action<int> RoomsClearedChanged;
        public event Action<Vector2Int> RoomChanged;
        public event Action<FloorSO> FloorChanged;
        public int RoomsCleared { get; private set; }
        public Vector2Int CurrentGridPos => _currentGridPos;
        public FloorSO Floor => _floor;
        public bool IsCurrentRoomExit => _floor != null && _currentGridPos == _floor.ExitPosition;
        public bool IsRoomTransitioning { get; private set; }

        readonly Dictionary<Vector2Int, PickupSO> _pendingRewards = new();

        struct DroppedWeaponEntry {
            public WeaponSO Weapon;
            public Vector3 Position;
        }
        readonly Dictionary<Vector2Int, List<DroppedWeaponEntry>> _droppedWeapons = new();

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy() {
            if (_currentRoom != null)
                _currentRoom.RoomCleared -= OnCurrentRoomCleared;
            if (Instance == this) Instance = null;
        }

        public void LoadFloor(FloorSO newFloor) {
            if (newFloor == null) {
                Debug.LogError("RoomManager: LoadFloor called with null floor.", this);
                return;
            }

            _floor = newFloor;
            _currentGridPos = _floor.StartPosition;
            _enteredFrom = null;
            _clearedRooms.Clear();
            _pendingRewards.Clear();
            _droppedWeapons.Clear();

            FloorChanged?.Invoke(_floor);
            SpawnRoom();
        }

        public void GoToNeighborInDirection(Direction dir) {
            if (_floor == null) return;
            Vector2Int target = _currentGridPos + dir.Offset();
            if (!_floor.HasRoomAt(target)) {
                Debug.LogWarning($"RoomManager: no room at {target} (going {dir} from {_currentGridPos}).", this);
                return;
            }
            _currentGridPos = target;
            _enteredFrom = dir.Opposite();
            SpawnRoom();
        }

        public bool HasNeighbor(Direction dir) => _floor != null && _floor.HasRoomAt(_currentGridPos + dir.Offset());

        void SpawnRoom() {
            IsRoomTransitioning = true;
            try {
                ClearProjectiles();

            if (_currentRoom != null) {
                _currentRoom.RoomCleared -= OnCurrentRoomCleared;
                Destroy(_currentRoom.gameObject);
            }

            FloorRoomEntry entry = _floor.FindRoomAt(_currentGridPos);
            if (entry == null || entry.roomPrefab == null) {
                Debug.LogError($"RoomManager: no valid room at {_currentGridPos}.", this);
                return;
            }

            GameObject go = Instantiate(entry.roomPrefab, Vector3.zero, Quaternion.identity);
            _currentRoom = go.GetComponent<RoomController>();
            if (_currentRoom == null) {
                Debug.LogError($"RoomManager: prefab {entry.roomPrefab.name} has no RoomController.", this);
                return;
            }

            _currentRoom.RoomCleared += OnCurrentRoomCleared;

            bool alreadyCleared = _clearedRooms.Contains(_currentGridPos);
            PickupSO pendingReward = alreadyCleared ? PeekPendingReward(_currentGridPos) : null;
            _currentRoom.Initialize(new RoomController.RoomState {
                IsCleared = alreadyCleared,
                PendingReward = pendingReward,
                RoomType = entry.roomType,
            }, entry.contentsOverride);

            SpawnTrackedDroppedWeapons();

            RoomChanged?.Invoke(_currentGridPos);
            PlacePlayerAtSpawn();
            } finally {
                IsRoomTransitioning = false;
            }
        }

        void PlacePlayerAtSpawn() {
            Transform player = PlayerLocator.Player;
            if (player == null || _currentRoom == null) return;

            Vector3 targetPos;
            if (_enteredFrom.HasValue) {
                SpawnPoint anchor = _currentRoom.GetDoorAnchor(_enteredFrom.Value);
                if (anchor != null) {
                    Vector2 pushIn = -(Vector2)_enteredFrom.Value.Offset() * doorEntryPushIn;
                    targetPos = anchor.transform.position + (Vector3)pushIn;
                } else {
                    Debug.LogWarning($"RoomManager: room at {_currentGridPos} has no door anchor for direction {_enteredFrom.Value}.", this);
                    SpawnPoint fallback = _currentRoom.GetPlayerSpawn();
                    targetPos = fallback != null ? fallback.transform.position : Vector3.zero;
                }
            } else {
                SpawnPoint spawn = _currentRoom.GetPlayerSpawn();
                if (spawn == null) {
                    Debug.LogWarning($"RoomManager: room at {_currentGridPos} has no player spawn marker.", this);
                    return;
                }
                targetPos = spawn.transform.position;
            }

            player.position = targetPos;
            if (player.TryGetComponent(out Rigidbody2D rb)) rb.linearVelocity = Vector2.zero;
        }

        void ClearProjectiles() {
            Projectile[] projectiles = FindObjectsByType<Projectile>();
            for (int i = 0; i < projectiles.Length; i++) {
                if (projectiles[i] != null) Destroy(projectiles[i].gameObject);
            }
        }

        void SpawnTrackedDroppedWeapons() {
            if (_currentRoom == null) return;
            if (!_droppedWeapons.TryGetValue(_currentGridPos, out var list) || list.Count == 0) return;
            if (weaponPickupPrefab == null) {
                Debug.LogWarning("RoomManager: weaponPickupPrefab not assigned. Cannot re-spawn dropped weapons.", this);
                return;
            }
            for (int i = 0; i < list.Count; i++) {
                if (list[i].Weapon == null) continue;
                WeaponPickup wp = Instantiate(weaponPickupPrefab, list[i].Position, Quaternion.identity, _currentRoom.transform);
                wp.Configure(list[i].Weapon, instant: true, pedestal: false);
            }
        }

        void OnCurrentRoomCleared() {
            _clearedRooms.Add(_currentGridPos);
            RoomsCleared++;
            RoomsClearedChanged?.Invoke(RoomsCleared);
        }

        public void RegisterPendingReward(PickupSO reward) {
            if (reward == null) return;
            _pendingRewards[_currentGridPos] = reward;
        }

        public PickupSO PeekPendingReward(Vector2Int pos) {
            _pendingRewards.TryGetValue(pos, out var so);
            return so;
        }

        public void NotifyPickupCollected() {
            _pendingRewards.Remove(_currentGridPos);
        }

        public void RegisterDroppedWeapon(WeaponSO weapon, Vector3 worldPosition) {
            if (weapon == null) return;
            if (!_droppedWeapons.TryGetValue(_currentGridPos, out var list)) {
                list = new List<DroppedWeaponEntry>();
                _droppedWeapons[_currentGridPos] = list;
            }
            list.Add(new DroppedWeaponEntry { Weapon = weapon, Position = worldPosition });
        }

        public void NotifyDroppedWeaponCollected(WeaponSO weapon, Vector3 position) {
            if (!_droppedWeapons.TryGetValue(_currentGridPos, out var list)) return;
            for (int i = list.Count - 1; i >= 0; i--) {
                if (list[i].Weapon == weapon &&
                    (list[i].Position - position).sqrMagnitude < 0.01f) {
                    list.RemoveAt(i);
                    return;
                }
            }
        }
    }
}