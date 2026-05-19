using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elenor {
    [Serializable]
    public class FloorRoomEntry {
        public GameObject roomPrefab;
        public Vector2Int gridPosition;
        [Tooltip("Optional. Overrides the prefab's default RoomContentsSO.")]
        public RoomContentsSO contentsOverride;
    }

    [CreateAssetMenu(menuName = "Elenor/Floors/Floor", fileName = "Floor_")]
    public class FloorSO : ScriptableObject {
        [SerializeField] string displayName = "Floor";
        [SerializeField, Tooltip("Player starts here. Must match a roomPrefab entry's gridPosition.")]
        Vector2Int startPosition;
        [SerializeField, Tooltip("The exit-room grid position. Must match a roomPrefab entry's gridPosition.")]
        Vector2Int exitPosition;
        [SerializeField] List<FloorRoomEntry> rooms = new();
        [SerializeField, Tooltip("Pickup rarities eligible to drop on this floor.")]
        List<PickupRarity> allowedRarities = new() { PickupRarity.Common };

        public string DisplayName => displayName;
        public Vector2Int StartPosition => startPosition;
        public Vector2Int ExitPosition => exitPosition;
        public IReadOnlyList<FloorRoomEntry> Rooms => rooms;
        public int RoomCount => rooms.Count;
        public IReadOnlyList<PickupRarity> AllowedRarities => allowedRarities;

        public FloorRoomEntry FindRoomAt(Vector2Int pos) {
            for (int i = 0; i < rooms.Count; i++) {
                if (rooms[i].gridPosition == pos) return rooms[i];
            }
            return null;
        }

        public bool HasRoomAt(Vector2Int pos) => FindRoomAt(pos) != null;
    }
}