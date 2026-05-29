using UnityEngine;
using System;

namespace Elenor {
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerHealth))]
    public class PlayerDash : MonoBehaviour {
        [SerializeField] float dashSpeed = 20f;
        [SerializeField] float dashDuration = 0.18f;
        [SerializeField] float dashCooldown = 0.8f;

        PlayerInputReader _input;
        PlayerMovement _movement;
        PlayerHealth _health;

        float _nextDashTime;
        float _dashEndsAt;
        bool _isDashing;

        public event Action DashStarted;
        public event Action DashEnded;

        public Rigidbody2D Body => _movement.Body;

        float _durationMultiplier = 1f;

        public void SetDurationMultiplier(float multiplier) {
            _durationMultiplier = Mathf.Max(0.01f, multiplier);
        }

        public void RefundCooldown() {
            _nextDashTime = Time.time;
        }

        public bool IsDashing => _isDashing;
        public bool IsReady => !_isDashing && Time.time >= _nextDashTime;
        public float CooldownProgress01 => 
            _isDashing ? 0f : Mathf.Clamp01(1f - Mathf.Max(0f, _nextDashTime - Time.time) / dashCooldown);
        
        void Awake() {
            _input = GetComponent<PlayerInputReader>();
            _movement = GetComponent<PlayerMovement>();
            _health = GetComponent<PlayerHealth>();
        }

        void OnEnable() {
            _input.DashPressed += TryDash;
        }

        void OnDisable() {
            if (_input != null) _input.DashPressed -= TryDash;
            if (_isDashing) EndDash();
        }

        void Update() {
            if (_isDashing && Time.time >= _dashEndsAt) EndDash();
        }

        void TryDash() {
            if (!IsReady) return;

            Vector2 dir = ComputeDashDirection();
            if (dir.sqrMagnitude < 0.0001f) return;

            BeginDash(dir);
        }
        
        Vector2 ComputeDashDirection() {
            if (_input.MoveInput.sqrMagnitude > 0.0001f) return _input.MoveInput.normalized;
            if (_input.ShootInput.sqrMagnitude > 0.0001f) return _input.ShootInput.normalized;
            return Vector2.zero;
        }

        void BeginDash(Vector2 dir) {
            _isDashing = true;
            float duration = dashDuration * _durationMultiplier;
            _dashEndsAt = Time.time + duration;
            _nextDashTime = Time.time + dashCooldown;

            _movement.MovementLocked = true;
            _movement.Body.linearVelocity = dir * dashSpeed;

            _health.GrantInvulnerability(duration);
            DashStarted?.Invoke();
        }

        void EndDash() {
            if (!_isDashing) return;
            _isDashing = false;
            if (_movement != null) _movement.MovementLocked = false;
            DashEnded?.Invoke();
        }
    }
}