using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Elenor {
    public class DeathScreen : MonoBehaviour {
        [Header("UI References")]
        [SerializeField] GameObject panel;
        [SerializeField] Button restartRunButton;
        [SerializeField] Button quitToMenuButton;

        [Header("Scene Flow")]
        [SerializeField] string gameSceneName = "Game";
        [SerializeField] string mainMenuSceneName = "MainMenu";

        PlayerHealth _health;

        void Awake() {
            if (panel != null) panel.SetActive(false);
        }

        void Start() {
            Transform player = PlayerLocator.Player;
            if (player != null) _health = player.GetComponent<PlayerHealth>();

            if (_health != null) {
                _health.Died += OnPlayerDied;
            } else {
                Debug.LogWarning("DeathScreen: no PlayerHealth component found.", this);
            }

            if (restartRunButton != null) {
                restartRunButton.onClick.AddListener(OnRestartClicked);
            } else {
                Debug.LogWarning("DeathScreen: no restartButton assigned.", this);
            }

            if (quitToMenuButton != null) {
                quitToMenuButton.onClick.AddListener(OnQuitToMenuClicked);
            } else {
                Debug.LogWarning("DeathScreen: no quitToMenuButton assigned.", this);
            }
        }

        void OnDestroy() {
            if (_health != null) _health.Died -= OnPlayerDied;
            if (restartRunButton != null) restartRunButton.onClick.RemoveListener(OnRestartClicked);
        }

        void OnPlayerDied() {
            if (panel != null) panel.SetActive(true);
            Time.timeScale = 0f; // Stop time
        }

        void OnRestartClicked() {
            Time.timeScale = 1f; // Resume time
            SceneManager.LoadScene(gameSceneName);
        }

        void OnQuitToMenuClicked() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}