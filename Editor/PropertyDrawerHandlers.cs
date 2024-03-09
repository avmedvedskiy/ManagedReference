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
        private static readonly List<(Type drawer, Type targetType)> _allDrawers;
        private static readonly Dictionary<uint, PropertyDrawer> _cachedDrawers = new();

        static PropertyDrawerHandlers()
        {
            _allDrawers = GetAllCustomPropertyDrawers();
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
                return drawer.GetPropertyHeight(property,label);
                
            }
            return EditorGUI.GetPropertyHeight(property,label,true);
        }

        private static PropertyDrawer GetPropertyDrawer(SerializedProperty property)
        {
            if (property.managedReferenceValue == null)
                return null;

            foreach (var drawer in _allDrawers)
            {
                if (drawer.targetType == property.GetManagedReferenceType())
                {
                    return GetCachedPropertyDrawer(property, drawer);
                }
            }

            return null;
        }

        private static PropertyDrawer GetCachedPropertyDrawer(SerializedProperty property,
            (Type drawer, Type targetType) drawer)
        {
            if (_cachedDrawers.TryGetValue(property.contentHash, out var propertyDrawer) && propertyDrawer != null)
            {
                return propertyDrawer;
            }

            propertyDrawer = (PropertyDrawer)Activator.CreateInstance(drawer.drawer);
            _cachedDrawers[property.contentHash] = propertyDrawer;
            return propertyDrawer;
        }

        private static List<(Type, Type )> GetAllCustomPropertyDrawers() =>
            TypeCache
                .GetTypesWithAttribute<CustomPropertyDrawer>()
                .Where(x => !x.IsAbstract)
                .Select(x => (x, GetTargetType(x.GetCustomAttributes<CustomPropertyDrawer>().First())))
                .ToList();

        private static Type GetTargetType(CustomPropertyDrawer attribute) =>
            (Type)attribute
                .GetType()
                .GetField("m_Type", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(attribute);
    }
}