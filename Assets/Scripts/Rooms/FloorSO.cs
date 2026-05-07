using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    /// <summary>
    /// An ordered list of room prefabs for player to progress through.
    /// </summary>
    [CreateAssetMenu(menuName = "Elenor/Floors/Floor", fileName = "Floor_")]
    public class FloorSO : ScriptableObject {
        [SerializeField] string displayName = "Floor";

        [SerializeField, Tooltip("Room prefabs in visit order.")]
        List<GameObject> rooms = new();

        public string DisplayName => displayName;
        public IReadOnlyList<GameObject> Rooms => rooms;
        public int RoomCount => rooms.Count;
    }
}