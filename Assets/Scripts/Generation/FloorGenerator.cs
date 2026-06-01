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

        public static GeneratedFloor Generate(FloorGenConfigSO config, int seed) {
            var rng = new SeededRng(seed);

            for (int attempt = 0; attempt < config.MaxGenerationRetries; attempt++) {
                GeneratedFloor floor = GenerateLayout(config, rng);
                if (!floor.IsLayoutComplete) continue;
                if (!TryAssignRoomTypes(floor, config, rng)) continue;

                floor.SeedUsed = seed;
                floor.IsValid = true;
                return floor;
            }

            Debug.LogError($"FloorGenerator: generation failed after {config.MaxGenerationRetries} attempts. Falling back.", config);
            return null;
        }

        static bool TryAssignRoomTypes(GeneratedFloor floor, FloorGenConfigSO config, SeededRng rng) {
            Vector2Int origin = GeneratedFloor.Origin;

            if (!floor.TryGetRoom(origin, out RoomNode startRoom)) return false;

            // Clear any prior assignment (if we ever reuse that is)
            foreach (RoomNode node in floor.Rooms.Values) {
                node.AssignedType = null;
            }

            startRoom.AssignedType = RoomType.Starting;
            floor.StartPosition = origin;

            Vector2Int exitPos = FindFarthestRoom(floor, origin);
            floor.ExitPosition = exitPos;

            var deadEnds = new List<Vector2Int>();
            foreach (RoomNode node in floor.Rooms.Values) {
                if (node.Position == origin) continue;
                if (node.CountConnections(floor.Rooms) == 1) {
                    deadEnds.Add(node.Position);
                }
            }

            // Exit is never a special slot
            deadEnds.Remove(floor.ExitPosition);
            rng.Shuffle(deadEnds);

            var requiredSpecials = BuildRequiredSpecialSlots(config);
            var assignedSpecialPositions = new List<Vector2Int>();

            foreach ((RoomType type, int minDistance) in requiredSpecials) {
                if (!TryTakeSpecialSlot(
                    floor, deadEnds, assignedSpecialPositions, exitPos, origin,
                    type, minDistance, out Vector2Int slot)) {
                    return false;
                }

                floor.Rooms[slot].AssignedType = type;
                assignedSpecialPositions.Add(slot);
            }

            foreach (RoomNode node in floor.Rooms.Values) {
               if (!node.AssignedType.HasValue) {
                    node.AssignedType = RoomType.Normal;
                }
            }

            return true;
        }

        static List<(RoomType type, int minDistance)> BuildRequiredSpecialSlots(FloorGenConfigSO config) {
            var list = new List<(RoomType type, int)>();
            foreach (SpecialRoomRequirement req in config.SpecialRooms) {
                for (int i = 0; i < req.count; i++) {
                    list.Add((req.roomType, req.minDistanceFromStart));
                }
            }
            return list;
        }

        static bool TryTakeSpecialSlot(
            GeneratedFloor floor,
            List<Vector2Int> deadEnds,
            List<Vector2Int> assignedSpecials,
            Vector2Int exitPos,
            Vector2Int origin,
            RoomType type,
            int minDistance,
            out Vector2Int slot
        ) {
            for (int i = 0; i < deadEnds.Count; i++) {
                Vector2Int candidate = deadEnds[i];
                if (candidate == exitPos) continue;
                if (minDistance > 0 && GetPathDistance(floor, origin, candidate) < minDistance) continue;
                if (type == RoomType.ModifierRoom && IsAdjacentToModifier(candidate, assignedSpecials, floor)) continue;

                slot = candidate;
                deadEnds.RemoveAt(i);
                return true;
            }

            slot = default;
            return false;
        }

        static bool IsAdjacentToModifier(Vector2Int candidate, List<Vector2Int> assignedSpecials, GeneratedFloor floor) {
            foreach (Vector2Int pos in assignedSpecials) {
                if (!floor.Rooms.TryGetValue(pos, out RoomNode node)) continue;
                if (node.AssignedType != RoomType.ModifierRoom) continue;
                if (ManhattanDistance(candidate, pos) == 1) return true;
            }
            return false;
        }

        static int ManhattanDistance(Vector2Int a, Vector2Int b) {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        static Vector2Int FindFarthestRoom(GeneratedFloor floor, Vector2Int origin) {
            var distances = BuildDistanceMap(floor, origin);

            Vector2Int farthest = origin;
            int maxDistance = 0;
            foreach ((Vector2Int pos, int distance) in distances) {
                if (distance > maxDistance) {
                    maxDistance = distance;
                    farthest = pos;
                }
            }
            return farthest;
        }

        static int GetPathDistance(GeneratedFloor floor, Vector2Int origin, Vector2Int target) {
            return BuildDistanceMap(floor, origin).TryGetValue(target, out int d) ? d : int.MaxValue;
        }

        static Dictionary<Vector2Int, int> BuildDistanceMap(GeneratedFloor floor, Vector2Int origin) {
            var distances = new Dictionary<Vector2Int, int>();
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(origin);
            distances[origin] = 0;

            while (queue.Count > 0) {
                Vector2Int current = queue.Dequeue();
                int nextDistance = distances[current] + 1;

                foreach (Direction dir in AllDirections) {
                    Vector2Int neighbor = current + dir.Offset();
                    if (!floor.HasRoomAt(neighbor) || distances.ContainsKey(neighbor)) continue;
                    distances[neighbor] = nextDistance;
                    queue.Enqueue(neighbor);
                }
            }
            return distances;
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