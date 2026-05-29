using UnityEngine;
using System.Collections;
using TMPro;
using Elenor;

namespace Elenor.UI.HUD {
    public class HudNotification : PickupInventoryListener {
        [SerializeField] CanvasGroup group;
        [SerializeField] TMP_Text text;
        [SerializeField] float fadeIn = 0.2f;
        [SerializeField] float fadeOut = 0.5f;
        [SerializeField, Tooltip("Hold duration used when Show is called without an explicit duration.")]
        float defaultDuration = 4.5f;

        PlayerShooter _shooter;
        Coroutine _routine;

        void Awake() {
            if (group != null) group.alpha = 0f;
        }

        protected override void Start() {
            base.Start();
            Transform player = PlayerLocator.Player;
            if (player != null) _shooter = player.GetComponent<PlayerShooter>();
            if (_shooter != null) _shooter.WeaponSwapped += OnWeaponSwapped;
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (_shooter != null) _shooter.WeaponSwapped -= OnWeaponSwapped;
        }

        public void Show(string message, float duration = -1f) {
            if (text != null) text.text = message;
            if (duration < 0f) duration = defaultDuration;
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(Run(duration));
        }

        protected override void OnPickupAcquired(PickupSO pickup, int level) {
            if (pickup == null) return;
            Show($"{pickup.DisplayName} - Lv {level}!");
        }

        //TODO: Identical to the one above.
        protected override void OnPickupLeveledUp(PickupSO pickup, int level) {
            if (pickup == null) return;
            Show($"{pickup.DisplayName} - Lv {level}!");
        }

        void OnWeaponSwapped(WeaponSO weapon) {
            if (weapon == null) return;
            Show($"Weapon swapped - {weapon.DisplayName}!");
        }

        IEnumerator Run(float hold) {
            yield return Fade(0f, 1f, fadeIn);
            yield return new WaitForSeconds(hold);
            yield return Fade(1f, 0f, fadeOut);
            _routine = null;
        }

        // TODO: This module should not be responsible for fading etc.
        IEnumerator Fade(float from, float to, float duration) {
            if (group == null || duration <= 0f) {
                if (group != null) group.alpha = to;
                yield break;
            }
            float t = 0f;
            while (t < duration) {
                t += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            group.alpha = to;
        }
    }
}