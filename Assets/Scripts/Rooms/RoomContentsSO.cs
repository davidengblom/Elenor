using System.Collections.Generic;
using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Per-room content definition.
    /// </summary>
    [CreateAssetMenu(menuName = "Elenor/Rooms/Room Contents", fileName = "RoomContents_")]
    public class RoomContentsSO : ScriptableObject {
        [SerializeField, Tooltip("Enemy prefabs to spawn, indexed against the room's enemy spawn points in hierarchy order.")]
        List<EnemySO> initialEnemies = new();

        public IReadOnlyList<EnemySO> InitialEnemies => initialEnemies;
    }
}