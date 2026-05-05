using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Elenor {
    public class DeathScreen : MonoBehaviour {
        [SerializeField] GameObject panel;
        [SerializeField] Button restartButton;
        [SerializeField] string bootSceneName = "Boot";

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

            if (restartButton != null) {
                restartButton.onClick.AddListener(OnRestartClicked);
            } else {
                Debug.LogWarning("DeathScreen: no restartButton assigned.", this);
            }
        }

        void OnDestroy() {
            if (_health != null) _health.Died -= OnPlayerDied;
            if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        void OnPlayerDied() {
            if (panel != null) panel.SetActive(true);
            Time.timeScale = 0f; // Stop time
        }

        void OnRestartClicked() {
            Time.timeScale = 1f; // Resume time
            SceneManager.LoadScene(bootSceneName);
        }
    }
}