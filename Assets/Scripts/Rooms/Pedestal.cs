using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Handles the spawn position for item rooms.
    /// </summary>
    public class Pedestal : MonoBehaviour {
# if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = new Color(1f, 0.9f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, 0.3f);
            Gizmos.DrawLine(
                transform.position + Vector3.up * 0.3f,
                transform.position - Vector3.up * 0.3f
            );
            Gizmos.DrawLine(
                transform.position + Vector3.right * 0.3f,
                transform.position - Vector3.right * 0.3f
            );
        }
#endif
    }
}