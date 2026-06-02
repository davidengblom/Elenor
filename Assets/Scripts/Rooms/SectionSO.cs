using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Sections/Section", fileName = "Section_")]
    public class SectionSO : ScriptableObject {
        [SerializeField] string displayName = "Section";
        [SerializeField, Tooltip("Floor gen configs in order. Last floor's exit ends the section.")]
        List<FloorGenConfigSO> floorConfigs = new();

        public string DisplayName => displayName;
        public IReadOnlyList<FloorGenConfigSO> FloorConfigs => floorConfigs;
        public int FloorCount => floorConfigs.Count;

        public FloorGenConfigSO GetFloorConfig(int index) {
            if (index < 0 || index >= floorConfigs.Count) return null;
            return floorConfigs[index];
        }
    }
}