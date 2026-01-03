#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Simpleton
{
    [CustomPropertyDrawer(typeof(SimpletonStateMachine), useForChildren: true)]
    public class SimpletonStateMachinePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;
        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label
        )
        {
            var stateMachine = fieldInfo.GetValue(property.serializedObject.targetObject) as SimpletonStateMachine;
            GUI.enabled = stateMachine.Initial!=null;
            if (GUILayout.Button($"Inspect {property.displayName}"))
                SimpletonInspectorWindow.CreateWindow(stateMachine);
        }
    }
}
#endif
