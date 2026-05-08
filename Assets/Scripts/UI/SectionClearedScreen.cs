using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Elenor {
    public class SectionClearedScreen : MonoBehaviour {
        [SerializeField] GameObject panel;
        [SerializeField] Button restartButton;
        [SerializeField] string bootSceneName = "Boot";

        void Awake() {
            if (panel != null) panel.SetActive(false);
        }

        void Start() {
            if (RunManager.Instance != null) {
                RunManager.Instance.SectionCompleted += OnSectionCompleted;
            } else {
                Debug.LogWarning("SectionClearedScreen: no RunManager found.", this);
            }

            if (restartButton != null) {
                restartButton.onClick.AddListener(OnRestartClicked);
            } else {
                Debug.LogWarning("SectionClearedScreen: no restartButton assigned.", this);
            }
        }

        void OnDestroy() {
            if (RunManager.Instance != null) {
                RunManager.Instance.SectionCompleted -= OnSectionCompleted;
            }
            if (restartButton != null) {
                restartButton.onClick.RemoveListener(OnRestartClicked);
            }
        }

        void OnSectionCompleted() {
            if (panel != null) panel.SetActive(true);
            Time.timeScale = 0f; // Stop time
        }

        void OnRestartClicked() {
            Time.timeScale = 1f; // Resume time
            SceneManager.LoadScene(bootSceneName);
        }
    }
}