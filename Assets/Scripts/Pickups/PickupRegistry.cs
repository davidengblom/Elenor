using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    public class PickupRegistry : MonoBehaviour {
        public static PickupRegistry Instance { get; private set; }

        [SerializeField] PickupRegistrySO registry;

        public PickupRegistrySO Registry => registry;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy() {
            if (Instance == this) Instance = null;
        }

        public IEnumerable<PickupSO> GetByRarities(IReadOnlyList<PickupRarity> allowed) {
            if (registry == null) return System.Linq.Enumerable.Empty<PickupSO>();
            return registry.GetByRarities(allowed);
        }
    }
}