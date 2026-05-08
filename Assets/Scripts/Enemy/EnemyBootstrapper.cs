using UnityEngine;

namespace Elenor {
    public class EnemyBootstrapper : MonoBehaviour {
        [SerializeField] EnemySO data;

        public EnemySO Data => data;

        public void Configure(EnemySO so) {
            data = so;
            Apply();
        }

        void Awake() {
            // Scene-placed instances. good for debug
            if (data != null) Apply();
        }

        void Apply() {
            // Could this be done in a loop?
            if (TryGetComponent<EnemyHealth>(out var h)) h.Init(data);
            if (TryGetComponent<EnemyMover>(out var m)) m.Init(data);
            if (TryGetComponent<EnemyShooter>(out var s)) s.Init(data);
            if (TryGetComponent<EnemyMelee>(out var ml)) ml.Init(data);

            if (data == null) return;

            if (TryGetComponent<SpriteRenderer>(out var sr)) {
                if (data.Sprite != null) sr.sprite = data.Sprite;
                sr.color = data.Tint;
            }
            transform.localScale = Vector3.one * data.Scale;

            if (TryGetComponent<Rigidbody2D>(out var rb)) {
                rb.constraints = data.IsStationary ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }
}