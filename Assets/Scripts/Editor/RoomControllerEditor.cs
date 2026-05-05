using UnityEditor;
using UnityEngine;

namespace Elenor.EditorTools
{
    [CustomEditor(typeof(RoomController))]
    public class RoomControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var room = (RoomController)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Room Markers", EditorStyles.boldLabel);

            int enemy  = room.GetSpawns(SpawnPoint.Kind.Enemy).Count;
            int player = room.GetSpawns(SpawnPoint.Kind.Player).Count;
            int door   = room.GetSpawns(SpawnPoint.Kind.Door).Count;

            EditorGUILayout.LabelField("Enemy Spawns",  enemy.ToString());
            EditorGUILayout.LabelField("Player Spawns", player == 1 ? "Set" : (player == 0 ? "MISSING" : $"{player} (too many)"));
            EditorGUILayout.LabelField("Door Anchors",  door   == 1 ? "Set" : (door   == 0 ? "MISSING" : $"{door} (too many)"));

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Enemy Spawn"))   CreateMarker(room, SpawnPoint.Kind.Enemy);
                if (GUILayout.Button("Add Player Spawn"))  CreateMarker(room, SpawnPoint.Kind.Player);
                if (GUILayout.Button("Add Door Anchor"))   CreateMarker(room, SpawnPoint.Kind.Door);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Validate Room"))
            {
                EditorUtility.DisplayDialog("Room Validation", room.Validate(), "OK");
            }
        }

        static void CreateMarker(RoomController room, SpawnPoint.Kind kind)
        {
            var go = new GameObject($"Spawn_{kind}_{System.Guid.NewGuid().ToString().Substring(0, 4)}");
            Undo.RegisterCreatedObjectUndo(go, "Create Spawn Point");
            Undo.SetTransformParent(go.transform, room.transform, "Parent Spawn Point");
            go.transform.localPosition = Vector3.zero;

            var sp = Undo.AddComponent<SpawnPoint>(go);
            var so = new SerializedObject(sp);
            so.FindProperty("kind").enumValueIndex = (int)kind;
            so.ApplyModifiedProperties();

            Selection.activeGameObject = go;
            EditorUtility.SetDirty(room);
        }
    }
}