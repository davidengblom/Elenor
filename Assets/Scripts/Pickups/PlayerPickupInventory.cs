using UnityEngine;
using System;
using System.Collections.Generic;

namespace Elenor {
    /// <summary>
    /// Tracks which pickups the player has acquired and at what level.
    /// </summary>
    public class PlayerPickupInventory : MonoBehaviour {
        readonly Dictionary<PickupSO, int> _levels = new();
        readonly Dictionary<PickupSO, IBehaviorModifier> _modifiers = new();

        public event Action<PickupSO, int> PickupAcquired;
        public event Action<PickupSO, int> PickupLeveledUp;

        public IReadOnlyDictionary<PickupSO, int> Levels => _levels;

        public int GetLevel(PickupSO pickup) => _levels.TryGetValue(pickup, out int level) ? level : 0;

        public bool IsMaxed(PickupSO pickup) => GetLevel(pickup) >= PickupSO.MaxLevel;

        /// <summary>
        /// Acquire a pickup. Returns false if the pickup is already at max level.
        /// </summary>
        public bool TryAcquire(PickupSO pickup) {
            if (pickup == null) return false;
            if (pickup.Category != PickupCategory.Modifier) return false;
            if (pickup is not ModifierSO modifierPickup) return false;

            int currentLevel = GetLevel(pickup);
            if (currentLevel >= PickupSO.MaxLevel) return false;

            int newLevel = currentLevel + 1;
            _levels[pickup] = newLevel;

            if (currentLevel == 0) {
                IBehaviorModifier modifier = (IBehaviorModifier)gameObject.AddComponent(modifierPickup.ModifierComponentType);
                modifier.Level = newLevel;
                modifierPickup.ConfigureModifier(modifier, newLevel);
                _modifiers[pickup] = modifier;
                PickupAcquired?.Invoke(pickup, newLevel);
            } else {
                if (_modifiers.TryGetValue(pickup, out var existing)) {
                    existing.Level = newLevel;
                    modifierPickup.ConfigureModifier(existing, newLevel);
                }
                PickupLeveledUp?.Invoke(pickup, newLevel);
            }

            return true;
        }
#if UNITY_EDITOR
        [ContextMenu("DEBUG: Acquire random from registry")]
        void DebugAcquireRandom() {
            if (PickupRegistry.Instance == null || PickupRegistry.Instance.Registry == null) {
                Debug.Log("PickupRegistry not in scene or no SO assigned.");
                return;
            }
            var all = PickupRegistry.Instance.Registry.AllPickups;
            if (all.Count == 0) {
                Debug.Log("Registry is empty.");
                return;
            }
            PickupSO pickup = all[UnityEngine.Random.Range(0, all.Count)];
            TryAcquire(pickup);
        }
#endif
    }
}