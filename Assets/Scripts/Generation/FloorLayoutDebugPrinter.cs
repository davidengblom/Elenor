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
            sb.AppendLine(
                $"FloorGen: target={floor.TargetRoomCount}, actual={floor.RoomCount} " +
                $"layoutComplete={floor.IsLayoutComplete} valid={floor.IsValid} " +
                $"deadEnds={CountDeadEnds(floor)} exit={floor.ExitPosition}");

            for (int y = maxY; y >= minY; y--) {
                for (int x = minX; x <= maxX; x++) {
                    var pos = new Vector2Int(x, y);
                    if (!floor.HasRoomAt(pos)) {
                        sb.Append("  ");
                        continue;
                    }

                    RoomNode node = floor.Rooms[pos];

                    if (floor.IsValid || node.AssignedType.HasValue) {
                        sb.Append(TypeChar(node, pos, floor)).Append(" ");
                    } else if (pos == GeneratedFloor.Origin) {
                        sb.Append("@ ");
                    } else {
                        int connections = node.CountConnections(floor.Rooms);
                        sb.Append(connections).Append(" ");
                    }
                }
                sb.AppendLine();
            }

            sb.AppendLine("Legend: S=start E=exit W=weapon M=modifier N=normal | layout-only: @/1/2/3/4");
            return sb.ToString();
        }

        static char TypeChar(RoomNode node, Vector2Int pos, GeneratedFloor floor) {
            if (pos == floor.StartPosition) return 'S';
            if (pos == floor.ExitPosition) return 'E';

            return node.AssignedType switch {
                RoomType.Starting => 'S',
                RoomType.WeaponRoom => 'W',
                RoomType.ModifierRoom => 'M',
                _ => 'N',
            };
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