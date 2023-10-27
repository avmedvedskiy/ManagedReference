using System;
using System.Text.RegularExpressions;
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
            if (PropertyInSubArray(property))
            {
                return SearchInArrayProperty(property, propertyName);
            }

            return SearchInSingleProperty(property, propertyName);
        }

        private bool PropertyInSubArray(SerializedProperty property) => Regex.Matches(property.propertyPath, "Array").Count > 1;
        private SerializedProperty SearchInSingleProperty(SerializedProperty property, string propertyName)
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

        private SerializedProperty SearchInArrayProperty(SerializedProperty property, string propertyName)
        {
            var dataIndex = property.propertyPath.Split('.')[2];
            var iterator = property.serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                //если в пути совпадает индекс с базовым - то значит мы находимся в нужном элементе
                if (iterator.name == propertyName && iterator.depth == property.depth - 1 && iterator.propertyPath.Contains(dataIndex))
                {
                    return iterator;
                }
            }
            iterator.Dispose();
            return null;
        }
        
    }
}