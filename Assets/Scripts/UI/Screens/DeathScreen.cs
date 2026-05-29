using UnityEngine;
using Elenor;

namespace Elenor.UI.Screens {
    public class DeathScreen : GameScreen {
        PlayerHealth _health;

        protected override void Start() {
            base.Start();

            Transform player = PlayerLocator.Player;
            if (player != null) _health = player.GetComponent<PlayerHealth>();

            if (_health != null) {
                _health.Died += OnPlayerDied;
            } else {
                Debug.LogWarning("DeathScreen: no PlayerHealth component found.", this);
            }
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (_health != null) _health.Died -= OnPlayerDied;
        }

        void OnPlayerDied() {
            Show();
        }
    }
}