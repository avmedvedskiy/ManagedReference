using UnityEditor;
using UnityEngine;

namespace ManagedReference
{
    public static class SelectObjectHelper
    {
        private static readonly GUIContent NullLabel = new("Null");
        public static GenericMenu CreateComponentsDropdown(Object target, SerializedProperty property)
        {
            GenericMenu nodesMenu = new GenericMenu();
            nodesMenu.AddItem(NullLabel, false, x => { OnSelect(null); }, null);

            if (target is GameObject gameObject)
            {
                //add gameobject and all components
                nodesMenu.AddItem(new GUIContent(gameObject.GetType().Name), false, OnSelect,
                    gameObject);
                var allComponents = gameObject.GetComponents<Component>();
                foreach (var component in allComponents)
                {
                    nodesMenu.AddItem(new GUIContent(component.GetType().Name), false, OnSelect,
                        component);
                }
            }
            
            return nodesMenu;

            void OnSelect(object value)
            {
                property.objectReferenceValue = (Object)value;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}