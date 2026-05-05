using UnityEngine;

namespace Elenor {
    public class SpawnPoint : MonoBehaviour {
        public enum Kind { Enemy, Player, Door }

        [SerializeField] Kind kind = Kind.Enemy;

        public Kind PointKind => kind;

        void OnDrawGizmos() {
            Color c = kind switch {
                Kind.Enemy => new Color(1f, 0.85f, 0.2f), // yellow
                Kind.Player => new Color(0.3f, 0.9f, 1f), // cyan
                Kind.Door => new Color(0.3f, 1f, 0.4f),   // green
                _ => Color.white,
            };

            Gizmos.color = c;
            Vector3 p = transform.position;
            Gizmos.DrawWireSphere(p, 0.4f);
            Gizmos.DrawLine(p + Vector3.up * 0.4f, p - Vector3.up * 0.4f);
            Gizmos.DrawLine(p + Vector3.right * 0.4f, p - Vector3.right * 0.4f);
        }
    }
}