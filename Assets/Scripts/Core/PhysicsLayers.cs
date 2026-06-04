using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Cached layer masks for physics queries
    /// </summary>
    public static class PhysicsLayers {
        static int _losBlockingMask = -1;

        public static int LosBlockingMask {
            get {
                if (_losBlockingMask < 0) {
                    _losBlockingMask = LayerMask.GetMask("Wall", "Obstacle");
                }
                return _losBlockingMask;
            }
        }
    }
}