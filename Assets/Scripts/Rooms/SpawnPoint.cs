using UnityEngine;

namespace Elenor {
    public class SpawnPoint : MonoBehaviour {
        public enum Kind { Enemy, Player, Door, Exit }

        [SerializeField] Kind kind = Kind.Enemy;
        [SerializeField, Tooltip("Only when Kind == Door")]
        Direction direction;

        public Kind PointKind => kind;
        public Direction DoorDirection => direction;

        void OnDrawGizmos() {
            Color c = kind switch {
                Kind.Enemy => new Color(1f, 0.85f, 0.2f), // yellow
                Kind.Player => new Color(0.3f, 0.9f, 1f), // cyan
                Kind.Door => new Color(0.3f, 1f, 0.4f),   // green
                Kind.Exit => new Color(1f, 0.4f, 0.9f),   // magenta
                _ => Color.white,
            };

            Gizmos.color = c;

            if (kind == Kind.Door || kind == Kind.Exit) {
                Vector3 arrow = direction switch {
                    Direction.North => Vector3.up,
                    Direction.South => Vector3.down,
                    Direction.East => Vector3.right,
                    Direction.West => Vector3.left,
                    _ => Vector3.zero,
                };
                Gizmos.DrawLine(transform.position, transform.position + arrow * 0.6f);
            }

            Vector3 p = transform.position;
            Gizmos.DrawWireSphere(p, 0.4f);
            Gizmos.DrawLine(p + Vector3.up * 0.4f, p - Vector3.up * 0.4f);
            Gizmos.DrawLine(p + Vector3.right * 0.4f, p - Vector3.right * 0.4f);
        }
    }
}