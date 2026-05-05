using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PlayerHealth))]
    public class PlayerVisuals : MonoBehaviour {
        [SerializeField] Color dashColor = Color.white;
        [SerializeField, Range(1f, 30f)] float invulnBlinkHz = 10f;
        [SerializeField, Range(0f, 1f)] float invulnMinAlpha = 0.35f;

        SpriteRenderer _renderer;
        PlayerHealth _health;
        PlayerDash _dash;
        Color _baseColor;

        void Awake() {
            _renderer = GetComponent<SpriteRenderer>();
            _health = GetComponent<PlayerHealth>();
            _dash = GetComponent<PlayerDash>();
            _baseColor = _renderer.color;
        }

        void Update() {
            if (_dash != null && _dash.IsDashing) {
                _renderer.color = dashColor;
                return;
            }

            if (_health.IsInvulnerable) {
                float t = (Mathf.Sin(Time.time * invulnBlinkHz * Mathf.PI * 2f) + 1f) * 0.5f;
                Color faded = new Color(_baseColor.r, _baseColor.g, _baseColor.b, invulnMinAlpha);
                _renderer.color = Color.Lerp(faded, _baseColor, t);
                return;
            }

            _renderer.color = _baseColor;
        }
    }
}