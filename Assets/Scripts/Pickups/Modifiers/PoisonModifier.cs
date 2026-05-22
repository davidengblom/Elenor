using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Behavior modifier added to the player when Pickup_Poison is acquired.
    /// </summary>
    public class PoisonModifier : MonoBehaviour, IBehaviorModifier {
        public int Level { get; set; }

        public float Dps { get; private set; }
        public float Duration { get; private set; }
        public bool SpawnCloudOnDeath { get; private set; }
        public float CloudRadius { get; private set; }
        public float CloudDuration { get; private set; }
        public PoisonCloud CloudPrefab { get; private set; }

        public void Configure(
            float dps,
            float duration,
            bool spawnCloudOnDeath,
            float cloudRadius,
            float cloudDuration,
            PoisonCloud cloudPrefab
        ) {
            Dps = dps;
            Duration = duration;
            SpawnCloudOnDeath = spawnCloudOnDeath;
            CloudRadius = cloudRadius;
            CloudDuration = cloudDuration;
            CloudPrefab = cloudPrefab;
        }
    }
}