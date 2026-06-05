using UnityEngine;
using System;
using System.Collections.Generic;

namespace Elenor {
    [Serializable]
    public struct SpecialRoomRequirement {
        public RoomType roomType;
        [Min(1)] public int count;
        [Tooltip("BFS path distance from start.")]
        public int minDistanceFromStart;
    }

    [CreateAssetMenu(menuName = "Elenor/Floors/FloorGen Config", fileName = "FloorGenConfig_")]
    public class FloorGenConfigSO : ScriptableObject {
        [Header("Identity")]
        [SerializeField] string displayName = "Floor";
        [SerializeField, Tooltip("0-based index within the section.")]
        int floorDepthIndex;

        [Header("Generation")]
        [SerializeField, Min(1)] int minRoomCount = 6;
        [SerializeField, Min(1)] int maxRoomCount = 8;
        [SerializeField, Min(1)] int maxGenerationRetries = 10;
        [SerializeField] List<SpecialRoomRequirement> specialRooms = new();
        [SerializeField, Tooltip("Pickup rarities eligible on this floor.")]
        List<PickupRarity> allowedRarities = new() { PickupRarity.Common };

        [Header("Room Prefabs")]
        [SerializeField] GameObject startingRoomPrefab;
        [SerializeField, Tooltip("Used for weapon and modifier rooms.")]
        GameObject itemRoomPrefab;
        [SerializeField] GameObject bossArenaPrefab;
        [SerializeField] List<GameObject> normalRoomPrefabs = new();

        public string DisplayName => displayName;
        public int FloorDepthIndex => floorDepthIndex;
        public int MinRoomCount => minRoomCount;
        public int MaxRoomCount => maxRoomCount;
        public int MaxGenerationRetries => maxGenerationRetries;
        public IReadOnlyList<SpecialRoomRequirement> SpecialRooms => specialRooms;
        public IReadOnlyList<PickupRarity> AllowedRarities => allowedRarities;
        public GameObject StartingRoomPrefab => startingRoomPrefab;
        public GameObject ItemRoomPrefab => itemRoomPrefab;
        public GameObject BossArenaPrefab => bossArenaPrefab;
        public IReadOnlyList<GameObject> NormalRoomPrefabs => normalRoomPrefabs;
        public bool IsFinalFloorInSection => bossArenaPrefab != null;
    }
}