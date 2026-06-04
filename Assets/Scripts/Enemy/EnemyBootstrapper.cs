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
            foreach (var component in GetComponents<IEnemyComponent>()) {
                component.Init(data);
            }

            if (data == null) return;

            if (TryGetComponent<SpriteRenderer>(out var sr)) {
                if (data.Sprite != null) sr.sprite = data.Sprite;
                sr.color = data.Tint;
            }
            transform.localScale = Vector3.one * data.Scale;

            if (TryGetComponent<Rigidbody2D>(out var rb)) {
                rb.constraints = data.IsStationary
                    ? RigidbodyConstraints2D.FreezeAll
                    : RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }
}