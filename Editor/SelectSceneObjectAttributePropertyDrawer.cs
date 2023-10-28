using UnityEditor;
using UnityEngine;

namespace ManagedReference
{
    [CustomPropertyDrawer(typeof(SelectSceneObjectAttribute))]
    public class SelectSceneObjectAttributePropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent NullLabel = new("Null");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, false);
            if (EditorGUI.EndChangeCheck())
            {
                ShowDropdown(property.objectReferenceValue, property);
            }
        }

        private void ShowDropdown(Object target, SerializedProperty property)
        {
            GenericMenu nodesMenu = new GenericMenu();
            nodesMenu.AddItem(NullLabel, false, x => { OnSelect(null, property); }, null);

            if (target is GameObject gameObject)
            {
                //add gameobject and all components
                nodesMenu.AddItem(new GUIContent(gameObject.GetType().Name), false, x => { OnSelect(x, property); },
                    gameObject);
                var allComponents = gameObject.GetComponents<Component>();
                foreach (var component in allComponents)
                {
                    nodesMenu.AddItem(new GUIContent(component.GetType().Name), false, x => { OnSelect(x, property); },
                        component);
                }
            }

            nodesMenu.ShowAsContext();
        }

        private void OnSelect(object value, SerializedProperty property)
        {
            property.objectReferenceValue = (Object)value;
            property.serializedObject.ApplyModifiedProperties();
        }

    }
}