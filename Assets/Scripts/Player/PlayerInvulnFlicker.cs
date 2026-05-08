using UnityEngine;
using System.Collections;
using System;

namespace Elenor {
    [RequireComponent(typeof(PlayerHealth), typeof(SpriteRenderer))]
    public class PlayerInvulnFlicker : MonoBehaviour {
        [SerializeField, Range(0f, 1f), Tooltip("Sprite alpha at the trough of each pulse.")]
        float minAlpha = 0.3f;
        [SerializeField, Min(1f), Tooltip("Pulses per second.")]
        float pulseHz = 8f;

        SpriteRenderer _sprite;
        PlayerHealth _health;
        Color _baseColor;
        Coroutine _routine;

        void Awake() {
            _sprite = GetComponent<SpriteRenderer>();
            _health = GetComponent<PlayerHealth>();
            _baseColor = _sprite.color;
        }

        void OnEnable() {
            _health.Damaged += OnDamaged;
        }

        void OnDisable() {
            _health.Damaged -= OnDamaged;
            if (_routine != null) {
                StopCoroutine(_routine);
                _routine = null;
            }
            _sprite.color = _baseColor;
        }

        void OnDamaged(float _) {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(FlickerRoutine());
        }

        IEnumerator FlickerRoutine() {
            while (_health.IsInvulnerable && _health.IsAlive) {
                float wave = 0.5f + 0.5f * Mathf.Sin(Time.time * pulseHz * Mathf.PI * 2f);
                float a = Mathf.Lerp(minAlpha, 1f, wave);
                Color c = _baseColor;
                c.a = a;
                _sprite.color = c;
                yield return null;
            }
            _sprite.color = _baseColor;
            _routine = null;
        }
    }
}