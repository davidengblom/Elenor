using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    public class PlayerMovement : MonoBehaviour {
        [SerializeField] float moveSpeed = 6f;

        Rigidbody2D _rb;
        PlayerInputReader _input;

        public Rigidbody2D Body => _rb;
        public bool MovementLocked { get; set; }
        public float MoveSpeed {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(0f, value);
        }

        void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInputReader>();
        }

        void FixedUpdate() {
            if (MovementLocked) return;
            _rb.linearVelocity = _input.MoveInput * MoveSpeed;
        }

        [ContextMenu("Debug: Log MoveSpeed")]
        void DebugLogMoveSpeed() {
            Debug.Log($"PlayerMovement: moveSpeed={moveSpeed}", this);
        }
    }
}