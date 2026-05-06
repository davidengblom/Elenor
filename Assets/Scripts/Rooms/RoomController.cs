using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elenor {
    public class RoomController : MonoBehaviour {
        public event Action RoomCleared;

        [Header("Spawning")]
        [SerializeField] RoomContentsSO contents;
        [SerializeField] GameObject pickupPrefab;
        [SerializeField] GameObject doorPrefab;

        public bool IsCleared { get; private set; }

        readonly HashSet<GameObject> _activeEnemies = new();

        public IReadOnlyCollection<GameObject> ActiveEnemies => _activeEnemies;
        public int ActiveEnemyCount => _activeEnemies.Count;

        void Start() {
            SpawnInitialEnemies();
        }

        void SpawnInitialEnemies() {
            if (contents == null) {
                Debug.LogWarning($"{name}: no RoomContentsSO assigned. Room will be 'cleared' immediately.", this);
                return;
            }

            var enemies = contents.InitialEnemies;
            var spawns = GetEnemySpawns();
            int placed = 0;

            for (int i = 0; i < enemies.Count && i < spawns.Count; i++) {
                if (enemies[i] == null) continue;
                Instantiate(enemies[i], spawns[i].transform.position, Quaternion.identity, transform);
                placed++;
            }

            if (enemies.Count > spawns.Count) {
                Debug.LogWarning($"{name}: more initialEnemies ({enemies.Count}) than enemy spawns ({spawns.Count}). Extras ignored.", this);
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
            if (contents == null || contents.PossiblePickups.Count == 0) {
                Debug.LogWarning($"{name}: contents has no possible pickups. Skipping reward.", this);
                return;
            }

            var pool = contents.PossiblePickups;
            PickupSO so = pool[UnityEngine.Random.Range(0, pool.Count)];
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

            // Skip room-cleared logic when the scene is unloading
            // (e.g player death triggered a reload)
            if (!gameObject.scene.isLoaded) return; 

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

            if (contents == null) {
                sb.AppendLine("ERROR: No RoomContentsSO assigned.");
            } else {
                sb.AppendLine($"Initial enemies in contents: {contents.InitialEnemies.Count}");
                sb.AppendLine($"Possible pickups in contents: {contents.PossiblePickups.Count}");
                if (contents.InitialEnemies.Count > enemyCount) {
                    sb.AppendLine($"WARNING: More initialEnemies ({contents.InitialEnemies.Count}) than enemy spawns ({enemyCount}). Extras will be ignored.");
                }
                if (contents.PossiblePickups.Count == 0) {
                    sb.AppendLine("WARNING: contents has no possible pickups. No reward will spawn.");
                }
            }
            sb.AppendLine();

            if (enemyCount == 0) sb.AppendLine("WARNING: No enemy spawn points. Room will be 'cleared' immediately.");
            if (playerCount == 0) sb.AppendLine("ERROR: No player spawn point.");
            if (playerCount > 1) sb.AppendLine("WARNING: Multiple player spawn points found. Only the first will be used.");
            if (doorCount == 0) sb.AppendLine("ERROR: No door anchor.");
            if (doorCount > 1) sb.AppendLine("WARNING: Multiple door anchors found. Only the first will be used.");

            if (enemyCount > 0 && playerCount == 1 && doorCount == 1 && contents != null) {
                sb.AppendLine("All required markers and contents present. Room is well formed.");
            }

            return sb.ToString();
        }
    }
}