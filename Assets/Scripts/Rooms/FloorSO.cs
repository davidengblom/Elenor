using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elenor {
    [Serializable]
    public class FloorRoomEntry {
        public GameObject roomPrefab;
        public Vector2Int gridPosition;
    }

    [CreateAssetMenu(menuName = "Elenor/Floors/Floor", fileName = "Floor_")]
    public class FloorSO : ScriptableObject {
        [SerializeField] string displayName = "Floor";
        [SerializeField, Tooltip("Player starts here. Must match a roomPrefab entry's gridPosition.")]
        Vector2Int startPosition;
        [SerializeField] List<FloorRoomEntry> rooms = new();

        public string DisplayName => displayName;
        public Vector2Int StartPosition => startPosition;
        public IReadOnlyList<FloorRoomEntry> Rooms => rooms;
        public int RoomCount => rooms.Count;

        public FloorRoomEntry FindRoomAt(Vector2Int pos) {
            for (int i = 0; i < rooms.Count; i++) {
                if (rooms[i].gridPosition == pos) return rooms[i];
            }
            return null;
        }

        public bool HasRoomAt(Vector2Int pos) => FindRoomAt(pos) != null;
    }
}