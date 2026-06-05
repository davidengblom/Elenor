using UnityEngine;
using System;

namespace Elenor {
    public class RunManager : MonoBehaviour {
        public static RunManager Instance { get; private set; }

        [SerializeField] SectionSO currentSection;

        [Header("Generation")]
        [SerializeField] bool useFixedSeed;
        [SerializeField] int fixedRunSeed = 12345;

        int _runSeed;
        FloorSO _runtimeFloor;

        public int RunSeed => _runSeed;
        public SectionSO CurrentSection => currentSection;
        public int CurrentFloorIndex { get; private set; }
        public FloorSO CurrentFloor => _runtimeFloor ?? RoomManager.Instance?.Floor;

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
            if (_runtimeFloor != null) {
                Destroy(_runtimeFloor);
            }
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
            _runSeed = useFixedSeed ? fixedRunSeed : Environment.TickCount;
            Debug.Log($"RunManager: run seed = {_runSeed}", this);
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
            if (currentSection == null) return;

            FloorGenConfigSO config = currentSection.GetFloorConfig(CurrentFloorIndex);
            if (config == null) {
                Debug.LogError($"RunManager: no floor config at index {CurrentFloorIndex}", this);
                return;
            }
            if (RoomManager.Instance == null) {
                Debug.LogError("RunManager: RoomManager not found", this);
                return;
            }

            if (_runtimeFloor != null) {
                Destroy(_runtimeFloor);
                _runtimeFloor = null;
            }

            const int extraSeedAttempts = 5;
            int floorSeed = GenerationSeed.ForFloor(_runSeed, CurrentFloorIndex);
            GeneratedFloor generated = null;
            int seedUsed = floorSeed;

            for (int seedOffset = 0; seedOffset <= extraSeedAttempts && generated == null; seedOffset++) {
                int trySeed = floorSeed + seedOffset;
                generated = FloorGenerator.Generate(config, trySeed);
                if (generated != null) {
                    seedUsed = trySeed;
                }
            }

            if (generated == null) {
                Debug.LogError(
                    $"RunManager: floor generation failed at index {CurrentFloorIndex}", this
                );
                return;
            }

            bool isFinalFloor = CurrentFloorIndex == currentSection.FloorCount - 1;
            _runtimeFloor = GeneratedFloorAdapter.ToFloorSO(generated, config, seedUsed, isFinalFloor);

            RoomManager.Instance.LoadFloor(_runtimeFloor);
            FloorIndexChanged?.Invoke(CurrentFloorIndex);
        }

        void CompleteSection() {
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