using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerMovement : MonoBehaviour {
        Rigidbody2D _rb;
        PlayerInputReader _input;
        PlayerStats _stats;
        Vector2 _lastNonZeroMoveInput = Vector2.up;

        public Rigidbody2D Body => _rb;
        public bool MovementLocked { get; set; }
        public Vector2 LastNonZeroMoveInput => _lastNonZeroMoveInput;

        void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInputReader>();
            _stats = GetComponent<PlayerStats>();
        }

        void FixedUpdate() {
            if (MovementLocked) return;

            if (_input.MoveInput.sqrMagnitude > 0.0001f) {
                _lastNonZeroMoveInput = _input.MoveInput.normalized;
            }
            
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