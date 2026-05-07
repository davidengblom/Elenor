using UnityEngine;

namespace Elenor {
    public enum Direction {
        North,
        South,
        East,
        West
    }

    public static class DirectionExtensions {
        public static Direction Opposite(this Direction dir) => dir switch {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => Direction.North,
        };

        public static Vector2Int Offset(this Direction dir) => dir switch {
            Direction.North => new Vector2Int(0, 1),
            Direction.South => new Vector2Int(0, -1),
            Direction.East => new Vector2Int(1, 0),
            Direction.West => new Vector2Int(-1, 0),
            _ => Vector2Int.zero,
        };
    }
}