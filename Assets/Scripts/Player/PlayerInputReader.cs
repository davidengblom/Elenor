using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elenor {
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputReader : MonoBehaviour {
        public Vector2 MoveInput { get; private set; }
        public Vector2 AimScreenPosition { get; private set; }
        public bool FireHeld { get; private set; }

        public event Action FirePressed;
        public event Action DashPressed;

        // Called by PlayerInput in Send Messages mode
        void OnMove(InputValue value) => MoveInput = value.Get<Vector2>();
        void OnAim(InputValue value) => AimScreenPosition = value.Get<Vector2>();

        void OnFire(InputValue value) {
            FireHeld = value.isPressed;
            if (value.isPressed) FirePressed?.Invoke();
        }

        void OnDash(InputValue value) {
            if (value.isPressed) DashPressed?.Invoke();
        }
    }
}