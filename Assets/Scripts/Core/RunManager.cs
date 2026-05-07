using UnityEngine;
using System;

namespace Elenor {
    public class RunManager : MonoBehaviour {
        public static RunManager Instance { get; private set; }

        [SerializeField] SectionSO currentSection;

        public SectionSO CurrentSection => currentSection;
        public int CurrentFloorIndex { get; private set; }
        public FloorSO CurrentFloor => currentSection != null ? currentSection.GetFloor(CurrentFloorIndex) : null;

        public event Action<int> FloorIndexChanged;
        public event Action SectionCompleted;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy() {
            if (Instance == this) Instance = null;
        }

        void Start() {
            StartRun();
        }

        public void StartRun() {
            if (currentSection == null) {
                Debug.LogError("RunManager: no Section assigned.", this);
                return;
            }
            if (currentSection.FloorCount == 0) {
                Debug.LogError("RunManager: section has no floors.", this);
                return;
            }
            CurrentFloorIndex = 0;
            LoadCurrentFloor();
        }

        public void AdvanceFloor() {
            if (currentSection == null) return;
            int next = CurrentFloorIndex + 1;
            if (next >= currentSection.FloorCount) {
                CompleteSection();
                return;
            }
            CurrentFloorIndex = next;
            LoadCurrentFloor();
        }

        void LoadCurrentFloor() {
            FloorSO floor = CurrentFloor;
            if (floor == null) {
                Debug.LogError($"RunManager: no floor at index {CurrentFloorIndex}.", this);
                return;
            }
            if (RoomManager.Instance == null) {
                Debug.LogError("RunManager: RoomManager not found.", this);
                return;
            }
            RoomManager.Instance.LoadFloor(floor);
            FloorIndexChanged?.Invoke(CurrentFloorIndex);
        }

        void CompleteSection() {
            Debug.Log($"RunManager: section completed. (placeholder)", this);
            SectionCompleted?.Invoke();
        }

#if UNITY_EDITOR
        [ContextMenu("DEBUG: Advance Floor")]
        void DebugAdvanceFloor() => AdvanceFloor();

        [ContextMenu("DEBUG: Restart Run")]
        void DebugRestartRun() => StartRun();
#endif
    }
}