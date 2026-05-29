using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elenor.UI.Screens {
    /// <summary>
    /// Base for full screen game state panels (death, section clear, pause)
    /// </summary>
    public abstract class GameScreen : MonoBehaviour {
        [Header("UI References")]
        [SerializeField] protected GameObject panel;
        [SerializeField] Button restartRunButton;
        [SerializeField] Button quitToMenuButton;

        [Header("Scene Flow")]
        [SerializeField] string gameSceneName = "Game";
        [SerializeField] string mainMenuSceneName = "MainMenu";

        protected virtual void Awake() {
            panel?.SetActive(false);
        }

        protected virtual void Start() {
            restartRunButton?.onClick.AddListener(OnRestartRunClicked);
            quitToMenuButton?.onClick.AddListener(OnQuitToMenuClicked);
        }

        protected virtual void OnDestroy() {
            restartRunButton?.onClick.RemoveListener(OnRestartRunClicked);
            quitToMenuButton?.onClick.RemoveListener(OnQuitToMenuClicked);
        }

        /// <summary> Hide the panel and resume time.
        protected virtual void Show() {
            panel?.SetActive(true);
            Time.timeScale = 0f;
        }

        /// <summary> Hide the panel and resume time.
        protected virtual void Hide() {
            panel?.SetActive(false);
            Time.timeScale = 1f;
        }

        void OnRestartRunClicked() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameSceneName);
        }

        void OnQuitToMenuClicked() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}