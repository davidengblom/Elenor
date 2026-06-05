using UnityEngine;

namespace Elenor {
    /// <summary>
    /// One way LOS aggro system.
    /// </summary>
    public class EnemyLineOfSightAggro : MonoBehaviour, IEnemyComponent {
        [SerializeField] EnemySO data;
        [SerializeField, Min(1), Tooltip("Run LOS rayccast every N frames")]
        int losCheckIntervalFrames = 4;

        SpriteRenderer _sprite;
        Color _baseColor;
        int _framePhase;

        bool _aggroed;
        bool _hasLineOfSight;
        float _leashTimer;

        bool _useLosAggro;
        float _detectionRange;
        bool _needsSightToAttack;
        float _leashDistance;
        float _leashSeconds;

        public bool IsAggroed => _aggroed;
        public bool HasLineOfSight => _hasLineOfSight;

        static int _spawnStaggerCounter;

        /// <summary> False while idle </summary>
        public bool CanMove => !_useLosAggro || _aggroed;

        /// <summary> Aggroed and if required current LOS for attacking </summary>
        public bool CanAttack => !_useLosAggro || (_aggroed && (!_needsSightToAttack || _hasLineOfSight));

        void Awake() {
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void Init(EnemySO so) {
            data = so;
            int interval = Mathf.Max(1, losCheckIntervalFrames);
            _framePhase = (_spawnStaggerCounter++) % interval;
            _leashTimer = 0f;

            if (so == null) {
                _useLosAggro = false;
                _aggroed = true;
                _hasLineOfSight = true;
                ApplyVisuals();
                return;
            }

            _detectionRange = so.DetectionRange;
            _useLosAggro = so.UsesLineOfSightAggro;
            _needsSightToAttack = so.NeedsSightToAttack;
            _leashDistance = so.LeashDistance;
            _leashSeconds = so.LeashSeconds;
            _baseColor = so.Tint;

            if (!_useLosAggro) {
                _aggroed = true;
                _hasLineOfSight = true;
            } else {
                _aggroed = false;
                _hasLineOfSight = false;
            }
            ApplyVisuals();
        }

        void Update() {
            if (!_useLosAggro) return;

            Transform player = PlayerLocator.Player;
            if (player == null) return;

            if (ShouldCheckLosThisFrame()) {
                bool inRange;
                _hasLineOfSight = HasClearLineOfSight(player, out inRange);

                if (!_aggroed) {
                    if (inRange && _hasLineOfSight) {
                        _aggroed = true;
                    }
                }
            }

            if (_aggroed && _leashSeconds > 0f) {
                float dist = Vector2.Distance(transform.position, player.position);
                bool disengaged = !_hasLineOfSight && dist > _leashDistance;
                if (disengaged) {
                    _leashTimer += Time.deltaTime;
                    if (_leashTimer >= _leashSeconds) {
                        _aggroed = false;
                        _leashTimer = 0f;
                    }
                } else {
                    _leashTimer = 0f;
                }
            }
            ApplyVisuals();
        }

        bool ShouldCheckLosThisFrame() {
            return (Time.frameCount + _framePhase) % losCheckIntervalFrames == 0;
        }

        bool HasClearLineOfSight(Transform player, out bool withinDetectionRange) {
            withinDetectionRange = false;

            Vector2 origin = transform.position;
            Vector2 target = player.position;
            Vector2 delta = target - origin;
            float dist = delta.magnitude;
            if (dist < 0.001f) {
                withinDetectionRange = true;
                return true;
            }

            if (dist > _detectionRange) {
                return false;
            }

            withinDetectionRange = true;
            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                delta / dist,
                dist,
                PhysicsLayers.LosBlockingMask
            );
            return hit.collider == null;
        }

        void ApplyVisuals() {
            if (_sprite == null) return;

            bool dormant = _useLosAggro && (!_aggroed || (_needsSightToAttack && !_hasLineOfSight));
            float alpha = dormant ? 0.45f : 1f;
            _sprite.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, _baseColor.a * alpha);
        }
    }
}