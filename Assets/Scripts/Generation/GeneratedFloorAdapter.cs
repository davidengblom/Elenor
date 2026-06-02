using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    public static class GeneratedFloorAdapter {
        public static FloorSO ToFloorSO(
            GeneratedFloor generated,
            FloorGenConfigSO config,
            int floorSeed,
            bool isFinalFloorInSection
        ) {
            var floor = FloorSO.CreateRuntimeInstance();
            var entries = new List<FloorRoomEntry>();
            var prefabRng = new SeededRng(GenerationSeed.ForPrefabSelection(floorSeed));

            foreach (RoomNode node in generated.Rooms.Values) {
                RoomType type = node.AssignedType ?? RoomType.Normal;
                GameObject prefab = PickPrefab(node, generated, config, isFinalFloorInSection, prefabRng);

                if (prefab == null) {
                    Debug.LogError(
                        $"GeneratedFloorAdapter: no prefab for room at {node.Position} ({type}).",
                        config
                    );
                    return null;
                }

                entries.Add(new FloorRoomEntry {
                    roomPrefab = prefab,
                    gridPosition = node.Position,
                    roomType = type,
                    contentsOverride = null
                });
            }

            var rarities = new List<PickupRarity>(config.AllowedRarities);
            floor.SetRuntimeData(
                config.DisplayName,
                generated.StartPosition,
                generated.ExitPosition,
                entries,
                rarities
            );

            return floor;
        }

        static GameObject PickPrefab(
            RoomNode node,
            GeneratedFloor generated,
            FloorGenConfigSO config,
            bool isFinalFloorInSection,
            SeededRng prefabRng
        ) {
            RoomType type = node.AssignedType ?? RoomType.Normal;
            bool isExit = node.Position == generated.ExitPosition;

            if (type == RoomType.Starting) {
                return config.StartingRoomPrefab != null ? config.StartingRoomPrefab : PickNormalPrefab(config, prefabRng);
            }

            if (type == RoomType.WeaponRoom || type == RoomType.ModifierRoom) {
                return config.ItemRoomPrefab;
            } 

            if (isExit && isFinalFloorInSection && config.BossArenaPrefab != null) {
                return config.BossArenaPrefab;
            }

            return PickNormalPrefab(config, prefabRng);
        }

        static GameObject PickNormalPrefab(FloorGenConfigSO config, SeededRng prefabRng) {
            IReadOnlyList<GameObject> pool = config.NormalRoomPrefabs;
            if (pool == null || pool.Count == 0) return null;
            return pool[prefabRng.NextInt(0, pool.Count)];
        }
    }
}