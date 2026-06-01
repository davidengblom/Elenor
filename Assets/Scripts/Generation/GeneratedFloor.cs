using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    public sealed class GeneratedFloor {
        public static readonly Vector2Int Origin = Vector2Int.zero;

        readonly Dictionary<Vector2Int, RoomNode> _rooms = new();

        public IReadOnlyDictionary<Vector2Int, RoomNode> Rooms => _rooms;
        public int SeedUsed { get; set; }
        public int TargetRoomCount { get; set; }
        public int FloorDepthIndex { get; set; }
        public Vector2Int StartPosition { get; set; } = Origin;
        public Vector2Int ExitPosition { get; set; }
        public bool UsedFallback { get; set; }
        public bool IsLayoutComplete => RoomCount >= TargetRoomCount;

        public int RoomCount => _rooms.Count;
        public bool TryGetRoom(Vector2Int pos, out RoomNode node) => _rooms.TryGetValue(pos, out node);
        public bool HasRoomAt(Vector2Int pos) => _rooms.ContainsKey(pos);

        public RoomNode AddRoom(Vector2Int pos) {
            var node = new RoomNode(pos);
            _rooms[pos] = node;
            return node;
        }
    }
}