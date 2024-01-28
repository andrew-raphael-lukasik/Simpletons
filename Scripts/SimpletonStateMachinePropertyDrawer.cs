#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Simpleton
{
    [CustomPropertyDrawer( typeof(SimpletonStateMachine) , useForChildren: true )]
    public class SimpletonStateMachinePropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight ( SerializedProperty property , GUIContent label ) => 0;

        public override void OnGUI ( Rect position , SerializedProperty property , GUIContent label ) //{}
        {
            var stateMachine = GetActualObjectForSerializedProperty<SimpletonStateMachine>( fieldInfo , property );
            GUI.enabled = stateMachine.Initial!=null;
            if( GUILayout.Button($"Inspect {property.displayName}") )
                SimpletonInspectorWindow.InspectAI( stateMachine );
        }

        // src* http://sketchyventures.com/2015/08/07/unity-tip-getting-the-actual-object-from-a-custom-property-drawer/
        static T GetActualObjectForSerializedProperty <T> ( FieldInfo fieldInfo , SerializedProperty property ) where T : class
        {
            var obj = fieldInfo.GetValue( property.serializedObject.targetObject );
            if( obj==null ) return null;
            T actualObject = null;
            if( obj.GetType().IsArray )
            {
                var index = System.Convert.ToInt32( new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()) );
                actualObject = ((T[])obj)[index];
            }
            else actualObject = obj as T;
            return actualObject;
        }
    }
}
#endif
