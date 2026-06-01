using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    public static class FloorGenerator {
        static readonly Direction[] AllDirections = {
            Direction.North, Direction.South, Direction.East, Direction.West,
        };

        /// <summary>
        /// Picks target room count from config range, then runs constrained random walk.
        /// </summary>
        public static GeneratedFloor GenerateLayout(FloorGenConfigSO config, SeededRng rng) {
            int targetCount = rng.NextInt(config.MinRoomCount, config.MaxRoomCount + 1);
            return GenerateLayout(config, rng, targetCount);
        }

        /// <summary>
        /// Runs the constrained random walk for an explicit target count (debug)
        /// </summary>
        public static GeneratedFloor GenerateLayout(FloorGenConfigSO config, SeededRng rng, int targetRoomCount) {
            var floor = new GeneratedFloor {
                TargetRoomCount = targetRoomCount,
                FloorDepthIndex = config.FloorDepthIndex,
                StartPosition = GeneratedFloor.Origin,
            };

            var queue = new List<Vector2Int>();
            Vector2Int origin = GeneratedFloor.Origin;
            floor.AddRoom(origin);
            queue.Add(origin);

            while (floor.RoomCount < targetRoomCount && queue.Count > 0) {
                int queueIndex = rng.NextInt(0, queue.Count);
                Vector2Int current = queue[queueIndex];

                var directions = new List<Direction>(AllDirections);
                rng.Shuffle(directions);

                bool placedThisStep = false;
                foreach (Direction dir in directions) {
                    Vector2Int candidate = current + dir.Offset();
                    if (floor.HasRoomAt(candidate)) continue;
                    if (CountFilledNeighbors(candidate, floor.Rooms) > 1) continue;

                    floor.AddRoom(candidate);
                    queue.Add(candidate);
                    placedThisStep = true;
                    break;
                }

                if (!placedThisStep) {
                    queue.RemoveAt(queueIndex);
                }
            }

            return floor;
        }

        static int CountFilledNeighbors(Vector2Int cell, IReadOnlyDictionary<Vector2Int, RoomNode> placed) {
            int count = 0;
            foreach (Direction dir in AllDirections) {
                if (placed.ContainsKey(cell + dir.Offset())) count++;
            }
            return count;
        }
    }
}