using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elenor {
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputReader : MonoBehaviour {
        public Vector2 MoveInput { get; private set; }
        public Vector2 ShootInput { get; private set; }

        public bool ShootHeld => ShootInput.sqrMagnitude > 0.01f;

        public event Action DashPressed;

        // Called by PlayerInput in Send Messages mode
        void OnMove(InputValue value) => MoveInput = value.Get<Vector2>();
        void OnShoot(InputValue value) => ShootInput = value.Get<Vector2>();

        void OnDash(InputValue value) {
            if (value.isPressed) DashPressed?.Invoke();
        }
    }
}