using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ManagedReference.Editor
{
    public static class PropertyDrawerHandlers
    {
        private static readonly Dictionary<uint, PropertyDrawer> _cachedDrawers = new();
        private static readonly List<(Type drawer, string targetType)> _allDrawers;

        static PropertyDrawerHandlers()
        {
            _allDrawers = GetAllDrawers();
        }

        public static void PropertyField(Rect position, SerializedProperty property, GUIContent label,
            bool includeChildren = false)
        {
            var drawer = GetPropertyDrawer(property);
            if (drawer != null)
            {
                drawer.OnGUI(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, includeChildren);
            }
        }

        public static float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var drawer = GetPropertyDrawer(property);
            if (drawer != null)
            {
                //return EditorGUIUtility.singleLineHeight;
                return drawer.GetPropertyHeight(property, label);
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private static PropertyDrawer GetPropertyDrawer(SerializedProperty property)
        {
            if (property.managedReferenceValue == null)
                return null;

            if (_cachedDrawers.TryGetValue(property.contentHash, out var propertyDrawer))
            {
                return propertyDrawer;
            }

            var managedReferenceFullTypename = property.managedReferenceFullTypename;
            var drawer = _allDrawers.Find(x => x.targetType == managedReferenceFullTypename);
            propertyDrawer = drawer.drawer != null ? (PropertyDrawer)Activator.CreateInstance(drawer.drawer) : null;
            _cachedDrawers[property.contentHash] = propertyDrawer;
            return propertyDrawer;
        }


        private static List<(Type x, string)> GetAllDrawers() =>
            TypeCache
                .GetTypesWithAttribute<CustomPropertyDrawer>()
                .Where(x => !x.IsAbstract)
                .Select(x => (x, GetTargetTypeName(x.GetCustomAttributes<CustomPropertyDrawer>().First())))
                .ToList();

        private static string GetTargetTypeName(CustomPropertyDrawer attribute)
        {
            var type = (Type)attribute
                .GetType()
                .GetField("m_Type", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(attribute);
            return $"{type.Assembly.GetName().Name} {type.FullName}";
        }
    }
}