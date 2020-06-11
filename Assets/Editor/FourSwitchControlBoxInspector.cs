
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using UnityEditor;

using MetroVR.Interactables;

namespace MetroVR.Editor {

    [CustomEditor (typeof (FourSwitchControlBox))]
    public class FourSwitchControlBoxInspector : UnityEditor.Editor {

        static GUIStyle ToggleButtonStyleNormal = null;
        static GUIStyle ToggleButtonStyleToggled = null;

        public override void OnInspectorGUI () {
            if (ToggleButtonStyleNormal == null) {
                ToggleButtonStyleNormal = "Button";
                ToggleButtonStyleToggled = new GUIStyle (ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
            }

            var box = (FourSwitchControlBox) target;

            GUILayout.BeginHorizontal ();
            if (GUILayout.Button ("Is powered?", serializedObject.FindProperty ("isPowered").boolValue ? ToggleButtonStyleToggled : ToggleButtonStyleNormal)) {
                serializedObject.FindProperty ("isPowered").boolValue = !serializedObject.FindProperty ("isPowered").boolValue;
                if (serializedObject.FindProperty ("isPowered").boolValue) {
                    serializedObject.FindProperty ("charge").floatValue = 1f;
                }
            }
            GUILayout.EndHorizontal ();
            GUILayout.BeginHorizontal ();
            if (GUILayout.Button ("Left", box.leftOn ? ToggleButtonStyleToggled : ToggleButtonStyleNormal)) {
                box.leftOn = !box.leftOn;
            }
            if (GUILayout.Button ("Inner left", box.centerLeftOn ? ToggleButtonStyleToggled : ToggleButtonStyleNormal)) {
                box.centerLeftOn = !box.centerLeftOn;
            }
            if (GUILayout.Button ("Inner right", box.centerRightOn ? ToggleButtonStyleToggled : ToggleButtonStyleNormal)) {
                box.centerRightOn = !box.centerRightOn;
            }
            if (GUILayout.Button ("Right", box.rightOn ? ToggleButtonStyleToggled : ToggleButtonStyleNormal)) {
                box.rightOn = !box.rightOn;
            }
            GUILayout.EndHorizontal ();

            GUILayout.Space (5f);
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("leftLed"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("centerLeftLed"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("centerRightLed"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("rightLed"));
            GUILayout.Space (5f);
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("leftRotator"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("centerLeftRotator"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("centerRightRotator"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("rightRotator"));
            GUILayout.Space (5f);

            EditorGUILayout.PropertyField (serializedObject.FindProperty ("unpoweredMaterial"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("redMaterial"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("greenMaterial"));
            GUILayout.Space (5f);

            serializedObject.FindProperty ("charge").floatValue = EditorGUILayout.Slider (new GUIContent ("Charge"), serializedObject.FindProperty ("charge").floatValue, 0, 1);
            if (serializedObject.FindProperty ("charge").floatValue == 1f) {
                serializedObject.FindProperty ("isPowered").boolValue = true;
            } else {
                serializedObject.FindProperty ("isPowered").boolValue = false;
            }

            EditorGUILayout.PropertyField (serializedObject.FindProperty ("onFullyChargedEvents"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("leftChargeEvents"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("centerLeftChargeEvents"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("centerRightChargeEvents"));
            EditorGUILayout.PropertyField (serializedObject.FindProperty ("rightChargeEvents"));

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties ();
            }
        }

    }
}
