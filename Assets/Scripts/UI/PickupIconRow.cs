using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    public class PickupIconRow : MonoBehaviour {
        [SerializeField] PickupIcon iconPrefab;
        [SerializeField] Transform container;

        readonly Dictionary<PickupSO, PickupIcon> _icons = new();
        PlayerPickupInventory _inventory;

        void Start() {
            Transform player = PlayerLocator.Player;
            if (player == null) return;
            _inventory = player.GetComponent<PlayerPickupInventory>();
            if (_inventory == null) return;

            _inventory.PickupAcquired += OnPickupAcquired;
            _inventory.PickupLeveledUp += OnPickupLeveledUp;

            foreach (var kvp in _inventory.Levels) {
                SpawnIcon(kvp.Key, kvp.Value);
            }
        }

        void OnDestroy() {
            if (_inventory == null) return;
            _inventory.PickupAcquired -= OnPickupAcquired;
            _inventory.PickupLeveledUp -= OnPickupLeveledUp;
        }

        void OnPickupAcquired(PickupSO pickup, int level) {
            SpawnIcon(pickup, level);
        }

        void OnPickupLeveledUp(PickupSO pickup, int level) {
            if (_icons.TryGetValue(pickup, out var icon)) {
                icon.UpdateLevel(level);
            }
        }

        void SpawnIcon(PickupSO pickup, int level) {
            if (iconPrefab == null || container == null) return;
            if (_icons.ContainsKey(pickup)) return;

            PickupIcon icon = Instantiate(iconPrefab, container);
            icon.Bind(pickup, level);
            _icons[pickup] = icon;
        }
    }
}