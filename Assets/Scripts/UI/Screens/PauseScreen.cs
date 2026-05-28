using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Elenor;

namespace Elenor.UI.Screens {
    public class PauseScreen : GameScreen {
        [Header("Pause Controls")]
        [SerializeField] Button resumeButton;

        bool _isPaused = false;

        protected override void Start() {
            base.Start();
            resumeButton?.onClick.AddListener(OnResumeClicked);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            resumeButton?.onClick.RemoveListener(OnResumeClicked);
        }

        void Update() {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
                if (_isPaused) {
                    OnResumeClicked();
                } else {
                    PauseGame();
                }
            }
        }

        void PauseGame() {
            if (Time.timeScale == 0f) return;
            _isPaused = true;
            Show();
        }

        void OnResumeClicked() {
            if (!_isPaused) return;
            _isPaused = false;
            Hide();
        }
    }
}