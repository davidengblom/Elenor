using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Elenor {
    public static class FloorLayoutDebugPrinter {
        public static string ToAscii(GeneratedFloor floor) {
            if (floor.RoomCount == 0) return "No rooms generated.";

            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            foreach (Vector2Int pos in floor.Rooms.Keys) {
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.y > maxY) maxY = pos.y;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"FloorGen: target={floor.TargetRoomCount}, actual={floor.RoomCount} complete={floor.IsLayoutComplete} deadEnds={CountDeadEnds(floor)}");

            for (int y = maxY; y >= minY; y--) {
                for (int x = minX; x <= maxX; x++) {
                    var pos = new Vector2Int(x, y);
                    if (!floor.HasRoomAt(pos)) {
                        sb.Append("  ");
                        continue;
                    }

                    if (pos == GeneratedFloor.Origin) {
                        sb.Append("@ ");
                        continue;
                    }

                    int connections = floor.Rooms[pos].CountConnections(floor.Rooms);
                    sb.Append(connections).Append(" ");
                }
                sb.AppendLine();
            }

            sb.AppendLine("Legend: @ = origin/start 1 = dead-end 2 = corridor 3 = junciton (blank = no room)");
            return sb.ToString();
        }

        static int CountDeadEnds(GeneratedFloor floor) {
            int count = 0;
            foreach (RoomNode node in floor.Rooms.Values) {
                if (node.Position == GeneratedFloor.Origin) continue;
                if (node.CountConnections(floor.Rooms) == 1) count++;
            }
            return count;
        }
    }
}