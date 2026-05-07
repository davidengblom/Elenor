using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMover : MonoBehaviour {
        [SerializeField] EnemySO data;

        Rigidbody2D _rb;

        void Awake() {
            _rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate() {
            if (data == null || data.MoveSpeed <= 0f) {

                _rb.linearVelocity = Vector2.zero;
                return;
            }

            Transform player = PlayerLocator.Player;
            if (player == null) {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 toPlayer = (Vector2)player.position - _rb.position;
            if (toPlayer.sqrMagnitude < 0.0001f) {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            _rb.linearVelocity = toPlayer.normalized * data.MoveSpeed;
        }
    }
}