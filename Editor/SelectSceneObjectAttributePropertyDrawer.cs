using UnityEditor;
using UnityEngine;

namespace ManagedReference
{
    [CustomPropertyDrawer(typeof(SelectSceneObjectAttribute))]
    public class SelectSceneObjectAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, false);
            if (EditorGUI.EndChangeCheck())
            {
                var dropdown = SelectObjectHelper.CreateComponentsDropdown(property.objectReferenceValue, property);
                dropdown.ShowAsContext();
            }
        }
    }
}