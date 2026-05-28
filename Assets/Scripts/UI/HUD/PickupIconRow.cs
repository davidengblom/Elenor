using UnityEngine;
using System.Collections.Generic;
using Elenor;

namespace Elenor.UI.HUD {
    public class PickupIconRow : PickupInventoryListener {
        [SerializeField] PickupIcon iconPrefab;
        [SerializeField] Transform container;

        readonly Dictionary<PickupSO, PickupIcon> _icons = new();

        protected override void OnInventoryReady() {
            foreach (var kvp in Inventory.Levels) {
                SpawnIcon(kvp.Key, kvp.Value);
            }
        }

        protected override void OnPickupAcquired(PickupSO pickup, int level) {
            SpawnIcon(pickup, level);
        }

        protected override void OnPickupLeveledUp(PickupSO pickup, int level) {
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