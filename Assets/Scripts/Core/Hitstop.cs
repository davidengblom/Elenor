using System.Collections;
using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Brief time freeze on kill for visual effect.
    /// </summary>
    public class Hitstop : MonoBehaviour {
        public static Hitstop Instance { get; private set; }

        [SerializeField, Min(0.01f), Tooltip("Default freeze duration, in real (unscaled) seconds.")]
        float defaultDuration = 0.06f;

        Coroutine _routine;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy() {
            if (Instance == this) Instance = null;
        }

        public void Pulse() => Pulse(defaultDuration);

        public void Pulse(float duration) {
            if (duration <= 0f) return;
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(Routine(duration));
        }

        IEnumerator Routine(float duration) {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
            _routine = null;
        }
    }
}