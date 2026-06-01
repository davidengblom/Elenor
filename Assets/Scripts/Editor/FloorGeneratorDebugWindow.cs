using UnityEngine;
using UnityEditor;

namespace Elenor.EditorTools {
    public class FloorGeneratorDebugWindow : EditorWindow {
        int _seed = 12345;
        FloorGenConfigSO _config;
        bool _overrideTargetCount;
        int _targetCount = 6;

        [MenuItem("Elenor/Floor Generator Debug")]
        static void Open() => GetWindow<FloorGeneratorDebugWindow>("Floor Gen Debug");

        void OnGUI() {
            _seed = EditorGUILayout.IntField("Seed", _seed);
            _config = (FloorGenConfigSO)EditorGUILayout.ObjectField("Config", _config, typeof(FloorGenConfigSO), false);

            _overrideTargetCount = EditorGUILayout.Toggle("Override Target Count", _overrideTargetCount);
            if (_overrideTargetCount) {
                _targetCount = EditorGUILayout.IntField("Target Count", _targetCount);
            }

            using (new EditorGUI.DisabledScope(_config == null)) {
                if (GUILayout.Button("Generate")) {
                    Generate();
                }
            }
        }

        void Generate() {
            GeneratedFloor floor = FloorGenerator.Generate(_config, _seed);

            if (floor == null) {
                Debug.LogWarning($"FloorGeneratorDebug: generation failed for seed {_seed}.");
                return;
            }

            Debug.Log(FloorLayoutDebugPrinter.ToAscii(floor));
        }
    }
}