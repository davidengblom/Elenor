using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    [RequireComponent(typeof(PlayerShooter))]
    public class DualStreamModifier : MonoBehaviour, IBehaviorModifier, IProjectileSpawnModifier {
        public int Level { get; set; }

        PlayerMovement _movement;

        void Awake() {
            _movement = GetComponent<PlayerMovement>();
        }

        public void ContributeDirections(Vector2 aimDir, List<Vector2> directions) {
            if (aimDir.sqrMagnitude < 0.0001f) return;

            aimDir = aimDir.normalized;
            directions.Clear();

            directions.Add(aimDir);
            directions.Add(-aimDir);

            if (Level >= 2) {
                directions.Add(GetMovementDirection());
            }

            if (Level >= 3) {
                Vector2 perpLeft = new Vector2(-aimDir.y, aimDir.x);
                Vector2 perpRight = new Vector2(aimDir.y, -aimDir.x);
                directions.Add(perpLeft);
                directions.Add(perpRight);
            }
        }

        Vector2 GetMovementDirection() {
            if (_movement != null && _movement.LastNonZeroMoveInput.sqrMagnitude > 0.0001f) {
                return _movement.LastNonZeroMoveInput.normalized;
            }
            return Vector2.up;
        }
    }
}