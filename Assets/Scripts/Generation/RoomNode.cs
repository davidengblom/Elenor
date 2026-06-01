using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    public sealed class RoomNode {
        public Vector2Int Position { get; }
        public RoomType? AssignedType { get; set; }
        public GameObject RoomPrefab { get; set; }
        public RoomContentsSO ContentsOverride { get; set; }

        public RoomNode(Vector2Int position) {
            Position = position;
        }

        public int CountConnections(IReadOnlyDictionary<Vector2Int, RoomNode> placed) {
            int count = 0;
            foreach (Direction dir in AllDirections) {
                if (placed.ContainsKey(Position + dir.Offset())) count++;
            }
            return count;
        }

        static readonly Direction[] AllDirections = {
            Direction.North, Direction.South, Direction.East, Direction.West,
        };
    }
}