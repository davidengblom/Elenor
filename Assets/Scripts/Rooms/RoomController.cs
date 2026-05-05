using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elenor {
    public class RoomController : MonoBehaviour {
        public event Action RoomCleared;

        [Header("Spawning")]
        [SerializeField] List<GameObject> initialEnemies = new();
        [SerializeField] GameObject pickupPrefab;
        [SerializeField] List<PickupSO> possiblePickups = new();
        [SerializeField] GameObject doorPrefab;

        public bool IsCleared { get; private set; }

        readonly HashSet<GameObject> _activeEnemies = new();

        public IReadOnlyCollection<GameObject> ActiveEnemies => _activeEnemies;
        public int ActiveEnemyCount => _activeEnemies.Count;

        void Start() {
            SpawnInitialEnemies();
        }

        void SpawnInitialEnemies() {
            var spawns = GetEnemySpawns();
            int placed = 0;

            for (int i = 0; i < initialEnemies.Count && i < spawns.Count; i++) {
                if (initialEnemies[i] == null) continue;
                Instantiate(initialEnemies[i], spawns[i].transform.position, Quaternion.identity, transform);
                placed++;
            }

            if (initialEnemies.Count > spawns.Count) {
                Debug.LogWarning($"{name}: more initialEnemies ({initialEnemies.Count}) than enemy spawns ({spawns.Count}). Extras ignored.", this);
            }

            if (placed == 0) {
                Debug.LogWarning($"{name}: spawned no enemies. Room will never become 'cleared'.", this);
            }
        }

        void SpawnReward() {
            if (pickupPrefab == null) {
                Debug.LogWarning($"{name}: no pickupPrefab assigned. Skipping reward.", this);
                return;
            }
            if (possiblePickups == null || possiblePickups.Count == 0) {
                Debug.LogWarning($"{name}: possiblePickups is empty. Skipping reward.", this);
                return;
            }

            PickupSO so = possiblePickups[UnityEngine.Random.Range(0, possiblePickups.Count)];
            if (so == null) return;

            GameObject go = Instantiate(pickupPrefab, transform.position, Quaternion.identity, transform);
            if (go.TryGetComponent<Pickup>(out var pickup)) {
                pickup.Configure(so);
            }
        }

        void SpawnDoor() {
            if (doorPrefab == null) {
                Debug.LogWarning($"{name}: no doorPrefab assigned. Skipping door.", this);
                return;
            }
            SpawnPoint anchor = GetDoorAnchor();
            if (anchor == null) {
                Debug.LogWarning($"{name}: no door anchor. Skipping door.", this);
                return;
            }
            Instantiate(doorPrefab, anchor.transform.position, Quaternion.identity, transform);
        }

        public void RegisterEnemy(GameObject enemy) {
            if (enemy == null) return;
            _activeEnemies.Add(enemy);
        }

        public void UnregisterEnemy(GameObject enemy) {
            if (enemy == null) return;
            if (!_activeEnemies.Remove(enemy)) return;

            if (!IsCleared && _activeEnemies.Count == 0) {
                IsCleared = true;
                SpawnReward();
                SpawnDoor();
                RoomCleared?.Invoke();
            }
        }

        public List<SpawnPoint> GetSpawns(SpawnPoint.Kind kind) {
            return GetComponentsInChildren<SpawnPoint>(includeInactive: true).Where(sp => sp.PointKind == kind).ToList();
        }

        public List<SpawnPoint> GetEnemySpawns() => GetSpawns(SpawnPoint.Kind.Enemy);
        public SpawnPoint GetPlayerSpawn() => GetSpawns(SpawnPoint.Kind.Player).FirstOrDefault();
        public SpawnPoint GetDoorAnchor() => GetSpawns(SpawnPoint.Kind.Door).FirstOrDefault();

        public string Validate() {
            var sb = new StringBuilder();
            int enemyCount = GetSpawns(SpawnPoint.Kind.Enemy).Count;
            int playerCount = GetSpawns(SpawnPoint.Kind.Player).Count;
            int doorCount = GetSpawns(SpawnPoint.Kind.Door).Count;

            sb.AppendLine($"Enemy spawns: {enemyCount}");
            sb.AppendLine($"Player spawns: {playerCount}");
            sb.AppendLine($"Door spawns: {doorCount}");
            sb.AppendLine();

            if (enemyCount == 0) sb.AppendLine("WARNING: No enemy spawn points. Room will be 'cleared' immediately.");
            if (playerCount == 0) sb.AppendLine("ERROR: No player spawn point.");
            if (playerCount > 1) sb.AppendLine("WARNING: Multiple player spawn points found. Only the first will be used.");
            if (doorCount == 0) sb.AppendLine("ERROR: No door anchor.");
            if (doorCount > 1) sb.AppendLine("WARNING: Multiple door anchors found. Only the first will be used.");

            if (enemyCount > 0 && playerCount == 1 && doorCount == 1) {
                sb.AppendLine("All required markers present. Room is well formed.");
            }

            return sb.ToString();
        }
    }
}