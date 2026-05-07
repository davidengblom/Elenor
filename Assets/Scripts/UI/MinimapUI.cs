using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Elenor {
    public class MinimapUI : MonoBehaviour {
        [Header("Layout")]
        [SerializeField] RectTransform container;
        [SerializeField] Vector2 cellSize = new(24, 24);
        [SerializeField] Vector2 cellSpacing = new(2, 2);

        [Header("Colors")]
        [SerializeField] Color visitedColor = new(0.85f, 0.85f, 0.85f, 1f);
        [SerializeField] Color hintedColor = new(0.4f, 0.4f, 0.4f, 0.5f);
        [SerializeField] Color currentColor = new(0.3f, 0.8f, 1f, 1f);

        readonly Dictionary<Vector2Int, Image> _cells = new();

        void Start() {
            if (RoomManager.Instance == null || RoomManager.Instance.Floor == null) {
                Debug.LogWarning("MinimapUI: no RoomManager or Floor available.", this);
                return;
            }
            if (container == null) {
                Debug.LogWarning("MinimapUI: container not assigned.", this);
                return;
            }

            BuildCells();
            RoomManager.Instance.RoomChanged += OnRoomChanged;
            Refresh();
        }

        void OnDestroy() {
            if (RoomManager.Instance != null) {
                RoomManager.Instance.RoomChanged -= OnRoomChanged;
            }
        }

        void BuildCells() {
            FloorSO floor = RoomManager.Instance.Floor;

            // Compute min grid pos so we can offset all cells to (0,0)-based coords
            Vector2Int min = new(int.MaxValue, int.MaxValue);
            foreach (var entry in floor.Rooms) {
                if (entry.gridPosition.x < min.x) {
                    min = new Vector2Int(entry.gridPosition.x, min.y);
                }
                if (entry.gridPosition.y < min.y) {
                    min = new Vector2Int(min.x, entry.gridPosition.y);
                }
            }

            foreach (var entry in floor.Rooms) {
                Vector2Int local = entry.gridPosition - min;
                var go = new GameObject($"Cell_{entry.gridPosition.x}_{entry.gridPosition.y}", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(container, false);

                var rt = (RectTransform)go.transform;
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 0f);
                rt.sizeDelta = cellSize;
                rt.anchoredPosition = new Vector2(
                    local.x * (cellSize.x + cellSpacing.x),
                    local.y * (cellSize.y + cellSpacing.y)
                );

                _cells[entry.gridPosition] = go.GetComponent<Image>();
            }
        }

        void OnRoomChanged(Vector2Int pos) {
            Refresh();
        }

        void Refresh() {
            Vector2Int current = RoomManager.Instance.CurrentGridPos;
            var visited = new HashSet<Vector2Int>(RoomManager.Instance.ClearedRooms);
            visited.Add(current);

            foreach (var kvp in _cells) {
                Vector2Int gridPos = kvp.Key;
                Image image = kvp.Value;

                if (gridPos == current) {
                    image.color = currentColor;
                    image.enabled = true;
                } else if (visited.Contains(gridPos)) {
                    image.color = visitedColor;
                    image.enabled = true;
                } else if (HasVisitedNeighbor(gridPos, visited)) {
                    image.color = hintedColor;
                    image.enabled = true;
                } else {
                    image.enabled = false;
                }
            }
        }

        static bool HasVisitedNeighbor(Vector2Int pos, HashSet<Vector2Int> visited) {
            foreach (Direction dir in System.Enum.GetValues(typeof(Direction))) {
                if (visited.Contains(pos + dir.Offset())) return true;
            }
            return false;
        }
    }
}