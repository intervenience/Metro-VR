
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

using MetroVR;

namespace MetroVR.Editor {

    [CustomEditor (typeof (Database))]
    public class DatabaseInspector : UnityEditor.Editor {

        //https://xinyustudio.wordpress.com/2015/07/21/unity3d-using-reorderablelist-in-custom-editor/

        //also https://sites.google.com/site/tuxnots/gamming/unity3d/unitymakeyourlistsfunctionalwithreorderablelist

        ReorderableList itemsList, magazineList, mobsList;

        void OnEnable () {

            itemsList = new ReorderableList (serializedObject, serializedObject.FindProperty ("items"), true, true, true, true);
            itemsList.elementHeight = EditorGUIUtility.singleLineHeight * 5f + 5f;
            itemsList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField (rect, "Items");
            };

            itemsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = itemsList.serializedProperty.GetArrayElementAtIndex (index);

                var id = element.FindPropertyRelative ("id");
                var gameObjectName = element.FindPropertyRelative ("gameObjectName");
                var prefab = element.FindPropertyRelative ("prefab");
                var magId = element.FindPropertyRelative ("magId");
                var magIn = element.FindPropertyRelative ("magIn");

                id.intValue = EditorGUI.IntField (new Rect (rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "ID" , id.intValue);

                gameObjectName.stringValue = EditorGUI.TextField (new Rect (rect.x, rect.y + (EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), "Name", gameObjectName.stringValue);

                EditorGUI.ObjectField (new Rect (rect.x, rect.y + (2 * EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), prefab);

                magId.intValue = EditorGUI.IntField (new Rect (rect.x, rect.y + (3 * EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), "Mag ID", magId.intValue);

                magIn.boolValue = EditorGUI.Toggle (new Rect (rect.x, rect.y + (4 * EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), "Mag Is In?", magIn.boolValue);
            };

            magazineList = new ReorderableList (serializedObject, serializedObject.FindProperty ("magazines"), true, true, true, true);
            magazineList.elementHeight = EditorGUIUtility.singleLineHeight * 3f + 5f;
            magazineList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField (rect, "Magazines");
            };

            magazineList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = magazineList.serializedProperty.GetArrayElementAtIndex (index);

                var id = element.FindPropertyRelative ("id");
                var gameObjectName = element.FindPropertyRelative ("gameObjectName");
                var prefab = element.FindPropertyRelative ("prefab");

                id.intValue = EditorGUI.IntField (new Rect (rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "ID", id.intValue);

                gameObjectName.stringValue = EditorGUI.TextField (new Rect (rect.x, rect.y + (EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), "Name", gameObjectName.stringValue);

                EditorGUI.ObjectField (new Rect (rect.x, rect.y + (2 * EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), prefab);
            };

            mobsList = new ReorderableList (serializedObject, serializedObject.FindProperty ("mobs"), true, true, true, true);
            mobsList.elementHeight = EditorGUIUtility.singleLineHeight * 3f + 5f;
            mobsList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField (rect, "Mobs");
            };

            mobsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = mobsList.serializedProperty.GetArrayElementAtIndex (index);

                var id = element.FindPropertyRelative ("id");
                var gameObjectName = element.FindPropertyRelative ("mobName");
                var prefab = element.FindPropertyRelative ("prefab");

                id.intValue = EditorGUI.IntField (new Rect (rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "ID", id.intValue);

                gameObjectName.stringValue = EditorGUI.TextField (new Rect (rect.x, rect.y + (EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), "Name", gameObjectName.stringValue);

                EditorGUI.ObjectField (new Rect (rect.x, rect.y + (2 * EditorGUIUtility.singleLineHeight), rect.width, EditorGUIUtility.singleLineHeight), prefab);
            };
        }

        public override void OnInspectorGUI () {
            serializedObject.Update ();

            itemsList.DoLayoutList ();
            magazineList.DoLayoutList ();
            mobsList.DoLayoutList ();

            serializedObject.ApplyModifiedProperties ();
            serializedObject.UpdateIfRequiredOrScript ();

            EditorUtility.SetDirty (target);
        }

    }

}
