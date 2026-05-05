using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerMovement : MonoBehaviour {
        Rigidbody2D _rb;
        PlayerInputReader _input;
        PlayerStats _stats;

        public Rigidbody2D Body => _rb;
        public bool MovementLocked { get; set; }

        void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInputReader>();
            _stats = GetComponent<PlayerStats>();
        }

        void FixedUpdate() {
            if (MovementLocked) return;
            float speed = _stats != null ? _stats.MoveSpeed : 0f;
            _rb.linearVelocity = _input.MoveInput * speed;
        }

        [ContextMenu("Debug: Log MoveSpeed")]
        void DebugLogMoveSpeed() {
            float speed = _stats != null ? _stats.MoveSpeed : 0f;
            Debug.Log($"PlayerMovement: moveSpeed={speed}", this);
        }
    }
}