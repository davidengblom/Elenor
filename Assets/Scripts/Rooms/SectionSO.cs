using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Sections/Section", fileName = "Section_")]
    public class SectionSO : ScriptableObject {
        [SerializeField] string displayName = "Section";
        [SerializeField, Tooltip("Floors in order. The last floor's exit ends the section.")]
        List<FloorSO> floors = new();

        public string DisplayName => displayName;
        public IReadOnlyList<FloorSO> Floors => floors;
        public int FloorCount => floors.Count;

        public FloorSO GetFloor(int index) {
            if (index < 0 || index >= floors.Count) return null;
            return floors[index];
        }
    }
}