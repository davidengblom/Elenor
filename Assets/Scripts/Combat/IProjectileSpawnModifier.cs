using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    /// <summary>
    /// Modifies how or how many projectiles are spawned.
    /// </summary>
    public interface IProjectileSpawnModifier {
        /// <summary>
        /// Populate directions for this shot.
        /// </summary>
        void ContributeDirections(Vector2 aimDir, List<Vector2> directions);
    }
}