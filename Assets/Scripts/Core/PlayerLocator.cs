using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Static helper for accessing the player. Caches the transform on first successful lookup.
    /// </summary>
    public static class PlayerLocator {
        static Transform _cache;

        public static Transform Player {
            get {
                if (_cache == null) {
                    GameObject p = GameObject.FindGameObjectWithTag("Player");
                    if (p != null) _cache = p.transform;
                }
                return _cache;
            }
        }
    }
}