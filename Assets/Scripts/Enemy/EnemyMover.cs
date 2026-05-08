using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMover : MonoBehaviour {
        [SerializeField] EnemySO data;

        Rigidbody2D _rb;
        float _stunUntil;

        void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            Init(data);
        }

        public void Init(EnemySO so) {
            data = so;
            if (so == null || so.IsStationary) {
                enabled = false;
                if (_rb != null) _rb.linearVelocity = Vector2.zero;
                return;
            }
            enabled = true;
        }

        public void Stun(float duration) {
            if (duration <= 0f) return;
            _stunUntil = Mathf.Max(_stunUntil, Time.time + duration);
        }

        void FixedUpdate() {
            if (data == null || data.IsStationary) {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            if (Time.time < _stunUntil) {
                // Stunned: let physics carry whatever impulse was applied
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