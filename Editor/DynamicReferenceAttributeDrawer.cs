using System;
using ManagedReference.Editor;
using UnityEditor;
using UnityEngine;

namespace ManagedReference
{
    [CustomPropertyDrawer(typeof(DynamicReferenceAttribute))]
    public class DynamicReferenceAttributeDrawer : BaseReferenceAttributeDrawer
    {
        private SerializedProperty _connectedProperty;
        protected override void CacheTypes(SerializedProperty property)
        {
            if (attribute is DynamicReferenceAttribute dynamicAttribute && !string.IsNullOrEmpty(dynamicAttribute.propertyName))
            {
                _connectedProperty ??= FindConnectedProperty(property, dynamicAttribute.propertyName);
                var type = _connectedProperty?.objectReferenceValue?.GetType();

                if (type != null)
                {
                    var targetType = property.GetManagedReferenceFieldType();
                    InitTypes(targetType, type);
                }
            }
        }

        private SerializedProperty FindConnectedProperty(SerializedProperty property, string propertyName)
        {
            var iterator = property.serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (iterator.name == propertyName && iterator.depth == property.depth - 1)
                {
                    return iterator;
                }
            }
            iterator.Dispose();
            return null;
        }
    }
}