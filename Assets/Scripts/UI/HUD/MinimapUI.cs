using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Elenor;

namespace Elenor.UI.HUD {
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
            if (RoomManager.Instance == null) {
                Debug.LogWarning("MinimapUI: no RoomManager available.", this);
                return;
            }
            if (container == null) {
                Debug.LogWarning("MinimapUI: container not assigned.", this);
                return;
            }

            RoomManager.Instance.FloorChanged += OnFloorChanged;
            RoomManager.Instance.RoomChanged += OnRoomChanged;

            // If RunManager.Start ran first, the floor is already loaded
            if (RoomManager.Instance.Floor != null) {
                OnFloorChanged(RoomManager.Instance.Floor);
            }
        }

        void OnDestroy() {
            if (RoomManager.Instance != null) {
                RoomManager.Instance.FloorChanged -= OnFloorChanged;
                RoomManager.Instance.RoomChanged -= OnRoomChanged;
            }
        }

        void BuildCells() {
            FloorSO floor = RoomManager.Instance.Floor;

            Vector2Int min = new(int.MaxValue, int.MaxValue);
            Vector2Int max = new(int.MinValue, int.MinValue);
            foreach (FloorRoomEntry entry in floor.Rooms) {
                if (entry.gridPosition.x < min.x) min.x = entry.gridPosition.x;
                if (entry.gridPosition.y < min.y) min.y = entry.gridPosition.y;
                if (entry.gridPosition.x > max.x) max.x = entry.gridPosition.x;
                if (entry.gridPosition.y > max.y) max.y = entry.gridPosition.y;
            }

            int gridW = max.x - min.x + 1;
            int gridH = max.y - min.y + 1;

            Vector2 cell = cellSize;
            Vector2 spacing = cellSpacing;
            Vector2 containerSize = container.rect.size;

            float totalW = gridW * cell.x + (gridW - 1) * spacing.x;
            float totalH = gridH * cell.y + (gridH - 1) * spacing.y;

            if (totalW > containerSize.x || totalH > containerSize.y) {
                float scale = Mathf.Min(
                    containerSize.x / totalW,
                    containerSize.y / totalH
                );
                cell *= scale;
                spacing *= scale;
            }

            foreach (FloorRoomEntry entry in floor.Rooms) {
                Vector2Int local = entry.gridPosition - min;
                var go = new GameObject($"Cell_{entry.gridPosition.x}_{entry.gridPosition.y}", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(container, false);

                var rt = (RectTransform)go.transform;
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 0f);
                rt.sizeDelta = cell;
                rt.anchoredPosition = new Vector2(
                    local.x * (cell.x + spacing.x),
                    local.y * (cell.y + spacing.y)
                );

                _cells[entry.gridPosition] = go.GetComponent<Image>();
            }
        }

        void ClearCells() {
            foreach (var img in _cells.Values) {
                if (img != null) Destroy(img.gameObject);
            }
            _cells.Clear();
        }

        void OnRoomChanged(Vector2Int pos) {
            Refresh();
        }

        void OnFloorChanged(FloorSO floor) {
            ClearCells();
            BuildCells();
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