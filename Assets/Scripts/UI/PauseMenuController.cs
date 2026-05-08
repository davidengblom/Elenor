using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Elenor {
    public class PauseMenuController : MonoBehaviour {
        [Header("UI References")]
        [SerializeField] GameObject panel;
        [SerializeField] Button resumeButton;
        [SerializeField] Button restartButton;
        [SerializeField] Button quitToMenuButton;

        [Header("Scene Flow")]
        [SerializeField] string gameSceneName = "Game";
        [SerializeField] string mainMenuSceneName = "MainMenu";

        PlayerInputReader _input;
        bool _isPaused;

        void Awake() {
            if (panel != null) panel.SetActive(false);
        }

        void Start() {
            if (resumeButton != null) {
                resumeButton.onClick.AddListener(OnResumeClicked);
            }
            if (restartButton != null) {
                restartButton.onClick.AddListener(OnRestartClicked);
            }
            if (quitToMenuButton != null) {
                quitToMenuButton.onClick.AddListener(OnQuitToMenuClicked);
            }
        }

        void OnDestroy() {
            if (resumeButton != null) resumeButton.onClick.RemoveListener(OnResumeClicked);
            if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartClicked);
            if (quitToMenuButton != null) quitToMenuButton.onClick.RemoveListener(OnQuitToMenuClicked);
        }

        void Update() {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
                OnPausePressed();
            }
        }

        void OnPausePressed() {
            if (_isPaused) OnResumeClicked();
            else Pause();
        }

        void Pause() {
            if (Time.timeScale == 0f) return;
            _isPaused = true;
            Time.timeScale = 0f;
            if (panel != null) panel.SetActive(true);
        }

        void OnResumeClicked() {
            if (!_isPaused) return;
            _isPaused = false;
            Time.timeScale = 1f;
            if (panel != null) panel.SetActive(false);
        }

        void OnRestartClicked() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameSceneName);
        }

        void OnQuitToMenuClicked() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}